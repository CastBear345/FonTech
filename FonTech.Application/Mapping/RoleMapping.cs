using AutoMapper;
using FonTech.Domain.DTO.Role;
using FonTech.Domain.Entities;

namespace FonTech.Application.Mapping;

public class RoleMapping : Profile
{
    public RoleMapping()
    {
        CreateMap<Role, RoleDto>().ReverseMap();
    }
}
