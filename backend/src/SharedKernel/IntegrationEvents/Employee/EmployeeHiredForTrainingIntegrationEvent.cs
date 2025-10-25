namespace IntegrationEvents.Employee
{
    public class EmployeeHiredForTrainingIntegrationEvent : IntegrationEventBase        // Creating an apprentice in Training when an employee is hired
    {
        public string Number { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
    }
}
