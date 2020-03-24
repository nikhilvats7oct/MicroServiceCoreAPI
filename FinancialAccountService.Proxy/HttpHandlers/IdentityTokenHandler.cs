using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FinancialAccountService.Proxy.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FinancialAccountService.Proxy.HttpHandlers
{
    public class IdentityTokenHandler : DelegatingHandler
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<IdentityTokenHandler> _logger;
        private readonly ProxyConfiguration _proxyConfiguration;
        private readonly IMemoryCache _cache;

        private const string TokenKey = "FinancialAccountService_ClientCredentials_AccessToken";
        public IdentityTokenHandler(
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor,
            ILogger<IdentityTokenHandler> logger,
            ProxyConfiguration proxyConfiguration,
            IMemoryCache cache
            )
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _proxyConfiguration = proxyConfiguration;
            _cache = cache;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                var token = string.Empty;

                if (_proxyConfiguration.UseTokenFromRequest)
                {
                    if (_httpContextAccessor?.HttpContext?.User?.Identity != null &&
                        _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                    {
                        token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
                    }
                }

                else
                {
                    token = await GetAccessToken();

                    if (request.Headers.Contains("Authorization"))
                    {
                        request.Headers.Remove("Authorization");
                    }
                }

                request.Headers.Add("Authorization", $"Bearer {token}");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "An error has occurred while setting up auth header to the http client.");
                throw;
            }

            return await base.SendAsync(request, cancellationToken);
        }

        private async Task<string> GetAccessToken()
        {
            var token = _cache.Get<string>(TokenKey);

            if (!string.IsNullOrWhiteSpace(token))
            {
                return token;
            }

            using (var client = _httpClientFactory.CreateClient())
            {
                var authorizationServerTokenIssuerUri = new Uri(_proxyConfiguration.AuthorizationServerTokenIssuerUri);
                var clientId = _proxyConfiguration.ClientId;
                var clientSecret = _proxyConfiguration.ClientSecret;

                HttpResponseMessage responseMessage;

                var tokenRequest = new HttpRequestMessage(HttpMethod.Post, authorizationServerTokenIssuerUri);
                HttpContent httpContent = new FormUrlEncodedContent(
                    new[]
                    {
                        new KeyValuePair<string, string>("grant_type", "client_credentials"),
                        new KeyValuePair<string, string>("client_id", clientId),
                        new KeyValuePair<string, string>("client_secret", clientSecret)
                    });
                tokenRequest.Content = httpContent;
                responseMessage = await client.SendAsync(tokenRequest);

                var response = await responseMessage.Content.ReadAsStringAsync();
                var authorizationServerToken = JsonConvert.DeserializeObject<AuthorizationServerAnswer>(response);

                _cache.Set(TokenKey, authorizationServerToken.access_token, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(authorizationServerToken.expires_in)
                });

                return authorizationServerToken.access_token;
            }
        }

    }
}