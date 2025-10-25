using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Training.AppLogic;
using Training.Domain;

namespace Training.Infrastructure
{
    internal class TrainingDbRepository : ITrainingRepository
    {
        private readonly TrainingContext _context;

        public TrainingDbRepository(TrainingContext context)
        {
            _context = context;
        }

        public async Task<Domain.Training> AddAsync(Domain.Training training)
        {
            await _context.Trainings.AddAsync(training);
            await CommitTrackedChangesAsync();
            return training;
        }

        public async Task<IReadOnlyList<Domain.Training>> GetAllAsync()
        {
            return await _context.Trainings
                .Include(t => t.Participations).ThenInclude(p => p.Apprentice)

                .Include(t => t.Room)
                .Include(t => t.Coach)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Domain.Training>> GetFutureTrainingsAsync()
        {
            return await _context.Trainings
                .Include(t => t.Participations).ThenInclude(p => p.Apprentice)

                .Include(t => t.Room)
                .Include(t => t.Coach)
                .Where(t => t.TimeWindow.Start.AddMinutes(15) > DateTime.Now.AddHours(1))
                .ToListAsync();
        }
        public async Task<IReadOnlyList<Domain.Training>> GetPastTrainingsAsync()
        {
            return await _context.Trainings
                .Include(t => t.Participations).ThenInclude(p => p.Apprentice)

                .Include(t => t.Room)
                .Include(t => t.Coach)
                .Where(t => t.TimeWindow.Start < DateTime.Now.AddHours(1))
                .ToListAsync();
        }
        public async Task<int> GetNumberOfTrainingsByName(string name)
        {
            return await _context.Trainings.CountAsync(t => t.Name == name);
        }

        public async Task<Domain.Training?> GetByCodeAsync(Code code)
        {
            return await _context.Trainings
                .Include(t => t.Participations).ThenInclude(p => p.Apprentice)
                .Include(t => t.Room)
                .Include(t => t.Coach)
                .FirstOrDefaultAsync(t => t.TrainingCode == code);
        }

        public async Task CommitTrackedChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<Domain.Training>> GetTrainingsByRoomCode(Code roomCode)
        {
            return await _context.Trainings
                .Include(t => t.Participations)
                .Include(t => t.Room)
                .Include(t => t.Coach)
                .Where(t => t.RoomCode == roomCode)
                .ToListAsync();
        }
    }
}