using FinancialAccountService.Proxy.Interfaces;
using FinancialAccountService.Proxy.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using FinancialAccountService.Proxy.HttpHandlers;

namespace FinancialAccountService.Proxy.DependencyInjection
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddProxyMappings(this IServiceCollection services,
                                                               IConfiguration configuration,
                                                               ILoggerFactory loggerFactory)
        {
            services.AddHttpClient();
            services.AddHttpContextAccessor();
            
            var proxySettings = new ProxyConfiguration();
            configuration.Bind("Proxy", proxySettings);
            services.AddSingleton(proxySettings);
                   
            services.AddTransient<IdentityTokenHandler>();
            services.AddTransient<TracingHandler>();

            services.AddHttpClient<ICaseflowApiProxy, CaseflowApiProxy>(client =>
                {
                    client.BaseAddress = new Uri(proxySettings.CaseflowApiProxyUrl);
                })
               .AddHttpMessageHandler<IdentityTokenHandler>()
               .AddHttpMessageHandler<TracingHandler>();

            services.AddMemoryCache();
            
            return services;
        }
    }
}