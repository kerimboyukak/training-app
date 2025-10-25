namespace IntegrationEvents.Employee
{
    public class EmployeeFinishedTrainingIntegrationEvent : IntegrationEventBase
    {
        public string EmployeeNumber { get; set; } = string.Empty;

        public int TrainingHours { get; set; }
    }
}
