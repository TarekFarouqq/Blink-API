
using Blink_API.MapperConfigs;
using Blink_API.Models;
using Blink_API.Repositories;
using Blink_API.Services;
using Blink_API.UnitOfWorks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Blink_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext <BlinkDbContext> (s => 
            { s.UseSqlServer(builder.Configuration.GetConnectionString("conString")); });

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores <BlinkDbContext>();


            //Add UnitOfWork
            builder.Services.AddScoped<UnitOfWork>();
            //Add ProductRepo
            builder.Services.AddScoped<ProductRepo>();
            //Add ProductService
            builder.Services.AddScoped<ProductService>();





            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddAutoMapper(typeof(MapperConfig));
            builder.Services.AddCors(Options =>
            {
                Options.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwaggerUI(app => app.SwaggerEndpoint("/openapi/v1.json", "v1"));
            }

            //Allow to use wwwroot
            app.UseStaticFiles();

            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.UseCors();

            app.MapControllers();

            app.Run();


            // Jimmy is herez

        }
    }
}
