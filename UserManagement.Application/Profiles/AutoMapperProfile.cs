using AutoMapper;
using UserManagement.Application.Dtos;
using UserManagement.Application.Dtos.Permission;
using UserManagement.Application.Dtos.Role;
using UserManagement.Application.Extensions;
using UserManagement.Application.Models;

namespace UserManagement.Application.Profiles;

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

        CreateMap<Client, ClientDto>().ReverseMap();
        CreateMap<RoleRepresentation, RoleRepresentationDto>().ReverseMap();
        CreateMap<ClientMappingsRepresentation, ClientMappingsRepresentationDto>().ReverseMap();
        CreateMap<RealmMappingsResponse, RealmMappingsResponseDto>().ReverseMap();
        CreateMap<RoleRequestDto, PermissionRequestDto>().ReverseMap();
        CreateMap<RoleResponseDto, PermissionResponseDto>().ReverseMap();
        CreateMap<RoleResponse, RoleResponseDto>().ReverseMap();
        CreateMap<RoleResponse, PermissionResponseDto>().ReverseMap();
        // Ensure Lists are mapped correctly
        CreateMap<List<RoleResponseDto>, List<PermissionResponseDto>>()
            .ConvertUsing((src, dest, context) => src.Select(role => context.Mapper.Map<PermissionResponseDto>(role)).ToList());

        CreateMap<List<RoleResponse>,List<RoleResponseDto>>()
     .ConvertUsing((src, dest, context) => src.Select(role => context.Mapper.Map<RoleResponseDto>(role)).ToList());
        CreateMap<List<RoleResponse>, List<PermissionResponseDto>>()
 .ConvertUsing((src, dest, context) => src.Select(role => context.Mapper.Map<PermissionResponseDto>(role)).ToList());
    }
}
