using HumanResources.Api.Models;
using Test;

namespace HumanResources.Api.Tests.Builders
{
    internal class EmployeeCreateModelBuilder : BuilderBase<EmployeeCreateModel>
    {
        public EmployeeCreateModelBuilder()
        {
            Item = new EmployeeCreateModel
            {
                FirstName = Random.Shared.NextString(),
                LastName = Random.Shared.NextString(),
                StartDate = DateTime.Now
            };
        }
    }
}