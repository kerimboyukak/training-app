using Domain;

namespace HumanResources.Domain
{
    internal class Employee : Entity, IEmployee // making this internal so that other layers are forced to use the IEmployee interface instead of Employee
    {
        // other classes are not allowed to change the state of an Employee
        public EmployeeNumber Number { get; private set; }
        public string LastName { get; private set; }
        public string FirstName { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }
        public EmployeeType Type { get; private set; }
        public bool IsCoach { get; private set; }
        public int TrainingHours { get; private set; }

        private Employee()
        {
            // Making sure that the properties of reference types are never null
            Number = new EmployeeNumber(DateTime.MinValue, 1);
            LastName = string.Empty;
            FirstName = string.Empty;
        }

        protected override IEnumerable<object> GetIdComponents()
        {
            yield return Number;
        }

        public void Dismiss(bool withNotice = true)
        {
            if (!withNotice)
            {
                EndDate = DateTime.Now;
                return;
            }

            // Ensure the employee has not already been dismissed with notice
            Contracts.Require(EndDate == null, "Employee has already been dismissed with notice.");

            var now = DateTime.Now;
            var monthsSinceStart = (now.Year - StartDate.Year) * 12 + now.Month - StartDate.Month;

            if (monthsSinceStart < 3)
            {
                EndDate = now.AddDays(7);
            }
            else if (monthsSinceStart < 12)
            {
                EndDate = now.AddDays(14);
            }
            else
            {
                EndDate = now.AddDays(28);
            }
        }
        public void AppointAsCoach()
        {
            Contracts.Require(!IsCoach, "Employee is already a coach");
            IsCoach = true;
        }

        public void FinishTraining(int hours)
        {
            Contracts.Require(hours > 0, "The number of training hours must be a positive number");
            TrainingHours += hours;
        }

        internal class Factory : IEmployeeFactory   // internal because the other layers should use the interface, not the concrete implementation
        {
            public IEmployee CreateNew(string lastName, string firstName, DateTime startDate, int sequence, EmployeeType type)
            {
                Contracts.Require(startDate >= DateTime.Now.AddYears(-1), "The start date of an employee cannot be more than 1 year in the past");
                Contracts.Require(!string.IsNullOrEmpty(lastName), "The last name of an employee cannot be empty");
                Contracts.Require(lastName.Length >= 2, "The last name of an employee must at least have 2 characters");
                Contracts.Require(!string.IsNullOrEmpty(firstName), "The first name of an employee cannot be empty");
                Contracts.Require(firstName.Length >= 2, "The first name of an employee must at least have 2 characters");
                Contracts.Require(Enum.IsDefined(typeof(EmployeeType), type), "Invalid employee type");

                var employee = new Employee
                {
                    Number = new EmployeeNumber(startDate, sequence),
                    FirstName = firstName,
                    LastName = lastName,
                    StartDate = startDate,
                    EndDate = null,
                    Type = type,
                    IsCoach = false,
                    TrainingHours = 0
                };
                return employee;
            }
        }
    }
}