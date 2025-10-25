using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Training.Domain;

namespace Training.AppLogic
{
    public interface IApprenticeRepository
    {
        Task<Apprentice?> GetByIdAsync(string number);

        Task AddAsync(Apprentice apprentice);
        Task CommitTrackedChangesAsync();
        Task<Apprentice?> GetByNameAndCompanyAsync(string firstName, string lastName, string company);
    }
}
