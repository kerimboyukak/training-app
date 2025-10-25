using HumanResources.Domain;
using IntegrationEvents.Employee;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace HumanResources.AppLogic.Events
{
    internal class EmployeeFinishedTrainingEventConsumer : IConsumer<EmployeeFinishedTrainingIntegrationEvent>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<EmployeeFinishedTrainingEventConsumer> _logger;

        public EmployeeFinishedTrainingEventConsumer(IEmployeeRepository employeeRepository, ILogger<EmployeeFinishedTrainingEventConsumer> logger)
        {
            _employeeRepository = employeeRepository;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<EmployeeFinishedTrainingIntegrationEvent> context)
        {
            EmployeeFinishedTrainingIntegrationEvent @event = context.Message;
            _logger.LogDebug($"HumanResources - Handling employee finish training. Id: {@event.EventId}");
            IEmployee? employee = _employeeRepository.GetByNumberAsync(@event.EmployeeNumber).Result;
            if (employee is null)
            {
                _logger.LogDebug($"HumanResources - No employee found with number '{@event.EmployeeNumber}'. Id: {@event.EventId}");
                return;
            }
            employee.FinishTraining(@event.TrainingHours);
            await _employeeRepository.CommitTrackedChangesAsync();
            _logger.LogDebug($"HumanResources - Employee with number '{@event.EmployeeNumber}' finished training. Id: {@event.EventId}");

        }
    }
}
