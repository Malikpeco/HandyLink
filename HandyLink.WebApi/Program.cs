using FluentValidation;
using HandyLink.Model.Requests;
using HandyLink.Services;
using HandyLink.Services.Database;
using HandyLink.Services.Interfaces;
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

            // Add services to the container.
            builder.Services.AddControllers();

            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddProblemDetails();

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            
            builder.Services.AddDbContext<HandyLinkDbContext>(options =>
                options.UseSqlServer(connectionString));


            builder.Services.AddMapster();

            builder.Services.AddScoped<ICountryService, CountryService>();

            builder.Services.AddScoped<IValidator<CountryInsertRequest>, CountryInsertValidator>();
            builder.Services.AddScoped<IValidator<CountryUpdateRequest>, CountryUpdateValidator>();

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
