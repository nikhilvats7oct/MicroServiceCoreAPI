using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace FinancialAccountService.Proxy.HttpHandlers
{
    public class TracingHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TracingHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private static string TraceHeaderName => "X-Trace-Id";

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (_httpContextAccessor?.HttpContext?.Request?.Headers != null &&
                _httpContextAccessor.HttpContext.Request.Headers.ContainsKey(TraceHeaderName))
            {
                request.Headers.Add(TraceHeaderName,
                    _httpContextAccessor.HttpContext.Request.Headers[TraceHeaderName].ToString());
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
