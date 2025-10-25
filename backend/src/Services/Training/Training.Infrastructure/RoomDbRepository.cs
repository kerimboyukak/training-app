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
    internal class RoomDbRepository : IRoomRepository
    {
        private readonly TrainingContext _context;
        public RoomDbRepository(TrainingContext context)
        {
            _context = context;
        }

        public async Task CommitTrackedChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<Room>> GetAllAsync()
        {

            return await _context.Rooms.ToListAsync();

        }

        public async Task<Room?> GetByIdAsync(String roomCode)
        {
            return await _context.Rooms.FirstOrDefaultAsync(r => r.RoomCode == roomCode);
        }
    }
}
