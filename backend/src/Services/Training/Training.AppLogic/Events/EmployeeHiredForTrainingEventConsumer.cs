using IntegrationEvents.Employee;
using MassTransit;
using Microsoft.Extensions.Logging;
using Training.Domain;

namespace Training.AppLogic.Events
{
    internal class EmployeeHiredForTrainingEventConsumer : IConsumer<EmployeeHiredForTrainingIntegrationEvent>
    {
        private readonly IApprenticeRepository _apprenticeRepository;
        private readonly ILogger<EmployeeHiredForTrainingEventConsumer> _logger;

        public EmployeeHiredForTrainingEventConsumer(IApprenticeRepository apprenticeRepository, ILogger<EmployeeHiredForTrainingEventConsumer> logger)
        {
            _apprenticeRepository = apprenticeRepository;
            _logger = logger;
        }


        public async Task Consume(ConsumeContext<EmployeeHiredForTrainingIntegrationEvent> context)
        {
            EmployeeHiredForTrainingIntegrationEvent @event = context.Message;
            _logger.LogDebug($"Training - Handling employee hire. Id: {@event.EventId}");
            Apprentice? apprentice = await _apprenticeRepository.GetByIdAsync(@event.Number);
            if (apprentice is not null)
            {
                _logger.LogDebug($"Training - No apprentice added. An apprentice with id '{@event.Number}' already exists. Id: {@event.EventId}");
                return;
            }
            apprentice = Apprentice.CreateNew(@event.Number, @event.FirstName, @event.LastName, @event.Company);
            await _apprenticeRepository.AddAsync(apprentice);
            _logger.LogDebug($"Training - Apprentice with id '{@event.Number}' added. Id: {@event.EventId}");
        }
    }
}
