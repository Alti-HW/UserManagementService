using AutoMapper;
using UserManagement.Application.Dtos;
using UserManagement.Application.Extensions;
using UserManagement.Application.Models;

namespace UserManagement.Application.Profiles
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Users, UserDto>()
                .ForMember(d => d.Created, opt => opt.MapFrom(src => src.CreatedTimestamp.ToDateTimeOrNow()))
                .ForMember(d => d.Id, opt => opt.MapFrom(src => new Guid(src.Id)))
                .ReverseMap()
                .ForMember(d => d.CreatedTimestamp, opt => opt.MapFrom(src => src.Created.ToUnixTimestamp()))
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id.ToString()));
        }
    }
}
