using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Training.Domain;


namespace Training.AppLogic
{
    public interface ITrainingRepository
    {
        Task<IReadOnlyList<Domain.Training>> GetAllAsync();
        Task<IReadOnlyList<Domain.Training>> GetFutureTrainingsAsync();
        Task<IReadOnlyList<Domain.Training>> GetPastTrainingsAsync();
        Task<Domain.Training?> GetByCodeAsync(Code code);
        Task<Domain.Training> AddAsync(Domain.Training training);
        Task<int> GetNumberOfTrainingsByName(string name);
        Task CommitTrackedChangesAsync();
        Task<IReadOnlyList<Domain.Training>> GetTrainingsByRoomCode(Code roomCode);
    }
}
