using HumanResources.AppLogic;
using HumanResources.Domain;
using Microsoft.EntityFrameworkCore;

namespace HumanResources.Infrastructure
{
    internal class EmployeeDbRepository : IEmployeeRepository  // andere klassen dwingen om de interface te gebruiken door deze op internal te zetten
    {
        private readonly HumanResourcesContext _context;

        public EmployeeDbRepository(HumanResourcesContext context)
        {
            _context = context;
        }
        public async Task AddAsync(IEmployee employee)
        {
            // not referencing the DbSet directly because we want to work with the IEmployee interface
            // Entity Framework will determine the right DbSet based on the type of the object
            _context.Add(employee);
            await CommitTrackedChangesAsync();
        }

        public Task CommitTrackedChangesAsync()
        {
            return _context.SaveChangesAsync();
        }

        public async Task<IEmployee?> GetByNumberAsync(EmployeeNumber number)
        {
            return await _context.Employees.FindAsync(number);
        }

        public async Task<int> GetNumberOfStartersOnAsync(DateTime startDate)
        {
            return await _context.Employees.CountAsync(e => e.StartDate.Date == startDate.Date);
        }
    }
}
