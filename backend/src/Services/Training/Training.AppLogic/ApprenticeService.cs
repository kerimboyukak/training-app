using Domain;
using HumanResources.Domain;
using IntegrationEvents.Employee;
using MassTransit;
using Training.Domain;

namespace Training.AppLogic
{
    internal class ApprenticeService : IApprenticeService
    {
        private readonly ITrainingRepository _trainingRepository;
        private readonly IApprenticeRepository _apprenticeRepository;
        private readonly IPublishEndpoint _eventBus;


        public ApprenticeService(ITrainingRepository trainingRepository, IApprenticeRepository apprenticeRepository, IPublishEndpoint eventBus)
        {
            _trainingRepository = trainingRepository;
            _apprenticeRepository = apprenticeRepository;
            _eventBus = eventBus;
        }

        public async Task<Apprentice> RegisterApprentice(string trainingCode, string apprenticeId)
        {
            Domain.Training? training = await _trainingRepository.GetByCodeAsync(trainingCode);
            Contracts.Require(training is not null, $"Cannot find a training with code '{trainingCode}'");

            Apprentice? apprentice = await _apprenticeRepository.GetByIdAsync(apprenticeId);
            Contracts.Require(apprentice is not null, $"Cannot find an apprentice with id '{apprenticeId}'");


            training!.Register(apprentice!);
            await _trainingRepository.CommitTrackedChangesAsync();

            return apprentice!;
        }

        public async Task<Apprentice> RegisterExternalApprentice(string trainingCode, string firstName, string lastName, string company)
        {
            var existingApprentice = await _apprenticeRepository.GetByNameAndCompanyAsync(firstName, lastName, company);
            if (existingApprentice is not null)
            {
                await RegisterApprentice(trainingCode, existingApprentice.Id);
                return existingApprentice;
            }

            string id = Guid.NewGuid().ToString().Substring(0, 11);
            var apprentice = Apprentice.CreateNew(id, firstName, lastName, company);
            await _apprenticeRepository.AddAsync(apprentice);
            await RegisterApprentice(trainingCode, apprentice.Id);

            return apprentice;
        }

        public async Task<Participation> FinishParticipation(string trainingCode, string apprenticeId)
        {
            var training = await _trainingRepository.GetByCodeAsync(new Code(trainingCode));
            Contracts.Require(training is not null, "The training with the given code does not exist.");

            var apprentice = await _apprenticeRepository.GetByIdAsync(apprenticeId);
            Contracts.Require(apprentice is not null, "The apprentice with the given id does not exist.");

            training?.FinishParticipation(apprenticeId);

            var trainingHours = (training!.TimeWindow.End - training.TimeWindow.Start).TotalHours;
            var xp = (int)(trainingHours / 2);

            apprentice!.AddXp(xp);

            // HumanResources should be notified when an employees TrainingHours increases
            var @event = new EmployeeFinishedTrainingIntegrationEvent
            {
                EmployeeNumber = apprenticeId,
                TrainingHours = (int)trainingHours
            };
            await _eventBus.Publish(@event);

            await _apprenticeRepository.CommitTrackedChangesAsync();

            var participation = training.Participations.FirstOrDefault(p => p.ApprenticeId == apprenticeId);
            Contracts.Require(participation is not null, "The participation with the given apprentice id does not exist.");

            return participation!;
        }
    }
}
