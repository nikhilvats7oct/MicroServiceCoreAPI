using System.Diagnostics.CodeAnalysis;
using FinancialAccountService.Services.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FinancialAccountService.WebApi.DependencyInjection
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddWebApiMappings(this IServiceCollection services,
                                                                IConfiguration configuration,
                                                                ILoggerFactory loggerFactory)
        {
            services.AddServicesMappings(configuration, loggerFactory);

            return services;
        }
    }
}