using DevOps.AppLogic.Events;
using IntegrationEvents.Employee;
using Test;

namespace DevOps.AppLogic.Tests.Builders;

internal class EmployeeHiredIntegrationEventBuilder : BuilderBase<EmployeeHiredIntegrationEvent>
{
    public EmployeeHiredIntegrationEventBuilder()
    {
        Item = new EmployeeHiredIntegrationEvent
        {
            Number = Random.Shared.NextString(),
            FirstName = Random.Shared.NextString(),
            LastName = Random.Shared.NextString()
        };
    }
}