using HandyLink.Model.Database.Enums;
using HandyLink.Services.Database.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Services.Database
{
    public partial class HandyLinkDbContext
    {
        private void CreateSeed(ModelBuilder modelBuilder)
        {
            SeedCountries(modelBuilder);
            SeedCities(modelBuilder);
            SeedJobStatuses(modelBuilder);
            SeedUserStatuses(modelBuilder);
            SeedServiceCategories(modelBuilder);
            //SeedUsers(modelBuilder);
            //SeedAdminProfiles(modelBuilder);
        }

        private void SeedCountries(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Country>().HasData(
                new
                {
                    Id = 1,
                    Name = "Bosnia and Herzegovina",
                }
            );
        }

        private void SeedCities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<City>().HasData(
                new
                {
                    Id = 1,
                    Name = "Mostar",
                    CountryId = 1,

                },
                new
                {
                    Id = 2,
                    Name = "Sarajevo",
                    CountryId = 1,
                },
                new
                {
                    Id = 3,
                    Name = "Tuzla",
                    CountryId = 1,
                },
                new
                {
                    Id = 4,
                    Name = "Banja Luka",
                    CountryId = 1,
                }
            );
        }

        private void SeedUserStatuses(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserStatus>().HasData(
                new
                {
                    Id = 1,
                    Name = "Active",
                    Code = "ACTIVE",
                    Description = "User can use the application normally as a client or handyman.",
                },
                new
                {
                    Id = 2,
                    Name = "Pending",
                    Code = "PENDING",
                    Description = "User is waiting for handyman application approval.",
                },
                new
                {
                    Id = 3,
                    Name = "Rejected",
                    Code = "REJECTED",
                    Description = "Users handyman application was rejected.",
                },
                new
                {
                    Id = 4,
                    Name = "Blocked",
                    Code = "BLOCKED",
                    Description = "User is blocked from using the application.",
                }
            );
        }

        private void SeedJobStatuses(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<JobStatus>().HasData(
                new
                {
                    Id = 1,
                    Name = "Pending",
                    Code = "PENDING",
                    Description = "Job is created but not confirmed yet.",
                },
                new
                {
                    Id = 2,
                    Name = "Confirmed",
                    Code = "CONFIRMED",
                    Description = "Both sides agreed on the job.",
                },
                new
                {
                    Id = 3,
                    Name = "Completed",
                    Code = "COMPLETED",
                    Description = "Job was completed.",
                },
                new
                {
                    Id = 4,
                    Name = "Cancelled",
                    Code = "CANCELLED",
                    Description = "Job was cancelled.",
                }
            );
        }

        private void SeedServiceCategories(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ServiceCategory>().HasData(
                new
                {
                    Id = 1,
                    Name = "Plumbing",
                    Description = "Water pipes, leaks, sinks, toilets, and similar plumbing work.",
                },
                new
                {
                    Id = 2,
                    Name = "Electrical Work",
                    Description = "Electrical repairs, installations, outlets, lights, and wiring.",
                },
                new
                {
                    Id = 3,
                    Name = "Painting",
                    Description = "Interior and exterior painting jobs.",
                },
                new
                {
                    Id = 4,
                    Name = "Carpentry",
                    Description = "Woodwork, furniture repairs, doors, shelves, and similar jobs.",
                },
                new
                {
                    Id = 5,
                    Name = "Tiling",
                    Description = "Wall and floor tile installation and repair.",
                }
            );
        }

        private void SeedUsers(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
                new
                {
                    Id = 1,
                    FirstName = "Admin",
                    LastName = "User",
                    Email = "admin@handylink.ba",
                    PasswordHash = "CHANGE_THIS_HASH",
                    PasswordSalt = "CHANGE_THIS_SALT",
                    PhoneNumber = "060000000",
                    ProfileImageBase64 = (string?)null,
                    UserType = UserType.Admin,
                    CityId = 1,
                    UserStatusId = 1,
                }
            );
        }

        private void SeedAdminProfiles(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AdminProfile>().HasData(
                new
                {
                    Id = 1,
                    UserId = 1,
                }
            );
        }
    }
}
