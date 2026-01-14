
using backend.Auth;
using backend.Contexts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Configuration;
using System.Text;
using System.Text.Json.Serialization;

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
            builder.Services.AddDbContext<Context>(builder => builder.UseMySQL(connStr));
            builder.Services.AddSingleton<AuthManager>();

            builder.Services.AddControllers()
                .AddJsonOptions(options => {
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
                });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("JWTBearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "JWT Token for authentication",
                    Name = "Authentication",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "bearer"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = "JWTBearer",
                                Type = ReferenceType.SecurityScheme,
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

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
                     });

                builder.Services.AddAuthorization(options =>
                {
                    options.AddPolicy("role", policy => policy.RequireClaim("role"));
                });
            }

            var app = builder.Build();

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
