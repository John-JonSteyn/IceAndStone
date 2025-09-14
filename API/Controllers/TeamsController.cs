using IceAndStone.API.Requests;
using IceAndStone.API.Responses;
using IceAndStone.API.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace IceAndStone.API.Controllers
{
    [ApiController]
    [Route("api/teams")]
    public class TeamsController : ControllerBase
    {
        private readonly ITeamService _service;
        public TeamsController(ITeamService service) => _service = service;

        [HttpPost("create-pair")]
        public async Task<ActionResult<(TeamResponse, TeamResponse)>> CreatePair([FromBody] CreateTeamsRequest request, CancellationToken cancellationToken)
            => Ok(await _service.CreatePairAsync(request, cancellationToken));

        [HttpPost("add-players")]
        public async Task<IActionResult> AddPlayers([FromBody] AddPlayersRequest request, CancellationToken cancellationToken)
        {
            await _service.AddPlayersAsync(request, cancellationToken);
            return NoContent();
        }
    }
}
