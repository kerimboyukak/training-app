using IntegrationEvents.Employee;
using MassTransit;
using Microsoft.Extensions.Logging;
using Training.Domain;

namespace Training.AppLogic.Events
{
    internal class EmployeeAppointedAsCoachEventConsumer : IConsumer<EmployeeAppointedAsCoachIntegrationEvent>
    {
        private readonly ICoachRepository _coachRepository;
        private readonly ILogger<EmployeeAppointedAsCoachEventConsumer> _logger;
       
        public EmployeeAppointedAsCoachEventConsumer(ICoachRepository coachRepository, ILogger<EmployeeAppointedAsCoachEventConsumer> logger)
        {
            _coachRepository = coachRepository;
            _logger = logger;

        }
        public async Task Consume(ConsumeContext<EmployeeAppointedAsCoachIntegrationEvent> context)
        {
            EmployeeAppointedAsCoachIntegrationEvent @event = context.Message;
            _logger.LogDebug($"Training - Handling employee hire. Id: {@event.EventId}");
            Coach? coach = await _coachRepository.GetByIdAsync(@event.Number);
            if (coach is not null)
            {
                _logger.LogDebug($"Training - No coach added. A coach with id '{@event.Number}' already exists. Id: {@event.EventId}");
                return;
            }
            coach = Coach.CreateNew(@event.Number, @event.FirstName, @event.LastName);
            await _coachRepository.AddAsync(coach);
            _logger.LogDebug($"Training - Coach with id '{@event.Number}' added. Id: {@event.EventId}");
        }
    }
}
