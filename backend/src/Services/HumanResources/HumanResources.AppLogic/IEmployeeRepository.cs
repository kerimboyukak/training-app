using HumanResources.Domain;

namespace HumanResources.AppLogic
{
    public interface IEmployeeRepository
    {
        Task AddAsync(IEmployee newEmployee);
        Task<IEmployee?> GetByNumberAsync(EmployeeNumber number);

        Task<int> GetNumberOfStartersOnAsync(DateTime startDate);
        Task CommitTrackedChangesAsync();
    }

}