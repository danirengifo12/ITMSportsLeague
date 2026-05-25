using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SportsLeague.API.DTOs.Request;
using SportsLeague.API.DTOs.Response;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.API.Controllers;
 
[ApiController]
[Route("api/match/{matchId}/lineup")]
public class MatchLineupController : ControllerBase
{
    private readonly IMatchLineupService _matchLineupService;
    private readonly IMapper _mapper;

    public MatchLineupController(
        IMatchLineupService matchLineupService,
        IMapper mapper)
    {
        _matchLineupService = matchLineupService;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        int matchId,
        [FromBody] CreateMatchLineupDto dto)
    {
        try
        {
            var lineup = _mapper.Map<MatchLineup>(dto);

            lineup.MatchId = matchId;

            var created = await _matchLineupService.CreateAsync(matchId,lineup);

            var response = _mapper.Map<MatchLineupDto>(created);

            return Created(
                $"/api/match/{matchId}/lineup/{created.Id}",
                response);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                message = ex.Message
            });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetByMatch(int matchId)
    {
        var lineups = await _matchLineupService
            .GetByMatchAsync(matchId);

        var response = _mapper.Map<List<MatchLineupDto>>(lineups);

        return Ok(response);
    }

    [HttpGet("team/{teamId}")]
    public async Task<IActionResult> GetByMatchAndTeam(
        int matchId,
        int teamId)
    {
        var lineups = await _matchLineupService
            .GetByMatchAndTeamAsync(matchId, teamId);

        var response = _mapper.Map<List<MatchLineupDto>>(lineups);

        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(
        int matchId,
        int id)
    {
        try
        {
            await _matchLineupService.DeleteAsync(id);

            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
    }
}
