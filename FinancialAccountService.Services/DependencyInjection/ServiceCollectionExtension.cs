using FinancialAccountService.Processes.DependencyInjection;
using FinancialAccountService.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace FinancialAccountService.Services.DependencyInjection
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddServicesMappings(this IServiceCollection services,
                                                                  IConfiguration configuration,
                                                                  ILoggerFactory loggerFactory)
        {
            services.AddProcessesMappings(configuration, loggerFactory);

            services.AddTransient<IRegisterService, RegisterService>();
            services.AddTransient<IForgotPasswordService, ForgotPasswordService>();
            services.AddTransient<ICaseflowHeartbeatService, CaseflowHeartbeatService>();
            services.AddTransient<IMyAccountsService, MyAccountsService>();
            services.AddTransient<IUsersService, UsersService>();
            services.AddTransient<IWebActivityService, WebActivityService>();
            services.AddTransient<ICallbackService, CallbackService>();

            return services;
        }
    }
}