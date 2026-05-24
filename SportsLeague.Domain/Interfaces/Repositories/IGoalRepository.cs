using SportsLeague.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsLeague.Domain.Interfaces.Repositories;

public interface IGoalRepository : IGenericRepository<Goal>

{

    Task<IEnumerable<Goal>> GetByMatchAsync(int matchId);

    Task<IEnumerable<Goal>> GetByMatchWithDetailsAsync(int matchId);

}
