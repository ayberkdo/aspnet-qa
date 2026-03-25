using aspnet_qa.API.Models;
using aspnet_qa.API.DTOs;
using AutoMapper;

namespace aspnet_qa.API.Mapping
{
    public class MapProfile : Profile
    {
        public MapProfile()
        {
            CreateMap<AppUser, UserDto>().ReverseMap();
            CreateMap<Tag, TagDto>().ReverseMap();

            CreateMap<Answer, AnswerDto>()
                .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.AppUser != null ? src.AppUser.FullName : "Kullanıcı"))
                .ForMember(dest => dest.AuthorUserName, opt => opt.MapFrom(src => src.AppUser != null ? src.AppUser.UserName : "kullanici"))
                .ForMember(dest => dest.AuthorPhotoUrl, opt => opt.MapFrom(src => src.AppUser != null ? src.AppUser.PhotoUrl : "default-profile-photo.jpg"))
                .ForMember(dest => dest.QuestionTitle, opt => opt.MapFrom(src => src.Question != null ? src.Question.Title : string.Empty))
                .ForMember(dest => dest.QuestionSlug, opt => opt.MapFrom(src => src.Question != null ? src.Question.Slug : string.Empty))
                .ReverseMap();

            CreateMap<Question, QuestionDto>()
                .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.AppUser != null ? src.AppUser.FullName : "Kullanıcı"))
                .ForMember(dest => dest.AuthorUserName, opt => opt.MapFrom(src => src.AppUser != null ? src.AppUser.UserName : "kullanici"))
                .ForMember(dest => dest.AuthorPhotoUrl, opt => opt.MapFrom(src => src.AppUser != null ? src.AppUser.PhotoUrl : "default-profile-photo.jpg"))
                .ForMember(dest => dest.AnswerCount, opt => opt.MapFrom(src => src.Answers != null ? src.Answers.Count : 0))
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src =>
                    src.QuestionTags != null
                        ? src.QuestionTags.Select(qt => qt.Tag)
                        : Enumerable.Empty<Tag>()))
                .ReverseMap();
        }
    }
}
