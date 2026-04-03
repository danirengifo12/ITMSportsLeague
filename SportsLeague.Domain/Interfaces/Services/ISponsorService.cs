using SportsLeague.Domain.Entities;

namespace SportsLeague.Domain.Interfaces.Services
{
    public interface ISponsorService
    {
        // CRUD
        Task<IEnumerable<Sponsor>> GetAllAsync();
        Task<Sponsor?> GetByIdAsync(int id);
        Task<Sponsor> CreateAsync(Sponsor sponsor);
        Task UpdateAsync(int id, Sponsor sponsor);
        Task DeleteAsync(int id);

        // Relación Sponsor - Tournament
        Task RegisterToTournamentAsync(int sponsorId, int tournamentId, decimal amount);
        Task<IEnumerable<TournamentSponsor>> GetTournamentsAsync(int sponsorId);
        Task RemoveFromTournamentAsync(int sponsorId, int tournamentId);
    }
}
