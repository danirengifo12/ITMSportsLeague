using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SportsLeague.API.DTOs.Request;
using SportsLeague.API.DTOs.Response;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SponsorController : ControllerBase
    {
        private readonly ISponsorService _service;
        private readonly IMapper _mapper;

        public SponsorController(ISponsorService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _service.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<SponsorResponseDTO>>(data));
        }

      
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var sponsor = await _service.GetByIdAsync(id);
            if (sponsor == null)
                return NotFound();

            return Ok(_mapper.Map<SponsorResponseDTO>(sponsor));
        }

       
        [HttpPost]
        public async Task<IActionResult> Create(SponsorRequestDTO dto)
        {
            try
            {
                var entity = _mapper.Map<Sponsor>(dto);
                var result = await _service.CreateAsync(entity);

                return CreatedAtAction(nameof(Get),
                    new { id = result.Id },
                    _mapper.Map<SponsorResponseDTO>(result));
            }
            catch (InvalidOperationException e)
            {
                return Conflict(e.Message);
            }
        }

       
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, SponsorRequestDTO dto)
        {
            try
            {
                await _service.UpdateAsync(id, _mapper.Map<Sponsor>(dto));
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException e)
            {
                return Conflict(e.Message);
            }
        }

        // ✅ DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost("{id}/tournaments")]
        public async Task<IActionResult> Register(int id, TournamentSponsorRequestDTO dto)
        {
            try
            {
                await _service.RegisterToTournamentAsync(id, dto.TournamentId, dto.ContractAmount);

                return StatusCode(201, new
                {
                    sponsorId = id,
                    tournamentId = dto.TournamentId,
                    contractAmount = dto.ContractAmount
                });
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (InvalidOperationException e)
            {
                return Conflict(e.Message);
            }
        }

        [HttpGet("{id}/tournaments")]
        public async Task<IActionResult> GetTournaments(int id)
        {
            try
            {
                var data = await _service.GetTournamentsAsync(id);

                return Ok(_mapper.Map<IEnumerable<TournamentSponsorResponseDTO>>(data));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        
        [HttpDelete("{id}/tournaments/{tid}")]
        public async Task<IActionResult> Remove(int id, int tid)
        {
            try
            {
                await _service.RemoveFromTournamentAsync(id, tid);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
