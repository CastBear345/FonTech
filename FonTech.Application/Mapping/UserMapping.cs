using AutoMapper;
using FonTech.Domain.DTO.User;
using FonTech.Domain.Entities;

namespace FonTech.Application.Mapping;

public class UserMapping : Profile
{
    public UserMapping()
    {
        CreateMap<User, UserDto>().ReverseMap();
    }
}
