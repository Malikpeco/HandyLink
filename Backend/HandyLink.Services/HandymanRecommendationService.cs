using HandyLink.Model.Responses;
using HandyLink.Services.Database;
using HandyLink.Services.Database.Entities;
using HandyLink.Services.Exceptions;
using HandyLink.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Services
{
    public class HandymanRecommendationService : IHandymanRecommendationService
    {
        private readonly HandyLinkDbContext _dbContext;

        public HandymanRecommendationService(HandyLinkDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<IReadOnlyList<RecommendedHandymanProfileResponse>> RecommendAsync(int clientProfileId, int take = 3, CancellationToken cancellationToken = default)
        {
            var clientProfile = await _dbContext.ClientProfiles.Include(x=>x.User).ThenInclude(x=>x.City).FirstOrDefaultAsync(x => x.Id == clientProfileId);
            if (clientProfile == null)
                throw new HandyLinkNotFoundException($"ClientProfile with Id {clientProfileId} not found.");

            var city = clientProfile.User.City;

            var previousJobs = await _dbContext.Jobs.Where(x => x.ClientProfileId == clientProfileId)
                .Select(x => new
                {
                    x.ServiceCategoryId,
                    x.CityId
                })
                .ToListAsync();

            var preferredServiceCategoryIds = previousJobs
                .Select(x => x.ServiceCategoryId)
                .Distinct()
                .ToList();

            var preferredCityIds = previousJobs
                .Select(x => x.CityId)
                .Distinct()
                .ToList();

            var hasHistory = previousJobs.Any();

            var query = _dbContext.HandymanProfiles.AsQueryable();

            query = IncludeRelatedEntitiesQuery(query);

            var handymanProfiles = await query.ToListAsync(cancellationToken);

            var recommendations = new List<RecommendedHandymanProfileResponse>();

            foreach(var handyman in handymanProfiles)
            {
                var recommendation = CreateRecommendation(handyman, clientProfile, preferredServiceCategoryIds, preferredCityIds, hasHistory);

                recommendations.Add(recommendation);
            }

            recommendations = recommendations
                .OrderByDescending(x => x.RecommendationScore)
                .ThenByDescending(x => x.AverageRating)
                .ThenByDescending(x => x.ReviewCount)
                .Take(take)
                .ToList();


            return recommendations;
        }



        private RecommendedHandymanProfileResponse CreateRecommendation(HandymanProfile handyman, ClientProfile client, List<int> preferredServiceCategoryIds, List<int> preferredCityIds, bool hasClientHistory)
        {
            var averageRating = GetAverageRating(handyman.Id);

            var reviewCount = GetReviewCount(handyman.Id);

            var completedJobsCount = GetCompletedJobsCount(handyman.Id);

            var sameCityAsClient = handyman.User.CityId == client.User.CityId;

            var matchesPreviousCategory = handyman.HandymanServiceCategories
                .Any(x => preferredServiceCategoryIds.Contains(x.ServiceCategoryId));

            var matchesPreviousCity = preferredCityIds
                .Contains(handyman.User.CityId);

            var recommendationScore = CalculateRecommendationScore(
                hasClientHistory,
                sameCityAsClient,
                matchesPreviousCategory,
                matchesPreviousCity,
                averageRating,
                reviewCount,
                completedJobsCount);

            var explanation = BuildExplanation(
                hasClientHistory,
                sameCityAsClient,
                matchesPreviousCategory,
                matchesPreviousCity,
                averageRating,
                reviewCount,
                completedJobsCount);

            return new RecommendedHandymanProfileResponse
            {
                HandymanProfileId = handyman.Id,
                UserFullName = handyman.User.FirstName + " " + handyman.User.LastName,
                CityName = handyman.User.City.Name,
                HandymanServiceCategoryNames= handyman.HandymanServiceCategories
                    .Select(x=>x.ServiceCategory.Name)
                    .ToList(),
                AverageRating = averageRating,
                ReviewCount = reviewCount,
                JobsCompleted = completedJobsCount,
                RecommendationScore = recommendationScore,
                Explanation = explanation
            };
        }


        private decimal GetAverageRating(int handymanProfileId)
        {
            return _dbContext.Reviews
                .Where(x => x.HandymanProfileId == handymanProfileId)
                .Select(x => (decimal?)x.Rating)
                .Average() ?? 0;
        }

        private int GetReviewCount(int handymanProfileId)
        {
            return _dbContext.Reviews
                .Count(x => x.HandymanProfileId == handymanProfileId);
        }

        private int GetCompletedJobsCount(int handymanProfileId)
        {
            return _dbContext.Jobs
                .Count(x => x.HandymanProfileId == handymanProfileId
                         && x.JobStatus.Code == "COMPLETED");
        }


        private decimal CalculateRecommendationScore(bool hasClientHistory, bool sameCityAsClient, bool matchesPreviousCategory, bool matchesPreviousCity, decimal averageRating, int reviewCount, int completedJobsCount)
        {
            decimal score = 0;

            if (sameCityAsClient)
            {
                score += 25;
            }

            if (hasClientHistory && matchesPreviousCategory)
            {
                score += 40;
            }

            if (hasClientHistory && matchesPreviousCity)
            {
                score += 15;
            }

            score += averageRating * 10;
            score += reviewCount;
            score += completedJobsCount * 2;

            return score;
        }


        private static string BuildExplanation(bool hasClientHistory, bool sameCityAsClient, bool matchesPreviousCategory, bool matchesPreviousCity, decimal averageRating, int reviewCount, int completedJobsCount)
        {
            var reasons = new List<string>();

            if (sameCityAsClient)
            {
                reasons.Add("works in your city");
            }

            if (hasClientHistory && matchesPreviousCategory)
            {
                reasons.Add("matches service categories from your previous jobs");
            }

            if (hasClientHistory && matchesPreviousCity)
            {
                reasons.Add("matches locations from your previous jobs");
            }

            if (averageRating > 0)
            {
                reasons.Add($"has an average rating of {averageRating:0.0}");
            }

            if (reviewCount > 0)
            {
                reasons.Add($"has {reviewCount} review(s)");
            }

            if (completedJobsCount > 0)
            {
                reasons.Add($"has completed {completedJobsCount} job(s)");
            }

            if (reasons.Count == 0)
            {
                return "Recommended as one of the available handymen on the platform.";
            }

            return "Recommended because this handyman " + string.Join(", ", reasons) + ".";
        }


        private IQueryable<HandymanProfile> IncludeRelatedEntitiesQuery(IQueryable<HandymanProfile> query)
        {
            return query
                .Include(x => x.User)
                    .ThenInclude(x => x.City)
                .Include(x => x.HandymanServiceCategories)
                    .ThenInclude(x=>x.ServiceCategory);
        }
    }
}
