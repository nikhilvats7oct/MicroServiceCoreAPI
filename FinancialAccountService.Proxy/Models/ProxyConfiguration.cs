using Microsoft.Extensions.Configuration;
using System;

namespace FinancialAccountService.Proxy.Models
{
    public class ProxyConfiguration
    {
        public string CaseflowApiProxyUrl { get; set; }
        public string AuthorizationServerTokenIssuerUri { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public bool UseTokenFromRequest { get; set; }
    }
}