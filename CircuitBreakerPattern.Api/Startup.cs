using CircuitBreakerPattern.Api.Extensions;
using CircuitBreakerPattern.Api.Filters;
using CircuitBreakerPattern.Api.Services;
using CircuitBreakerPattern.Api.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Extensions.Http;
using System;
using System.Net;
using System.Net.Http;

namespace CircuitBreakerPattern.Api
{
    /// <summary>
    /// Start-up of the web API.
    /// </summary>
    public class Startup
    {
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
                .Configure<ExternalApiSettings>(_configuration.GetSection("ExternalApiSettings"))
                .AddScoped<ExternalApiService>()
                .AddControllers(options =>
                {
                    options.Filters.Add<ExceptionFilter>();
                });

            // Don't forget to configure the typed HTTP clients.
            ConfigureHttpClients(services);
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

        /// <summary>
        /// Configure typed HTTP clients with policies for when a HTTP request exception, status code 5xx, 408 or 429 is returned by the external API.
        /// </summary>
        /// <param name="services">Collection of services for which the typed HTTP clients are available.</param>
        private static void ConfigureHttpClients(IServiceCollection services)
        {
            var policyBuilder = HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(response => response.StatusCode == HttpStatusCode.TooManyRequests);

            // Retry policy with a simple jitter.
            var delay = Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), 5);
            var retryPolicy = policyBuilder.WaitAndRetryAsync(
                delay,
                (outcome, timespan, retryAttempt, context) =>
                {
                    context.GetLogger()?.LogWarning($"Received status code '{outcome.Result.StatusCode}', delaying for {timespan} before making retry {retryAttempt}");
                }
            );

            // Circuit-breaker policy.
            var breakerPolicy = policyBuilder.CircuitBreakerAsync(2, TimeSpan.FromSeconds(5));

            // Policy which passes delegates without intervention.
            var noOpPolicy = Policy.NoOpAsync().AsAsyncPolicy<HttpResponseMessage>();

            // And configure the actual typed HTTP client(s).
            // But only apply the retry and breaker policy when a GET request is send.
            services
                .AddHttpClient<ExternalApiService>()
                .AddPolicyHandler(request => request.Method == HttpMethod.Get ? retryPolicy : noOpPolicy)
                .AddPolicyHandler(request => request.Method == HttpMethod.Get ? breakerPolicy : noOpPolicy);



            //TODO: Make logger work dynamically
            //TODO: Implement fallback policy
            //TODO: Move this functionality to service
            // Check https://github.com/App-vNext/Polly/wiki/Polly-and-HttpClientFactory
        }
    }
}