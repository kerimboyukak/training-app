using AutoMapper;
using Training.Domain;

namespace Training.Api.Models
{
    public class ApprenticeDetailModel
    {
        public string Id { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public int Xp { get; set; }

        private class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<Apprentice, ApprenticeDetailModel>();
            }
        }
    }
}
