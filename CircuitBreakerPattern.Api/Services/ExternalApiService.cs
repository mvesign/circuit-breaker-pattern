using CircuitBreakerPattern.Api.Settings;
using CircuitBreakerPattern.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CircuitBreakerPattern.Api.Services
{
    /// <summary>
    /// Service to connect with the external API.
    /// </summary>
    public class ExternalApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ExternalApiService> _logger;

        /// <summary>
        /// Default ctor.
        /// </summary>
        /// <param name="httpClient">Typed HTTP client.</param>
        /// <param name="externalApiSettings">Settings to connect with the external API.</param>
        /// <param name="logger">Logger.</param>
        public ExternalApiService(
            HttpClient httpClient,
            IOptionsMonitor<ExternalApiSettings> externalApiSettings,
            ILogger<ExternalApiService> logger
        )
        {
            _logger = logger;

            httpClient.BaseAddress = new Uri(externalApiSettings.CurrentValue.BaseUrl);
            if (externalApiSettings.CurrentValue.Headers?.Any() == true)
                foreach (var (key, value) in externalApiSettings.CurrentValue.Headers)
                    httpClient.DefaultRequestHeaders.Add(key, value);

            _httpClient = httpClient;
        }

        /// <summary>
        /// Send a request to the external API.
        /// </summary>
        /// <param name="urlPostfix">Post-fix value of the URL.</param>
        /// <param name="content">Content.</param>
        /// <returns>List of data models.</returns>
        public async Task<IEnumerable<ApiData>> SendRequestToExternalApiAsync(string urlPostfix, string content = null)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var response = string.IsNullOrEmpty(content)
                ? await _httpClient.GetAsync("/external-api/" + urlPostfix).ConfigureAwait(false)
                : await _httpClient.PostAsync("/external-api/" + urlPostfix, new StringContent(content)).ConfigureAwait(false);

            stopwatch.Stop();

            _logger.LogInformation($"Perform '{urlPostfix}' call in {stopwatch.Elapsed}");

            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<IEnumerable<ApiData>>(
                await response.Content.ReadAsStringAsync().ConfigureAwait(false)
            );
        }
    }
}