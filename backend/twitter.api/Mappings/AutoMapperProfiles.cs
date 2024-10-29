using AutoMapper;
using twitter.api.Models.Domain;
using twitter.api.Models.DTO;

namespace twitter.api.Mappings
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<UserRegisterDto, User>().ReverseMap();
            CreateMap<User,UserResponseDto>().ReverseMap();
        }
    }
}
