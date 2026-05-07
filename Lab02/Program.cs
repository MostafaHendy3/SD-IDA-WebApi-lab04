
using Lab02.Filters;
using Lab02.Middleware;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;

namespace Lab02
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers(
                options => {
                    options.Filters.Add<ExceptionFilter>();
                    options.Filters.Add<ResultFilter>();
                }
            );
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.AddDbContext<Models.UniDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("Dev_Connection")));

            // NLog: Register as Logging Provider
            var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
            logger.Debug("init main");
            builder.Logging.ClearProviders();
            builder.Host.UseNLog();

            builder.Services.AddSwaggerGen();

            var app = builder.Build();

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

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
