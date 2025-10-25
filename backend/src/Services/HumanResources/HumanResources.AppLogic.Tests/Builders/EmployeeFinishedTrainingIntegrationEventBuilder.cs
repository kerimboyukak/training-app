using IntegrationEvents.Employee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test;

namespace HumanResources.AppLogic.Tests.Builders
{
    internal class EmployeeFinishedTrainingIntegrationEventBuilder : BuilderBase<EmployeeFinishedTrainingIntegrationEvent>
    {
        public EmployeeFinishedTrainingIntegrationEventBuilder()
        {
            Item = new EmployeeFinishedTrainingIntegrationEvent
            {
                EmployeeNumber = "20240101001",
                TrainingHours = Random.Shared.Next()
            };
        }
    }
}
