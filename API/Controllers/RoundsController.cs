using IceAndStone.API.Requests;
using IceAndStone.API.Responses;
using IceAndStone.API.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace IceAndStone.API.Controllers
{
    [ApiController]
    [Route("api/rounds")]
    public class RoundsController : ControllerBase
    {
        private readonly IRoundService _service;
        public RoundsController(IRoundService service) => _service = service;

        [HttpPost("start")]
        public async Task<ActionResult<RoundResponse>> Start([FromBody] StartRoundRequest request, CancellationToken cancellationToken)
            => Ok(await _service.StartAsync(request, cancellationToken));

        [HttpPost("end")]
        public async Task<ActionResult<RoundResponse>> End([FromBody] EndRoundRequest request, CancellationToken cancellationToken)
            => Ok(await _service.EndAsync(request, cancellationToken));
    }
}
