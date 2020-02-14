using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Net;

namespace CircuitBreakerPattern.ExternalApi
{
    /// <summary>
    /// Start-up of the web API.
    /// </summary>
    public class Startup
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Default ctor.
        /// </summary>
        /// <param name="configuration">Configuration settings.</param>
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Configure web API services.
        /// </summary>
        /// <param name="services">Collection of services.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddSingleton(
                    ((HttpStatusCode[])Enum.GetValues(typeof(HttpStatusCode)))
                        .Where(code => (int)code >= 400)
                        .Select(code => (int)code)
                        .ToArray()
                )
                .AddControllers();
        }

        /// <summary>
        /// Configure the web API.
        /// </summary>
        /// <param name="application">Application builder.</param>
        /// <param name="environment">Host environment.</param>
        // ReSharper disable once UnusedMember.Global
        public void Configure(IApplicationBuilder application, IWebHostEnvironment environment)
        {
            if (environment.IsDevelopment())
                application.UseDeveloperExceptionPage();

            application.UseRouting();

            application.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}