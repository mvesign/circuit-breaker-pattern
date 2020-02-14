using Bogus;
using CircuitBreakerPattern.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace CircuitBreakerPattern.ExternalApi.Controllers
{
    /// <summary>
    /// Data controller of the external API.
    /// </summary>
    [ApiController]
    [Route("external-api")]
    public class DataController : ControllerBase
    {
        private readonly int[] _faultyHttpStatusCodes;
        private readonly Faker _faker = new Faker();

        /// <summary>
        /// Default ctor.
        /// </summary>
        /// <param name="faultyHttpStatusCodes">HTTP status code in the 4xx or 5xx range.</param>
        public DataController(int[] faultyHttpStatusCodes)
        {
            _faultyHttpStatusCodes = faultyHttpStatusCodes;
        }

        /// <summary>
        /// Get ok HTTP status code.
        /// </summary>
        /// <returns>Response.</returns>
        [HttpGet("ok")]
        [Produces("application/json")]
        public IActionResult GetOk()
        {
            var numberOfModels = _faker.Random.Number(1, 10);

            IEnumerable<ApiData> GetDataModels()
            {
                for (var i = 0; i < numberOfModels; ++i)
                    yield return new ApiData
                    {
                        Id = i + 1,
                        Name = _faker.Random.Word(),
                        Value = _faker.Random.Word()
                    };
            }

            return Ok(
                GetDataModels()
            );
        }

        /// <summary>
        /// Get a random, non-circuit breaker HTTP status code, which is present in the 4xx range.
        /// </summary>
        /// <returns>Response.</returns>
        [HttpGet("other-error")]
        [Produces("application/json")]
        public IActionResult GetOtherError()
        {
            var statusCodes = _faultyHttpStatusCodes.Where(code => code < 500).ToArray();

            return StatusCode(
                statusCodes[_faker.Random.Number(0, statusCodes.Length - 1)]
            );
        }

        /// <summary>
        /// Get a random server error HTTP status code, which is present in the 4xx or 5xx range.
        /// </summary>
        /// <param name="content">Content of the request.</param>
        /// <returns>Response.</returns>
        [HttpPost("post")]
        [Produces("application/json")]
        public IActionResult PostError([FromBody] string content)
        {
            return StatusCode(
                _faultyHttpStatusCodes[_faker.Random.Number(0, _faultyHttpStatusCodes.Length - 1)]
            );
        }

        /// <summary>
        /// Get a random server error HTTP status code, which is present in the 5xx range
        /// </summary>
        /// <returns>Response.</returns>
        [HttpGet("server-error")]
        [Produces("application/json")]
        public IActionResult GetServerError()
        {
            var statusCodes = _faultyHttpStatusCodes.Where(code => code >= 500).ToArray();

            return StatusCode(
                statusCodes[_faker.Random.Number(0, statusCodes.Length - 1)]
            );
        }

        /// <summary>
        /// Get time-out HTTP status code.
        /// </summary>
        /// <returns>Response.</returns>
        [HttpGet("time-out")]
        [Produces("application/json")]
        public IActionResult GetTimeOut()
        {
            // Don't wait to simulate a time-out, just return the corresponding HTTP status code instead.
            return StatusCode((int)HttpStatusCode.RequestTimeout);
        }

        /// <summary>
        /// Get too many requests HTTP status code.
        /// </summary>
        /// <returns>Response.</returns>
        [HttpGet("too-many-requests")]
        [Produces("application/json")]
        public IActionResult GetTooManyRequests()
        {
            return StatusCode((int)HttpStatusCode.TooManyRequests);
        }
    }
}