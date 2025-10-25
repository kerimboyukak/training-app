using AutoMapper;
using Training.Domain;

namespace Training.Api.Models
{
    public class CoachDetailModel
    {
        public string Id { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        private class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<Coach, CoachDetailModel>();
            }
        }
    }
}
