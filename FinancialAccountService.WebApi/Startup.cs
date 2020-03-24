using FinancialAccountService.Proxy;
using FinancialAccountService.Proxy.DependencyInjection;
using FinancialAccountService.Proxy.Interfaces;
using FinancialAccountService.Proxy.Models;
using FinancialAccountService.WebApi.DependencyInjection;
using FinancialAccountService.WebApi.Filters;
using FinancialAccountService.WebApi.Middleware;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Debugging;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinancialAccountService.WebApi
{
    public class Startup
    {
        private IServiceCollection _services;
        private readonly ILogger<Startup> _logger;
        private readonly ILoggerFactory _loggerFactory;

        public Startup(IConfiguration configuration, ILogger<Startup> logger, ILoggerFactory loggerFactory)
        {
            Configuration = configuration;
            _logger = logger;
            _loggerFactory = loggerFactory;

            Log.Logger = new LoggerConfiguration()
                .ReadFrom
                .Configuration(configuration)
                .CreateLogger();

#if DEBUG
            SelfLog.Enable(Console.Error);
#endif
        }
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            _services = services;
            _logger.LogInformation("Configuring Services");

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(o =>
                {
                    o.Authority = Configuration["OAuth:Authority"];
                    o.Audience = Configuration["OAuth:Audience"];
                    o.RequireHttpsMetadata = false; // For Testing
                });

            _services.AddWebApiMappings(Configuration, _loggerFactory);

            _services.AddMvc()
                     .AddFluentValidation()
                     .AddMvcOptions(options => options.Filters.Add(typeof(LoggingAsyncActionFilter)));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Financial Account Service", Description = "", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });
                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                {
                    {"Bearer", new string[] { }},
                });
            });

            services.AddCors();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime lifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                ListAllRegisteredServices(app);
                app.UseDatabaseErrorPage();
            }

            app.AddTracing();
            
            app.UseHsts();
            //app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthentication();

            app.AddSubjectIdToLog();

            app.UseMiddleware(typeof(ExceptionHandler));

            app.UseStaticFiles();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Financial Account Service");
            });

            app.UseCors();
            app.UseMvc();

            lifetime.ApplicationStarted.Register(OnStarted);
        }

        private void OnStarted()
        {
            _logger.LogInformation("Account Service Started...");
        }

        private void ListAllRegisteredServices(IApplicationBuilder app)
        {
            app.Map("/allservices", builder => builder.Run(async context =>
            {
                var sb = new StringBuilder();
                sb.Append("<h1>All Services</h1>");
                sb.Append("<table><thead>");
                sb.Append("<tr><th>Type</th><th>Lifetime</th><th>Instance</th></tr>");
                sb.Append("</thead><tbody>");
                foreach (var svc in _services)
                {
                    sb.Append("<tr>");
                    sb.Append($"<td>{svc.ServiceType.FullName}</td>");
                    sb.Append($"<td>{svc.Lifetime}</td>");
                    sb.Append($"<td>{svc.ImplementationType?.FullName}</td>");
                    sb.Append("</tr>");
                }
                sb.Append("</tbody></table>");
                await context.Response.WriteAsync(sb.ToString());
            }));
        }
    }
}