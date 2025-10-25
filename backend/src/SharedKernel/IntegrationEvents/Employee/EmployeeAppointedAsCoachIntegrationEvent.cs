namespace IntegrationEvents.Employee
{
    public class EmployeeAppointedAsCoachIntegrationEvent : IntegrationEventBase    // Creating a Coach when an employee is appointed as a coach
    {
        public string Number { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
    }
}
