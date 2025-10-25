using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Training.Domain;

namespace Training.AppLogic
{
    public interface IApprenticeService
    {
        Task<Apprentice> RegisterApprentice(string trainingCode, string apprenticeId);
        Task<Apprentice> RegisterExternalApprentice(string trainingCode, string firstName, string lastName, string company);
        Task<Participation> FinishParticipation(string trainingCode, string apprenticeId);

    }
}
