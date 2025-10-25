
namespace HumanResources.Domain
{
    public interface IEmployeeFactory
    {
        // resposible for creating IEmployee instances
        // other layers should use this interface instead of the constructor of the Employee class
        IEmployee CreateNew(string lastName, string firstName, DateTime startDate, int sequence, EmployeeType type);


    }
}