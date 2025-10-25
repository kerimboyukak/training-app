using DevOps.AppLogic;
using DevOps.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace DevOps.Infrastructure
{
    internal class DeveloperDbRepository : IDeveloperRepository
    {
        private readonly DevOpsContext _context;
        public DeveloperDbRepository(DevOpsContext context)
        {
            _context = context;   
        }

        public async Task AddAsync(Developer developer)
        {
            _context.Add(developer);
            await CommitTrackedChangesAsync();
        }

        public async Task CommitTrackedChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<Developer>> FindDevelopersWithoutATeamAsync()
        {
            return await _context.Developers.Where(d => d.TeamId == null).ToListAsync();
        }

        public async Task<Developer?> GetByIdAsync(string number)
        {
            return await _context.Developers.FirstOrDefaultAsync(d => d.Id == number);
        }
    }
}
