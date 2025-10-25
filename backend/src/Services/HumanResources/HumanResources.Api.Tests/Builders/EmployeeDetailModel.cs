using HumanResources.Api.Models;
using Test;

namespace HumanResources.Api.Tests.Builders
{
    internal class EmployeeDetailModelBuilder : BuilderBase<EmployeeDetailModel>
    {
        public EmployeeDetailModelBuilder()
        {
            Item = new EmployeeDetailModel
            {
                Number = Random.Shared.NextString(),
                FirstName = Random.Shared.NextString(),
                LastName = Random.Shared.NextString(),
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(1)
            };
        }
    }
}