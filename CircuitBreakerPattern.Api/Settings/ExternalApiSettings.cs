using System.Collections.Generic;

namespace CircuitBreakerPattern.Api.Settings
{
    public class ExternalApiSettings
    {
        public string BaseUrl { get; set; }
        public Dictionary<string, string> Headers { get; set; }
    }
}