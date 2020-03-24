using Microsoft.AspNetCore.Builder;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinancialAccountService.WebApi.Middleware
{
    public static class TraceMiddleware
    {
        public const string TraceHeaderName = "X-Trace-Id";

        public static void AddTracing(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                string traceId = null;

                if (context.Request.Headers.ContainsKey(TraceHeaderName))
                {
                    traceId = context.Request.Headers[TraceHeaderName];
                }

                if (!string.IsNullOrWhiteSpace(traceId))
                {
                    context.TraceIdentifier = traceId;
                }

                // Add Trace ID to response.
                context.Response.OnStarting(() =>
                {
                    if (!context.Response.Headers.ContainsKey(TraceHeaderName))
                    {
                        context.Response.Headers.Add(TraceHeaderName, traceId);
                    }

                    return Task.CompletedTask;
                });

                using (LogContext.PushProperty("OccurrenceId", traceId))
                {
                    await next.Invoke();
                }
            });
        }
    }
}
