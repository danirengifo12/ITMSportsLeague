using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories;
using SportsLeague.Domain.Interfaces.Services;
using System.Text.RegularExpressions;

namespace SportsLeague.Domain.Services
{
    public class SponsorService : ISponsorService
    {
        private readonly ISponsorRepository _sponsorRepository;
        private readonly ITournamentRepository _tournamentRepository;
        private readonly ITournamentSponsorRepository _tsRepository;

        public SponsorService(
            ISponsorRepository sponsorRepository,
            ITournamentRepository tournamentRepository,
            ITournamentSponsorRepository tsRepository)
        {
            _sponsorRepository = sponsorRepository;
            _tournamentRepository = tournamentRepository;
            _tsRepository = tsRepository;
        }

        // ✅ GET ALL
        public async Task<IEnumerable<Sponsor>> GetAllAsync()
        {
            return await _sponsorRepository.GetAllAsync();
        }

        // ✅ GET BY ID
        public async Task<Sponsor?> GetByIdAsync(int id)
        {
            return await _sponsorRepository.GetByIdAsync(id);
        }

        // ✅ CREATE
        public async Task<Sponsor> CreateAsync(Sponsor sponsor)
        {
            if (await _sponsorRepository.ExistsByNameAsync(sponsor.Name))
                throw new InvalidOperationException("El nombre ya existe");

            if (!IsValidEmail(sponsor.ContactEmail))
                throw new InvalidOperationException("Email inválido");

            return await _sponsorRepository.CreateAsync(sponsor);
        }

        // ✅ UPDATE
        public async Task UpdateAsync(int id, Sponsor sponsor)
        {
            var existing = await _sponsorRepository.GetByIdAsync(id);

            if (existing == null)
                throw new KeyNotFoundException("Sponsor no encontrado");

            if (existing.Name != sponsor.Name &&
                await _sponsorRepository.ExistsByNameAsync(sponsor.Name))
                throw new InvalidOperationException("Nombre duplicado");

            if (!IsValidEmail(sponsor.ContactEmail))
                throw new InvalidOperationException("Email inválido");

            existing.Name = sponsor.Name;
            existing.ContactEmail = sponsor.ContactEmail;
            existing.Phone = sponsor.Phone;
            existing.WebsiteUrl = sponsor.WebsiteUrl;
            existing.Category = sponsor.Category;

            await _sponsorRepository.UpdateAsync(existing);
        }

        // ✅ DELETE
        public async Task DeleteAsync(int id)
        {
            var exists = await _sponsorRepository.ExistsAsync(id);

            if (!exists)
                throw new KeyNotFoundException("No existe");

            await _sponsorRepository.DeleteAsync(id);
        }

        // 🔥 REGISTER TO TOURNAMENT
        public async Task RegisterToTournamentAsync(int sponsorId, int tournamentId, decimal amount)
        {
            if (amount <= 0)
                throw new InvalidOperationException("Monto inválido");

            var sponsor = await _sponsorRepository.GetByIdAsync(sponsorId);
            if (sponsor == null)
                throw new KeyNotFoundException("Sponsor no existe");

            var tournament = await _tournamentRepository.GetByIdAsync(tournamentId);
            if (tournament == null)
                throw new KeyNotFoundException("Tournament no existe");

            var exists = await _tsRepository.GetByTournamentAndSponsorAsync(tournamentId, sponsorId);
            if (exists != null)
                throw new InvalidOperationException("Ya está vinculado");

            await _tsRepository.CreateAsync(new TournamentSponsor
            {
                SponsorId = sponsorId,
                TournamentId = tournamentId,
                ContractAmount = amount
            });
        }

        // 🔥 GET TOURNAMENTS (CORREGIDO)
        public async Task<IEnumerable<TournamentSponsor>> GetTournamentsAsync(int sponsorId)
        {
            var sponsor = await _sponsorRepository.GetByIdAsync(sponsorId);

            if (sponsor == null)
                throw new KeyNotFoundException("Sponsor no existe");

            return await _tsRepository.GetBySponsorAsync(sponsorId);
        }

        // 🔥 REMOVE RELATION
        public async Task RemoveFromTournamentAsync(int sponsorId, int tournamentId)
        {
            var relation = await _tsRepository
                .GetByTournamentAndSponsorAsync(tournamentId, sponsorId);

            if (relation == null)
                throw new KeyNotFoundException("No existe relación");

            await _tsRepository.DeleteAsync(relation.Id);
        }

        // 🔒 VALIDACIÓN EMAIL
        private bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }
    }
}