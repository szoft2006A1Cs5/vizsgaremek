
using backend.Services;
using backend.Contexts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using backend.Services.EmailService;
using backend.Services.RentalService;
using backend.Services.ResourceService;
using Resend;

namespace backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            var connStr = builder.Configuration.GetConnectionString("comove");
            if (connStr == null)
            {
                Console.WriteLine("Nem található connection string az adatbázis kapcsolathoz!");
                return;
            }
            
            // Add services to the container.
            builder.Services.AddDbContext<Context>(optionsBuilder => optionsBuilder.UseMySQL(connStr));
            builder.Services.AddSingleton(TimeProvider.System);
            builder.Services.AddScoped<AuthService>();
            builder.Services.AddScoped<RentalService>();
            builder.Services.AddSingleton<IResourceService, LocalResourceService>();

            if (builder.Configuration["Auth:Mail:Resend:Token"] is string resendToken)
            {
                builder.Services.AddOptions();
                builder.Services.AddHttpClient<ResendClient>();
                builder.Services.Configure<ResendClientOptions>(o =>
                {
                    o.ApiToken = resendToken;
                });
                builder.Services.AddTransient<IResend, ResendClient>();

                builder.Services.AddScoped<IEmailService, ResendEmailService>();
            }

            builder.Services.AddControllers()
                .AddJsonOptions(options => {
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                    options.JsonSerializerOptions.Converters.Add(
                        new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                    );
                });
            
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Kulon scope, hogy a titkos adatok azonnal droppoljanak
            {
                var key = builder.Configuration["Auth:Jwt:Secret"];
                var iss = builder.Configuration["Auth:Issuer"];
                var aud = builder.Configuration["Auth:Audience"];

                if (key == null || iss == null || aud == null)
                {
                    Console.WriteLine("Hiányos az azonosítási konfiguráció!");
                    return;
                }
                
                builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                     {
                         options.TokenValidationParameters = new TokenValidationParameters
                         {
                             ValidateIssuer = true,
                             ValidateAudience = true,
                             ValidateLifetime = true,
                             ValidateIssuerSigningKey = true,
                             ValidIssuer = iss,
                             ValidAudience = aud,
                             IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
                         };

                         options.Events = new JwtBearerEvents
                         {
                             OnMessageReceived = context =>
                             {
                                 if (context.Request.Cookies.TryGetValue("auth", out var token))
                                     context.Token = token;
                                 
                                 return Task.CompletedTask;
                             }
                         };
                     });
            }

            // CORS
            var allowedOrigins = builder.Configuration
                .GetSection("CORS")
                .GetChildren()
                .Where(x => !string.IsNullOrWhiteSpace(x.Value))
                .Select(x => x.Value!)
                .ToArray();
            
            var app = builder.Build();
            
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            
            app.UseHttpsRedirection();
            
            // Csak ha localresourceservice-t hasznalunk
            app.UseStaticFiles(new StaticFileOptions
            {
                RequestPath = "/res",
            });

            app.UseCors(policy =>
            {
                policy
                    .WithOrigins(allowedOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            
            app.Run();
        }
    }
}
