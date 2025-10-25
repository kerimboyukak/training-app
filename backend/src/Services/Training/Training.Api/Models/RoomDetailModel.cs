using AutoMapper;
using Training.Domain;

namespace Training.Api.Models
{
    public class RoomDetailModel
    {
        public string RoomCode { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        private class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<Room, RoomDetailModel>();
            }
        }
    }
}
