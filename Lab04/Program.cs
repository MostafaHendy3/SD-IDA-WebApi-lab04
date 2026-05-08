using System.Text;
using Lab04.Filters;
using Lab04.Middleware;
using Lab04.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NLog;
using NLog.Web;

namespace Lab04
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<ExceptionFilter>();
                options.Filters.Add<ResultFilter>();
            });
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.AddDbContext<Models.UniDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("Dev_Connection"))
            );
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<UniDbContext>()
                .AddDefaultTokenProviders();

            var jwtSection = builder.Configuration.GetSection("Jwt");
            var signingKey = jwtSection["Key"]
                ?? throw new InvalidOperationException("Jwt:Key is not configured.");
            var keyBytes = Encoding.UTF8.GetBytes(signingKey);

            builder.Services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSection["Issuer"],
                        ValidAudience = jwtSection["Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                        ClockSkew = TimeSpan.FromMinutes(1),
                        RoleClaimType =
                            System.Security.Claims.ClaimTypes.Role,
                    };
                });

            // Generic repository registration
            builder.Services.AddScoped(
                typeof(Repositories.IRepository<>),
                typeof(Repositories.GenericRepository<>)
            );
            builder.Services.AddScoped<Repositories.IUnitOfWork, Repositories.UnitOfWork>();
            builder.Services.AddScoped<
                Repositories.IRefreshTokenRepository,
                Repositories.RefreshTokenRepository
            >();

            builder.Services.AddCors(op =>
            {
                op.AddPolicy(
                    "AllowAll",
                    builder =>
                    {
                        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                    }
                );
            });

            // NLog: Register as Logging Provider
            var logger = NLog
                .LogManager.Setup()
                .LoadConfigurationFromAppSettings()
                .GetCurrentClassLogger();
            logger.Debug("init main");
            builder.Logging.ClearProviders();
            builder.Host.UseNLog();

            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            await SeedRolesAsync(app.Services);

            app.UseMiddleware<ExceptionMiddleware>();
            app.UseMiddleware<LoggingMiddleware>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.MapGet("/", () => "Hello World!");

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors();
            app.MapControllers();

            app.Run();
        }

        private static async Task SeedRolesAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            foreach (var roleName in new[] { AppRoles.Student, AppRoles.Admin })
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }
    }
}
