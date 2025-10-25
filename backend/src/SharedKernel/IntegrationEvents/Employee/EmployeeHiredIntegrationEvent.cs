namespace IntegrationEvents.Employee
{
    public class EmployeeHiredIntegrationEvent : IntegrationEventBase       // Creating a developer in DevOps when an employee is hired
    {
        public string Number { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
    }
}