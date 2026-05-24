using SportsLeague.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsLeague.Domain.Interfaces.Repositories;

public interface ICardRepository : IGenericRepository<Card>

{

    Task<IEnumerable<Card>> GetByMatchAsync(int matchId);

    Task<IEnumerable<Card>> GetByMatchWithDetailsAsync(int matchId);

}
