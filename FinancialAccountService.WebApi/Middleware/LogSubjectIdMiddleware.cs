using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Serilog.Context;

namespace FinancialAccountService.WebApi.Middleware
{
    public static class LogSubjectIdMiddleware
    {
        public static void AddSubjectIdToLog(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                if (context.User.Identity.IsAuthenticated)
                {
                    var subjectId = context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

                    if (string.IsNullOrWhiteSpace(subjectId))
                    {
                        subjectId = context.User.Claims.FirstOrDefault(x => x.Type == "sub")?.Value;
                    }

                    if (string.IsNullOrWhiteSpace(subjectId))
                    {
                        subjectId = context.User.Claims.FirstOrDefault(x => x.Type == "email")?.Value ?? "UNKNOWN";
                    }

                    using (LogContext.PushProperty("SubjectId", subjectId))
                    {
                        await next.Invoke();
                    }
                }
                else
                {
                    await next.Invoke();
                }
            });
        }
    }
}
