using DevOps.AppLogic;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevOps.Infrastructure
{
    internal class TeamDbRepository : ITeamRepository
    {
        private readonly DevOpsContext _context;
        public TeamDbRepository(DevOpsContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<Team>> GetAllAsync()
        {
            return await _context.Teams
                .Include(t => t.Developers)
                .ToListAsync();
        }

        public async Task<Team?> GetByIdAsync(Guid teamId)
        {
            return await _context.Teams
                .Include(t => t.Developers)
                .FirstOrDefaultAsync(t => t.Id == teamId);
        }
    }
}