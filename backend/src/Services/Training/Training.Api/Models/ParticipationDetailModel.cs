using AutoMapper;
using Training.Domain;

namespace Training.Api.Models
{
    public class ParticipationDetailModel
    {
        public string TrainingCode { get; set; } = string.Empty;
        public bool IsFinished { get; set; }
        public ApprenticeDetailModel Apprentice { get; set; } = new ApprenticeDetailModel();

        private class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<Participation, ParticipationDetailModel>()
                    .ForMember(dest => dest.Apprentice, opt => opt.MapFrom(src => src.Apprentice));

                CreateMap<Apprentice, ApprenticeDetailModel>();

            }
        }
    }
}