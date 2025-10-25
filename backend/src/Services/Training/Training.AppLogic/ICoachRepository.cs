using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Training.Domain;

namespace Training.AppLogic
{
    public interface ICoachRepository
    {
        Task<Coach?> GetByIdAsync(string number);

        Task AddAsync(Coach coach);
        Task CommitTrackedChangesAsync();

    }
}
