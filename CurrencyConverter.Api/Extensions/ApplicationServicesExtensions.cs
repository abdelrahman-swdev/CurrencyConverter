using CurrencyConverter.Core.Interfaces;
using CurrencyConverter.Infrastructure.Data.Repositories;
using CurrencyConverter.Service.Interfaces;
using CurrencyConverter.Service.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CurrencyConverter.Api.Extensions
{
    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<ICurrencyRepository, CurrencyRepository>();
            services.AddScoped<IExchangeHistoryRepository, ExchangeHistoryRepository>();
            services.AddScoped<ICurrencyService, CurrencyService>();
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }
    }
}
