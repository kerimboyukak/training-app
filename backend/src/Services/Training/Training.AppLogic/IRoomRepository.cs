using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Training.Domain;

namespace Training.AppLogic
{
    public interface IRoomRepository
    {
        Task<IReadOnlyList<Room>> GetAllAsync();
        Task<Room?> GetByIdAsync(string roomCode);
        Task CommitTrackedChangesAsync();    
    }
}
