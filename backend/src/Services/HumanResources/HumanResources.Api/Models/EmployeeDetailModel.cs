using AutoMapper;
using HumanResources.Domain;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HumanResources.Api.Models
{
    public class EmployeeDetailModel
    {
        public string Number { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public EmployeeType Type { get; set; }
        public bool IsCoach { get; set; }
        public int TrainingHours { get; set; }


        private class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<IEmployee, EmployeeDetailModel>();
            }
        }
    }
}