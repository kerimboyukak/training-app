using Training.Domain;

namespace Training.AppLogic
{
    public interface ICoachService
    {
        Task<Domain.Training> CreateTraining(string name, string description, int maximumCapacity, Code roomCode, string coachId, TimeWindow timeWindow);
    }
}
