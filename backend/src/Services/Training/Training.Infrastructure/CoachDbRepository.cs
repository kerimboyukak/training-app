using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Training.AppLogic;
using Training.Domain;

namespace Training.Infrastructure
{
    internal class CoachDbRepository : ICoachRepository
    {
        private readonly TrainingContext _context;

        public CoachDbRepository(TrainingContext context)
        {
            _context = context;   
        }
        public async Task AddAsync(Coach coach)
        {
            _context.Add(coach);
            await CommitTrackedChangesAsync();
        }

        public async Task CommitTrackedChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<Coach?> GetByIdAsync(string number)
        {
            return await _context.Coaches.FirstOrDefaultAsync(c => c.Id == number);

        }
    }
}
