using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Training.Mobile.Models;

namespace Training.Mobile.Services.Backend
{
    public interface ITrainingService
    {
        Task CreateTrainingAsync(TrainingCreateModel model);
        Task<IReadOnlyList<Room>> GetAllRoomsAsync();
        Task<TrainingDetail> GetTrainingDetailsAsync(string code);
        Task<IReadOnlyList<TrainingDetail>> GetFutureTrainingsAsync();
        Task<IReadOnlyList<TrainingDetail>> GetPastTrainingsAsync();
        Task RegisterApprenticeAsync(string code, string id);
        Task RegisterExternalApprenticeAsync(string code, ApprenticeCreateModel model);
        Task CompleteParticipation(string code, string id);
    }
}
