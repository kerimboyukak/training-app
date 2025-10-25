using Domain;
using HumanResources.Domain;

public interface IEmployeeService
{
    Task<IEmployee> HireNewAsync(string lastName, string firstName, DateTime startDate, EmployeeType type);
    Task DismissAsync(EmployeeNumber employeeNumber, bool withNotice);
    Task AppointEmployeeAsCoach(EmployeeNumber employeeNumber);
}