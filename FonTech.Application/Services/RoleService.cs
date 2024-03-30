using AutoMapper;
using FonTech.Domain.DTO.Role;
using FonTech.Domain.DTO.UserRole;
using FonTech.Domain.Entities;
using FonTech.Domain.Interfaces.Databases;
using FonTech.Domain.Interfaces.Repositories;
using FonTech.Domain.Interfaces.Services;
using FonTech.Domain.Result;
using Microsoft.EntityFrameworkCore;

namespace FonTech.Application.Services;

public class RoleService : IRoleService
{
    private readonly IBaseRepository<UserRole> _userRoleRepository;
    private readonly IUnitofWorkRepository _unitofWorkRepository; 
    private readonly IBaseRepository<User> _userRepository;
    private readonly IBaseRepository<Role> _roleRepository;
    private readonly IMapper _mapper;

    public RoleService(IBaseRepository<User> userRepository, 
        IBaseRepository<Role> roleRepository, IMapper mapper, 
        IBaseRepository<UserRole> userRoleRepository, IUnitofWorkRepository unitofWorkRepository)
    {
        _unitofWorkRepository = unitofWorkRepository;
        _userRoleRepository = userRoleRepository;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<BaseResult<RoleDto>> CreateRoleAsync(CreateRoleDto dto)
    {
        var role = await _roleRepository.GetAll().FirstOrDefaultAsync(r => r.Name == dto.Name);
        if (role != null)
        {
            return new BaseResult<RoleDto> 
            {
                ErrorMessage = "Роль уже существует",
                ErrorCode = 31
            };
        }

        role = new Role()
        {
            Name = dto.Name
        };

        await _roleRepository.CreateAsync(role);
        return new BaseResult<RoleDto>()
        {
            Data = _mapper.Map<RoleDto>(role)
        };
    }

    /// <inheritdoc />
    public async Task<BaseResult<RoleDto>> DeleteRoleAsync(long id)
    {
        var role = await _roleRepository.GetAll().FirstOrDefaultAsync(r => r.Id == id);
        if (role == null)
        {
            return new BaseResult<RoleDto>
            {
                ErrorMessage = "Роль не найдена",
                ErrorCode = 32
            };
        }

        _roleRepository.Remove(role);
        await _roleRepository.SaveChangesAsync();
        return new BaseResult<RoleDto>()
        {
            Data = _mapper.Map<RoleDto>(role)
        };
    }

    /// <inheritdoc />
    public async Task<BaseResult<RoleDto>> UpdateRoleAsync(RoleDto dto)
    {
        var role = await _roleRepository.GetAll().FirstOrDefaultAsync(r => r.Id == dto.Id);
        if (role == null)
        {
            return new BaseResult<RoleDto>
            {
                ErrorMessage = "Роль не найдена",
                ErrorCode = 32
            };
        }

        role.Name = dto.Name;

        var updatedRole = _roleRepository.Update(role);
        await _roleRepository.SaveChangesAsync();
        return new BaseResult<RoleDto>()
        {
            Data = _mapper.Map<RoleDto>(updatedRole)
        };
    }

    /// <inheritdoc />
    public async Task<BaseResult<UserRoleDto>> AddRoleForUserAsync(UserRoleDto dto)
    {
        var user = await _userRepository
            .GetAll()
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(r => r.Login == dto.Login);

        if (user == null)
        {

            return new BaseResult<UserRoleDto>
            {
                ErrorMessage = "Пользователь не найден",
                ErrorCode = 11
            };
        }

        var roles = user.Roles.Select(r => r.Name).ToArray();
        if (roles.All(r => r == dto.RoleName))
        {
            var role = await _roleRepository.GetAll().FirstOrDefaultAsync(r => r.Name == dto.RoleName);
            if (role == null)
            {
                return new BaseResult<UserRoleDto>
                {
                    ErrorMessage = "Роль не найдена",
                    ErrorCode = 32
                };
            }

            UserRole userRole = new UserRole()
            {
                RoleId = role.Id,
                UserId = user.Id
            };

            await _userRoleRepository.CreateAsync(userRole);

            return new BaseResult<UserRoleDto>
            {
                Data = new UserRoleDto()
                {
                    Login = user.Login,
                    RoleName = dto.RoleName,
                }
            };
        }

        return new BaseResult<UserRoleDto>
        {
            ErrorMessage = "Пользователь уже имеет эту роль",
            ErrorCode = 14
        };
    }

    /// <inheritdoc />
    public async Task<BaseResult<UserRoleDto>> DeleteRoleForUserAsync(DeleteUserRoleDto dto)
    {
        var user = await _userRepository
            .GetAll()
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(r => r.Login == dto.Login);

        if (user == null)
        {

            return new BaseResult<UserRoleDto>
            {
                ErrorMessage = "Пользователь не найден",
                ErrorCode = 11
            };
        }

        var role = user.Roles.FirstOrDefault(r => r.Id == dto.RoleId);
        if (role == null)
        {
            return new BaseResult<UserRoleDto>
            {
                ErrorMessage = "Роль не найдена",
                ErrorCode = 32
            };
        }

        var userRole = await _userRoleRepository
            .GetAll()
            .Where(x => x.RoleId == role.Id)
            .FirstOrDefaultAsync(x => x.UserId == user.Id);

        _userRoleRepository.Remove(userRole);
        await _userRoleRepository.SaveChangesAsync();

        return new BaseResult<UserRoleDto>
        {
            Data = new UserRoleDto()
            {
                Login = user.Login,
                RoleName = role.Name,
            }
        };
    }

    /// <inheritdoc />
    public async Task<BaseResult<UserRoleDto>> UpdateRoleForUserAsync(UpdateUserRoleDto dto)
    {
        var user = await _userRepository
            .GetAll()
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(r => r.Login == dto.Login);

        if (user == null)
        {

            return new BaseResult<UserRoleDto>
            {
                ErrorMessage = "Пользователь не найден",
                ErrorCode = 11
            };
        }

        var role = user.Roles.FirstOrDefault(r => r.Id == dto.FromRoleId);
        if (role == null)
        {
            return new BaseResult<UserRoleDto>
            {
                ErrorMessage = "Роль не найдена",
                ErrorCode = 32
            };
        }

        var newRoleForUser = await _roleRepository.GetAll().FirstOrDefaultAsync(x => x.Id == dto.ToRoleId);
        if (newRoleForUser == null)
        {
            return new BaseResult<UserRoleDto>
            {
                ErrorMessage = "Роль не найдена",
                ErrorCode = 32
            };
        }

        using (var transaction = await _unitofWorkRepository.BeginTransactionAsync())
        {
            try
            {
                var userRole = await _unitofWorkRepository.UserRoles
                .GetAll()
                .Where(x => x.RoleId == role.Id)
                .FirstAsync(x => x.UserId == user.Id);

                _unitofWorkRepository.UserRoles.Remove(userRole);
                await _unitofWorkRepository.SaveChangesAsync();

                var newUserRole = new UserRole()
                {
                    UserId = user.Id,
                    RoleId = newRoleForUser.Id,
                };
                await _unitofWorkRepository.UserRoles.CreateAsync(newUserRole);
                await _unitofWorkRepository.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
            }
        }

        return new BaseResult<UserRoleDto>
        {
            Data = new UserRoleDto()
            {
                Login = user.Login,
                RoleName = newRoleForUser.Name,
            }
        };
    }
}
