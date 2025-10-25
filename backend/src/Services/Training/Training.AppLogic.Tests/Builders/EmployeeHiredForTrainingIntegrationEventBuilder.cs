using IntegrationEvents.Employee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test;

namespace Training.AppLogic.Tests.Builders
{
    internal class EmployeeHiredForTrainingIntegrationEventBuilder : BuilderBase<EmployeeHiredForTrainingIntegrationEvent>
    {
        public EmployeeHiredForTrainingIntegrationEventBuilder()
        {
            Item = new EmployeeHiredForTrainingIntegrationEvent
            {
                Number = Random.Shared.NextString(),
                FirstName = Random.Shared.NextString(),
                LastName = Random.Shared.NextString(),
                Company = Random.Shared.NextString()
            };
        }
    }
}
