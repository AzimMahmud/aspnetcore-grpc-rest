using aspnetapp.Services;
using Greet.V1;
using Microsoft.AspNetCore.Mvc;

namespace aspnetapp.Controllers
{
    /// <summary>
    /// Hello controller
    /// </summary>
    [ApiVersion("1.0")]
    [ApiController]
    [Route("v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    public class HelloController : Controller
    {
        private readonly ILogger<HelloController> _logger;
        private readonly IGreeterService _service;

        /// <summary>
        /// Hello constructor
        /// </summary>
        public HelloController(ILogger<HelloController> logger, IGreeterService service)
        {
            _logger = logger;
            _service = service;
        }

        /// <summary>
        /// Sends a hello message and receives a hello reply
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /hello/{name}
        ///
        /// </remarks>
        /// <param name="name"></param>
        [HttpGet("{name}", Name = "SayHello")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(HelloReply))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SayHello(string name)
        {
            if (string.IsNullOrEmpty(name))
                return BadRequest();

            // Forward the call to the greeter service
            var response = await _service.SayHello(new HelloRequest { Name = name }, null);
            return Ok(response);
        }
    }
}
