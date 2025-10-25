using AutoMapper;
using Training.Domain;

namespace Training.Api.Models
{
    public class TrainingDetailModel
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int MaximumCapacity { get; set; }
        public RoomDetailModel? Room { get; set; }
        public CoachDetailModel? Coach { get; set; }
        public List<ParticipationDetailModel> Participations { get; set; } = new();
        public string TimeWindow { get; set; } = string.Empty;

        private class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<Domain.Training, TrainingDetailModel>()
                    .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.TrainingCode.ToString()))   // convert TrainingCode to string
                    .ForMember(dest => dest.Room, opt => opt.MapFrom(src => src.Room))
                    .ForMember(dest => dest.Coach, opt => opt.MapFrom(src => src.Coach))
                    .ForMember(dest => dest.Participations, opt => opt.MapFrom(src => src.Participations))
                    .ForMember(dest => dest.TimeWindow, opt => opt.MapFrom(src => src.TimeWindow.ToString())); // convert TimeWindow to string

                CreateMap<Room, RoomDetailModel>();
                CreateMap<Coach, CoachDetailModel>();
                CreateMap<Participation, ParticipationDetailModel>();
            }
        }
    }
}
