using HandyLink.Model.Requests;
using HandyLink.Model.Responses;
using HandyLink.Services.Constants;
using HandyLink.Services.Database;
using HandyLink.Services.Database.Entities;
using HandyLink.Services.Exceptions;
using HandyLink.Services.Hashing;
using HandyLink.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


namespace HandyLink.Services
{
    public class AuthService : IAuthService
    {
        private readonly HandyLinkDbContext _dbContext;
        private readonly IUserService _userService;
        private readonly IHashingService _hashingService;
        private readonly IConfiguration _configuration;
        private readonly IRefreshTokenService _refreshTokenService;

        public AuthService(HandyLinkDbContext dbContext, IUserService userService, IHashingService hashingService, IConfiguration configuration, IRefreshTokenService refreshTokenService)
        {
            _dbContext = dbContext;
            _userService = userService;
            _hashingService = hashingService;
            _configuration = configuration;
            _refreshTokenService = refreshTokenService;
        }

        public async Task<UserLoginResponse> LoginAsync(UserLoginRequest request)
        {
            var user = await _dbContext.Users.Include(x=>x.UserStatus).FirstOrDefaultAsync(x=>x.Email==request.Email);


            if (user == null)
            {
                throw new HandyLinkNotFoundException($"User with email {request.Email} doesn't exist");
            }

            var validPassword = _hashingService.Verify(user.PasswordHash, user.PasswordSalt, request.Password);
            if (!validPassword)
            {
                throw new HandyLinkNotFoundException("Wrong credential");
            }

            var accessToken = GenerateToken(user);
            var refreshTokenValue = GenerateRefreshToken();

            var refreshToken = new RefreshToken
            {
                UserId = user.Id,
                Token = refreshTokenValue,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };

            await _refreshTokenService.InsertAsync(refreshToken);

            return new UserLoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenValue
            };


        }

        public async Task<UserLoginResponse> RefreshAccessTokenAsync(RefreshAccessTokenRequest request)
        {
            if (string.IsNullOrEmpty(request.RefreshToken))
            {
                throw new HandyLinkBusinessRuleException("Refresh token is required");
            }

            var refreshToken = await _refreshTokenService.GetStoredTokenAsync(request.RefreshToken);

            if (refreshToken == null)
            {
                throw new HandyLinkValidationException("Invalid refresh token");
            }

            if (refreshToken.ExpiresAt < DateTime.UtcNow)
            {
                throw new HandyLinkBusinessRuleException("Refresh token has expired");
            }

            var user = await _dbContext.Users
             .Include(x => x.UserStatus)
             .FirstOrDefaultAsync(x => x.Id == refreshToken.UserId);

            if (user == null)
            {
                throw new HandyLinkNotFoundException("User not found");
            }

            if (user.UserStatus.Code != "ACTIVE") 
            {
                throw new HandyLinkBusinessRuleException("User is not active");
            }

            await _refreshTokenService.DeleteAllUserRefreshTokensAsync(user.Id);

            var accessToken = GenerateToken(user);
            var refreshTokenValue = GenerateRefreshToken();

            var token = new RefreshToken
            {
                UserId = user.Id,
                Token = refreshTokenValue,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };

            await _refreshTokenService.InsertAsync(token);

            return new UserLoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenValue
            };
        }


        private string GenerateToken(User user)
        {
            string secretKeyString = _configuration["JwtToken:SecretKey"] ?? string.Empty;
            var issuer = _configuration["JwtToken:Issuer"];
            var audience = _configuration["JwtToken:Audience"];
            var durationInMinutes = int.Parse(_configuration["JwtToken:DurationInMinutes"] ?? "1");

            var secretKey = Encoding.ASCII.GetBytes(secretKeyString);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimNames.Id, user.Id.ToString()),
                    new Claim(ClaimNames.FirstName, user.FirstName ?? string.Empty),
                    new Claim(ClaimNames.LastName, user.LastName ?? string.Empty),
                    new Claim(ClaimNames.Email, user.Email ?? string.Empty),
                    new Claim(ClaimNames.UserType, user.UserType.ToString()),
                    new Claim(ClaimNames.UserStatus, user.UserStatus.Code)
                }),
                Expires = DateTime.UtcNow.AddMinutes(durationInMinutes),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        private static string GenerateRefreshToken()
        {
            var randombytes = RandomNumberGenerator.GetBytes(64);
            return Convert.ToBase64String(randombytes);
        }

    }
}
