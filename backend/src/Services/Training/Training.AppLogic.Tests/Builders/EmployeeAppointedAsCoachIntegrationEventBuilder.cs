using IntegrationEvents.Employee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test;

namespace Training.AppLogic.Tests.Builders
{
    internal class EmployeeAppointedAsCoachIntegrationEventBuilder : BuilderBase<EmployeeAppointedAsCoachIntegrationEvent>
    {
        public EmployeeAppointedAsCoachIntegrationEventBuilder()
        {
            Item = new EmployeeAppointedAsCoachIntegrationEvent
            {
                Number = Random.Shared.NextString(),
                FirstName = Random.Shared.NextString(),
                LastName = Random.Shared.NextString()
            };
        }
    }
}
