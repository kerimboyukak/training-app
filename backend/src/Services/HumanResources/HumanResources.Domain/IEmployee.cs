namespace HumanResources.Domain
{
    public interface IEmployee
    {
        EmployeeNumber Number { get; }
        string LastName { get; }
        string FirstName { get; }
        DateTime StartDate { get; }
        DateTime? EndDate { get; }
        EmployeeType Type { get; }
        bool IsCoach { get; }
        int TrainingHours { get; }
        void FinishTraining(int hours);
        void Dismiss(bool withNotice = true);
        void AppointAsCoach();
    }
}
