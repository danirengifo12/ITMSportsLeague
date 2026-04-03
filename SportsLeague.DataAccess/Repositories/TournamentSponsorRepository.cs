namespace SportsLeague.DataAccess.Repositories
{
    using Microsoft.EntityFrameworkCore;
    using SportsLeague.DataAccess.Context;
    using SportsLeague.Domain.Entities;
    using SportsLeague.Domain.Interfaces.Repositories;

    public class TournamentSponsorRepository : GenericRepository<TournamentSponsor>, ITournamentSponsorRepository
    {
        public TournamentSponsorRepository(LeagueDbContext context) : base(context) { }

        public async Task<IEnumerable<TournamentSponsor>> GetBySponsorAsync(int sponsorId)
        {
            return await _dbSet
                .Where(ts => ts.SponsorId == sponsorId)
                .Include(ts => ts.Tournament)
                .ToListAsync();
        }

        public async Task<TournamentSponsor?> GetByTournamentAndSponsorAsync(int tournamentId, int sponsorId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(ts => ts.TournamentId == tournamentId && ts.SponsorId == sponsorId);
        }
    }
}
