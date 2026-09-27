using AngleSharp.Html.Parser;
using FluentValidation;
using System.Reflection;
using Npgsql;
using TestAssignment.Interfaces;
using TestAssignment.Services;

namespace TestAssignment
{
    public static class Startup
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("DefaultConnection")
                    ?? throw new InvalidOperationException("Строка подключения не найдена");

            services.AddNpgsqlDataSource(connectionString);

            services.AddSingleton<IHtmlParser, HtmlParser>();

            services.AddSingleton<IScrapingService, ScrapingService>();

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}
