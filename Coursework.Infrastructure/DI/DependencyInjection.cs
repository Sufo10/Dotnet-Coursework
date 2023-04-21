using System;
using Coursework.Application.Common.Interface;
using Coursework.Infrastructure.Persistent;
using Coursework.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Coursework.Infrastructure.DI
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            //services.AddCors
            //services.AddCors(options =>
            //{
            //    options.AddPolicy("AllowAllOrigins",
            //        builder =>
            //        {
            //            builder.AllowAnyOrigin()
            //                .AllowAnyHeader()
            //                .AllowAnyMethod();
            //        });
            //});

            services.AddDbContext<ApplicationDBContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("CAPostgreSQL"),
                b => b.MigrationsAssembly(typeof(ApplicationDBContext).Assembly.FullName)), ServiceLifetime.Transient);

            services.AddIdentityCore<IdentityUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890@.";
            }).AddRoles<IdentityRole>().AddEntityFrameworkStores<ApplicationDBContext>().AddSignInManager<SignInManager<IdentityUser>>();

            services.AddScoped<IApplicationDBContext>(provider => provider.GetService<ApplicationDBContext>());
            services.AddTransient<IDateTime, DateTimeService>();
            services.AddTransient<IFileStorage,ServerFileStorage>();
            services.AddTransient<ICarDetails, CarService>();
            services.AddTransient<IAuthenticate, AuthenticationService>();
            services.AddTransient<ICustomerDetails, CustomerService>();
            services.AddTransient<ITokenService, TokenService>();
            return services;
        }
    }
}
