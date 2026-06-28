using FluentValidation;
using HandyLink.Model.Requests;
using HandyLink.Services;
using HandyLink.Services.Database;
using HandyLink.Services.Hashing;
using HandyLink.Services.Interfaces;
using HandyLink.Services.Mapping;
using HandyLink.Services.Validators;
using HandyLink.WebApi.Common;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;



namespace HandyLink.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            MappingConfig.RegisterMappings();

            // Add services to the container.
            builder.Services.AddControllers();

            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddProblemDetails();

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<HandyLinkDbContext>(options =>
                options.UseSqlServer(connectionString));


            builder.Services.AddMapster();

            builder.Services.AddScoped<ICountryService, CountryService>();
            builder.Services.AddScoped<ICityService, CityService>();
            builder.Services.AddScoped<IServiceCategoryService, ServiceCategoryService>();
            builder.Services.AddScoped<IUserStatusService, UserStatusService>();
            builder.Services.AddScoped<IJobStatusService, JobStatusService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IHandymanApplicationService, HandymanApplicationService>();
            builder.Services.AddScoped<IHandymanProfileService, HandymanProfileService>();
            builder.Services.AddScoped<IClientProfileService, ClientProfileService>();
            builder.Services.AddScoped<IJobService, JobService>();
            builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            builder.Services.AddScoped<IAuthService, AuthService>();

            builder.Services.AddScoped<IHashingService, HashingService>();


            builder.Services.AddScoped<IValidator<CountryInsertRequest>, CountryInsertValidator>();
            builder.Services.AddScoped<IValidator<CountryUpdateRequest>, CountryUpdateValidator>();
            builder.Services.AddScoped<IValidator<CityInsertRequest>, CityInsertValidator>();
            builder.Services.AddScoped<IValidator<CityUpdateRequest>, CityUpdateValidator>();
            builder.Services.AddScoped<IValidator<ServiceCategoryInsertRequest>, ServiceCategoryInsertValidator>();
            builder.Services.AddScoped<IValidator<ServiceCategoryUpdateRequest>, ServiceCategoryUpdateValidator>();
            builder.Services.AddScoped<IValidator<UserStatusInsertRequest>, UserStatusInsertValidator>();
            builder.Services.AddScoped<IValidator<UserStatusUpdateRequest>, UserStatusUpdateValidator>();
            builder.Services.AddScoped<IValidator<JobStatusInsertRequest>, JobStatusInsertValidator>();
            builder.Services.AddScoped<IValidator<JobStatusUpdateRequest>, JobStatusUpdateValidator>();
            builder.Services.AddScoped<IValidator<UserInsertRequest>, UserInsertValidator>();
            builder.Services.AddScoped<IValidator<UserUpdateRequest>, UserUpdateValidator>();
            builder.Services.AddScoped<IValidator<HandymanApplicationInsertRequest>, HandymanApplicationInsertValidator>();
            builder.Services.AddScoped<IValidator<HandymanProfileInsertRequest>, HandymanProfileInsertValidator>();
            builder.Services.AddScoped<IValidator<HandymanProfileUpdateRequest>, HandymanProfileUpdateValidator>();
            builder.Services.AddScoped<IValidator<JobInsertRequest>, JobInsertValidator>();
            builder.Services.AddScoped<IValidator<JobProposalInsertRequest>, JobProposalInsertValidator>();



            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = builder.Configuration["JwtToken:Issuer"],
                    ValidAudience = builder.Configuration["JwtToken:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["JwtToken:SecretKey"] ?? string.Empty)
                    ),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

            builder.Services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
            });


            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(
                options =>
                {
                    options.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Version = "v1",
                        Title = "HandyLink API",
                        Description = "API for HandyLink application"
                    });
                    var jwtSecurityScheme = new OpenApiSecurityScheme
                    {
                        BearerFormat = "JWT",
                        Name = "JWT Authentication",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.Http,
                        Scheme = JwtBearerDefaults.AuthenticationScheme,
                        Reference = new OpenApiReference
                        {
                            Id = JwtBearerDefaults.AuthenticationScheme,
                            Type = ReferenceType.SecurityScheme
                        }
                    };

                    options.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);
                    options.AddSecurityRequirement(new OpenApiSecurityRequirement
                            {
                    { jwtSecurityScheme, Array.Empty<string>() }
                            });
                });

            var app = builder.Build();

            app.UseExceptionHandler();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
