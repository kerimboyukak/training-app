using DevOps.Domain;
using IntegrationEvents.Employee;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace DevOps.AppLogic.Events;

internal class EmployeeHiredEventConsumer : IConsumer<EmployeeHiredIntegrationEvent>
{
    private readonly IDeveloperRepository _developerRepository;
    private readonly ILogger<EmployeeHiredEventConsumer> _logger;

    public EmployeeHiredEventConsumer(IDeveloperRepository developerRepository, ILogger<EmployeeHiredEventConsumer> logger)
    {
        _developerRepository = developerRepository;
        _logger = logger;
    }
    public async Task Consume(ConsumeContext<EmployeeHiredIntegrationEvent> context)
    {
        EmployeeHiredIntegrationEvent @event = context.Message;
        _logger.LogDebug($"DevOps - Handling employee hire. Id: {@event.EventId}");
        Developer? developer = await _developerRepository.GetByIdAsync(@event.Number);
        if (developer is not null)
        {
            _logger.LogDebug($"DevOps - No developer added. A developer with id '{@event.Number}' already exists. Id: {@event.EventId}");
            return;
        }
        developer = Developer.CreateNew(@event.Number, @event.FirstName, @event.LastName, 0.0);
        await _developerRepository.AddAsync(developer);
        _logger.LogDebug($"DevOps - Developer with id '{@event.Number}' added. Id: {@event.EventId}");
    }
}