using Blink_API.MapperConfigs;
using Blink_API.Models;
using Blink_API.Repositories;
using Blink_API.Repositories.BranchRepos;
using Blink_API.Repositories.DiscountRepos;
using Blink_API.Services;
using Blink_API.Services.AuthServices;

using Blink_API.Services.BrandServices;

using Blink_API.Services.BranchServices;

using Blink_API.Services.CartService;
using Blink_API.Services.DiscountServices;
using Blink_API.Services.Product;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Blink_API.Services.InventoryService;
using Blink_API.Repositories.InventoryRepos;
using Blink_API.Services.BiDataService;

namespace Blink_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<BlinkDbContext>(s =>
            { s.UseSqlServer(builder.Configuration.GetConnectionString("conString")); });

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<BlinkDbContext>()
                .AddDefaultTokenProviders()
                .AddSignInManager();

            // add category repo
            builder.Services.AddScoped<CategoryRepo>();
            //addonf category services 
            builder.Services.AddScoped<CategoryService>();
            // Add Mapper
            builder.Services.AddAutoMapper(typeof(MapperConfig));
            builder.Services.AddScoped<CartService>();
            //Add UnitOfWork
            builder.Services.AddScoped<UnitOfWork>();
            //Add ProductRepo
            builder.Services.AddScoped<ProductRepo>();
            //Add ProductService
            builder.Services.AddScoped<ProductService>();
            //Add DiscountRepo
            builder.Services.AddScoped<DiscountRepo>();
            //Add DiscountService
            builder.Services.AddScoped<DiscountService>();
            // Add Branch Services
            builder.Services.AddScoped<BranchServices>();
            // Add Branch REPo
            builder.Services.AddScoped<BranchRepos>();
            //add Inventory Repo
            builder.Services.AddScoped<InventoryRepo>();
            //Add Inventory Service
            builder.Services.AddScoped<InventoryService>();

            // for biii
            builder.Services.AddScoped<BiStockService>();

            //Add New AuthService
            builder.Services.AddScoped<AuthServiceUpdated>();
 

            //Add Brand :
            builder.Services.AddScoped<BrandService>();


            // to store verify code :
            builder.Services.AddMemoryCache();

    //        builder.Services.AddControllers()
    //.AddJsonOptions(options =>
    //{
    //    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    //});


            #region Add AUTH SERVICES

            builder.Services.AddScoped(typeof(IAuthServices), typeof(AuthServices));
           
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options => {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["JWT:ValidAudience"],
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecretKey"])),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromDays(double.Parse(builder.Configuration["JWT:DurationInDays"]))


                };
            });
            #endregion
            // for email service :
            builder.Services.AddScoped<IEmailService, EmailService>();

            builder.Services.AddControllers();
                // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
                builder.Services.AddOpenApi();
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

            app.UseStaticFiles();

                app.UseHttpsRedirection();
                app.UseAuthentication();
                app.UseAuthorization();
                app.UseCors();

                app.MapControllers();

                app.Run();


                // Jimmy is herez

            }
    }
    }

