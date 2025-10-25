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
    internal class ApprenticeDbRepository : IApprenticeRepository
    {
        private readonly TrainingContext _context;

        public ApprenticeDbRepository(TrainingContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Apprentice apprentice)
        {
            _context.Apprentices.Add(apprentice);
            await CommitTrackedChangesAsync();
        }

        public async Task CommitTrackedChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<Apprentice?> GetByIdAsync(string number)
        {
            return await _context.Apprentices.FirstOrDefaultAsync(d => d.Id == number);

        }

        public async Task<Apprentice?> GetByNameAndCompanyAsync(string firstName, string lastName, string company)
        {
            return await _context.Apprentices.FirstOrDefaultAsync(d => d.FirstName == firstName && d.LastName == lastName && d.Company == company);
        }
    }
}
