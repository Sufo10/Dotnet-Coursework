using System;
using System.Text;
using Coursework.Application.Common.Interface;
using Coursework.Domain.Entities;
using Coursework.Infrastructure.Persistent;
using Coursework.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Coursework.Infrastructure.DI
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDBContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("CAPostgreSQL"),
                b => b.MigrationsAssembly(typeof(ApplicationDBContext).Assembly.FullName)), ServiceLifetime.Transient);

            services.AddIdentity<AppUser,IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890@.";
            }).AddEntityFrameworkStores<ApplicationDBContext>().AddDefaultTokenProviders();


            services.ConfigureApplicationCookie(options => {
                options.Cookie.SameSite = SameSiteMode.None;
                options.Cookie.Name = "Access-Cookie";
            });
            services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidAudience =  configuration["Jwt:Audience"],
            ValidIssuer = configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration["Jwt:Key"])
            )
        };
    });



            services.AddScoped<IApplicationDBContext>(provider => provider.GetService<ApplicationDBContext>());
            services.AddTransient<IDateTime, DateTimeService>();
            services.AddTransient<IFileStorage,ServerFileStorage>();
            services.AddTransient<ICarDetails, CarService>();
            services.AddTransient<IAuthenticate, AuthenticationService>();
            //services.AddTransient<AppUser, AuthenticationService>();
            services.AddTransient<ICustomerDetails, CustomerService>();
            services.AddTransient<ITokenService, TokenService>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<ICarTestDetails, CarTestService>();

            return services;
        }
    }
}
