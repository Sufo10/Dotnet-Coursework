using System;
using System.Text;
using Coursework.Application.Common.Interface;
using Coursework.Infrastructure.DI;
using Coursework.Infrastructure.Persistent;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.FileProviders;
using System.Text.Json;
using Coursework.Application.DTO;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllers();
// Learn more about configuring Swagger at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Student Teacher API",
        Version = "v1",
        Description = "Student Teacher API Services.",
        Contact = new OpenApiContact
        {
            Name = "Ajide Habeeb."
        },
    });
    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
            });
});
builder.Services.AddAuthentication();

//builder.Services.AddCors(options => options.AddPolicy("SubdomainDefault", builder => builder
//     .WithOrigins("https://localhost:5001")
//     .AllowCredentials()
//     .AllowAnyHeader()
//     .Build()
//));

builder.Services.AddCors(options =>
{
    options.AddPolicy("corsPolicy",
        policy =>
        {
            policy.WithOrigins("https://localhost:7190/",
                "http://localhost:5001/")
            .AllowCredentials()
            .AllowAnyHeader()
            .Build();
        });
});



builder.Services.ConfigureApplicationCookie(options =>
{
    options.Events.OnRedirectToAccessDenied = context =>
    {
        var errorMessage = new ErrorMessageResponse { Message = "Forbidden" };
        context.Response.StatusCode = 403;
        context.Response.ContentType = "application/json";
        return context.Response.WriteAsync(JsonSerializer.Serialize(errorMessage));
    };
    options.Events.OnRedirectToLogin = context =>
    {
        var errorMessage = new ErrorMessageResponse { Message = "Unauthorized" };
        context.Response.StatusCode = 401;
        context.Response.ContentType = "application/json";
        return context.Response.WriteAsync(JsonSerializer.Serialize(errorMessage));
    };
});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 1_500_000; // 1.5 MB limit
});
//var serviceProvider = builder.Services.BuildServiceProvider();
//try
//{
//    var dbContext = serviceProvider.GetRequiredService<ApplicationDBContext>();
//    dbContext.Database.Migrate();
//}
//catch
//{
//}

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//app.UseSession();
//app.UseCors(myAllowSpecificOrigins);

app.UseCors("corsPolicy");
app.UseHttpsRedirection();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(app.Environment.ContentRootPath, "images")),
    RequestPath = "/images"
});

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
