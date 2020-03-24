using FinancialAccountService.Processes.Interfaces;
using FinancialAccountService.Proxy.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace FinancialAccountService.Processes.DependencyInjection
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddProcessesMappings(this IServiceCollection services,
                                                                    IConfiguration configuration,
                                                                    ILoggerFactory loggerFactory)
        {
            

            services.AddTransient<ICheckDataProtectionProcess, CheckDataProtectionProcess>();
            services.AddTransient<ICheckIsWebRegisteredProcess, CheckIsWebRegisteredProcess>();
            services.AddTransient<ICompleteRegistrationProcess, CompleteRegistrationProcess>();
            services.AddTransient<ISendRegistrationEmailProcess, SendRegistrationEmailProcess>();
            services.AddTransient<ISendForgotPasswordEmailProcess, SendForgotPasswordEmailProcess>();
            services.AddTransient<IMyAccountsGetProcess, MyAccountsGetProcess>();
            services.AddTransient<IMyAccountsDetailGetProcess, MyAccountsDetailGetProcess>();
            services.AddTransient<IMyAccountsStatusDeriver, MyAccountsStatusDeriver>();
            services.AddTransient<IMyAccountDetailGetTransactionsRecentProcess, MyAccountDetailGetTransactionsRecentProcess>();
            services.AddTransient<IGetUserProcess, GetUserProcess>();
            services.AddTransient<ILogWebActivityProcess, LogWebActivityProcess>();
            services.AddTransient<ICompleteDeregistrationProcess, CompleteDeregistrationProcess>();
            services.AddTransient<ICaseflowHeartbeatProcess, CaseflowHeartbeatProcess>();

            services.AddProxyMappings(configuration, loggerFactory);

            return services;
        }
    }
}