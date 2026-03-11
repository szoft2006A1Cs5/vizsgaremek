
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
using System.Text.Json.Serialization;
using backend.Services.ResourceService;

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
            builder.Services.AddSingleton<AuthService>();
            builder.Services.AddScoped<RentalService>();
            builder.Services.AddSingleton<IResourceService, LocalResourceService>();

            builder.Services.AddControllers()
                .AddJsonOptions(options => {
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
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
                    options.AddPolicy("OnlyUser", policy => policy.RequireClaim(ClaimTypes.Role, "User"));
                    options.AddPolicy("OnlyAdmin", policy => policy.RequireClaim(ClaimTypes.Role, "Administrator"));
                    options.AddPolicy("AllowAll", policy => policy.RequireClaim(ClaimTypes.Role, "Administrator", "User"));
                });
            }

            /*
            builder.Services.AddCors(o =>
            {
                o.AddPolicy("AllowFrontend", x =>
                {
                    x
                        .WithOrigins("https://localhost:5173")
                        .WithOrigins("https://127.0.0.1:5173")
                        .WithOrigins("http://localhost:5173")
                        .WithOrigins("http://127.0.0.1:5173");
                });
            });
            */

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

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.UseCors(policy => 
                policy
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
            );
            
            app.Run();
        }
    }
}
