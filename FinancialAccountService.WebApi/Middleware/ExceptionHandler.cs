using Microsoft.AspNetCore.Http;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using FinancialAccountService.Models.Exceptions;
using Serilog;

namespace FinancialAccountService.WebApi.Middleware
{
    public class ExceptionHandler
    {
        private readonly RequestDelegate _next;

        public ExceptionHandler(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            HttpStatusCode code = HttpStatusCode.InternalServerError;
            string uniqueExceptionOccurrenceIdContent = Guid.NewGuid().ToString();     // new unique exception instance id

            switch (exception)
            {
                case ProxyException proxy:
                    // TODO: should we propagate up the status code?
                    // code = proxy.StatusCode;

                    // Propagate up id that can be used to link together exceptions on different nodes
                    if (proxy.ExceptionOccurrenceId.HasValue)
                        uniqueExceptionOccurrenceIdContent = proxy.ExceptionOccurrenceId.ToString();

                    break;
            }

            bool parsed = Guid.TryParse(uniqueExceptionOccurrenceIdContent, out Guid uniqueExceptionOccurrenceId);
            Debug.Assert(parsed);

            Log.Logger.Error(exception, "Exception with ExceptionOccurrenceId: {@OccurrenceId}", uniqueExceptionOccurrenceId);

            string result = uniqueExceptionOccurrenceIdContent;
            context.Response.ContentType = "text/plain";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }
    }
}
