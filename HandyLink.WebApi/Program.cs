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

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.UseExceptionHandler();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
