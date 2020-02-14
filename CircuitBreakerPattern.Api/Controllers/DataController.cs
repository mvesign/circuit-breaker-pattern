using CircuitBreakerPattern.Api.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CircuitBreakerPattern.Api.Controllers
{
    /// <summary>
    /// Data controller of the API.
    /// </summary>
    [ApiController]
    [Route("api")]
    public class DataController : ControllerBase
    {
        private readonly ExternalApiService _externalApiService;

        /// <summary>
        /// Default ctor.
        /// </summary>
        /// <param name="externalApiService">Service to connect with the external API.</param>
        public DataController(ExternalApiService externalApiService)
        {
            _externalApiService = externalApiService;
        }

        /// <summary>
        /// Get data models.
        /// </summary>
        /// <returns>Data models.</returns>
        [HttpGet("ok")]
        [Produces("application/json")]
        public async Task<IActionResult> TriggerOkAsync()
        {
            return Ok(
                await _externalApiService.SendRequestToExternalApiAsync("ok").ConfigureAwait(false)
            );
        }

        /// <summary>
        /// Get data models.
        /// </summary>
        /// <returns>Data models.</returns>
        [HttpGet("other-error")]
        [Produces("application/json")]
        public async Task<IActionResult> TriggerOtherErrorAsync()
        {
            return Ok(
                await _externalApiService.SendRequestToExternalApiAsync("other-error").ConfigureAwait(false)
            );
        }

        /// <summary>
        /// Get data models.
        /// </summary>
        /// <returns>Data models.</returns>
        [HttpPost("post")]
        [Produces("application/json")]
        public async Task<IActionResult> TriggerPostAsync([FromBody] string content)
        {
            return Ok(
                await _externalApiService.SendRequestToExternalApiAsync("post", content).ConfigureAwait(false)
            );
        }

        /// <summary>
        /// Get data models.
        /// </summary>
        /// <returns>Data models.</returns>
        [HttpGet("server-error")]
        [Produces("application/json")]
        public async Task<IActionResult> TriggerServerErrorAsync()
        {
            return Ok(
                await _externalApiService.SendRequestToExternalApiAsync("server-error").ConfigureAwait(false)
            );
        }

        /// <summary>
        /// Get data models.
        /// </summary>
        /// <returns>Data models.</returns>
        [HttpGet("time-out")]
        [Produces("application/json")]
        public async Task<IActionResult> TriggerTimeOutAsync()
        {
            return Ok(
                await _externalApiService.SendRequestToExternalApiAsync("time-out").ConfigureAwait(false)
            );
        }

        /// <summary>
        /// Get data models.
        /// </summary>
        /// <returns>Data models.</returns>
        [HttpGet("too-many-requests")]
        [Produces("application/json")]
        public async Task<IActionResult> TriggerTooManyRequestsAsync()
        {
            return Ok(
                await _externalApiService.SendRequestToExternalApiAsync("too-many-requests").ConfigureAwait(false)
            );
        }
    }
}