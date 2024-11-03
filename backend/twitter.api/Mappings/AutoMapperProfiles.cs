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
            CreateMap<User, UserResponseDto>().ReverseMap();
            CreateMap<User, UserProfileDto>().ReverseMap();

            CreateMap<User, BasicUserInfoDto>().ReverseMap();
            CreateMap<User, SuggestedUserDto>().ReverseMap();
            CreateMap<User, UpdateUserResponseDto>().ReverseMap();

            CreateMap<Post, CreatePostDto>().ReverseMap();
            CreateMap<Post, PostResponseDto>().ReverseMap();

            CreateMap<Post, PostDto>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.Likes, opt => opt.MapFrom(src => src.Likes))
                .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Comments))
                .ReverseMap();

            CreateMap<Comment, CommentDetailDto>().
                ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
                .ReverseMap();

            CreateMap<Notification, NotificationDto>().ReverseMap();
        }
    }
}
