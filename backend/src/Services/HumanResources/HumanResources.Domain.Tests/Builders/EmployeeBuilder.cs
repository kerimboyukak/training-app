using Test;

namespace HumanResources.Domain.Tests.Builders
{
    internal class EmployeeBuilder : BuilderBase<Employee>
    {
        public EmployeeBuilder()
        {
            ConstructItem();
            DateTime startDate = Random.Shared.NextDateTimeInFuture();
            SetProperty(e => e.Number, new EmployeeNumber(startDate, Random.Shared.Next(1, 1000)));
            SetProperty(e => e.FirstName, Random.Shared.NextString());
            SetProperty(e => e.LastName, Random.Shared.NextString());
            SetProperty(e => e.StartDate, startDate);
            SetProperty(e => e.EndDate, startDate.AddDays(Random.Shared.Next(10, 101)));
        }
        public EmployeeBuilder WithNumber(EmployeeNumber number)
        {
            SetProperty(e => e.Number, number);
            return this;
        }
        public EmployeeBuilder WithEndDate(DateTime? endDate)
        {
            SetProperty(e => e.EndDate, endDate);
            return this;
        }

        public EmployeeBuilder WithStartDate(DateTime startDate)
        {
            SetProperty(e => e.StartDate, startDate);
            return this;
        }
        public EmployeeBuilder WithIsCoach(bool isCoach)
        {
            SetProperty(e => e.IsCoach, isCoach);
            return this;
        }

        public EmployeeBuilder WithTrainingHours(int trainingHours)
        {
            SetProperty(e => e.TrainingHours, trainingHours);
            return this;
        }
    }
}