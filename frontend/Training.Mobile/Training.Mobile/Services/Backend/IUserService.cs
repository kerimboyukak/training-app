using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Training.Mobile.Services.Backend
{
    public interface IUserService : INotifyPropertyChanged
    {
        string? Id { get;}
        bool IsCoach { get; set; }
        Task ClearUserInfoAsync();
        Task<bool> CheckIfUserIsCoachAsync(string subjectId);
    }
}
