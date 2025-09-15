using IceAndStone.API.Requests;
using IceAndStone.API.Responses;
using IceAndStone.API.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace IceAndStone.API.Controllers
{
    [ApiController]
    [Route("api/sessions")]
    public class SessionsController : ControllerBase
    {
        private readonly ISessionService _service;
        public SessionsController(ISessionService service) => _service = service;

        /// <summary>Starts a new session on a specific lane.</summary>
        [HttpPost("start")]
        public async Task<ActionResult<SessionResponse>> Start([FromBody] StartSessionRequest request, CancellationToken cancellationToken)
            => Ok(await _service.StartAsync(request, cancellationToken));

        /// <summary>Ends the current session and closes any active games.</summary>
        [HttpPost("end")]
        public async Task<ActionResult<SessionResponse>> End([FromBody] EndSessionRequest request, CancellationToken cancellationToken)
            => Ok(await _service.EndAsync(request, cancellationToken));
    }
}
