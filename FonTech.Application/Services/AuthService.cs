using AutoMapper;
using FonTech.Domain.DTO;
using FonTech.Domain.DTO.Report;
using FonTech.Domain.DTO.Role;
using FonTech.Domain.DTO.User;
using FonTech.Domain.Entities;
using FonTech.Domain.Enum;
using FonTech.Domain.Interfaces.Databases;
using FonTech.Domain.Interfaces.Repositories;
using FonTech.Domain.Interfaces.Services;
using FonTech.Domain.Result;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Buffers.Text;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace FonTech.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitofWorkRepository _unitofWorkRepository;
    private readonly IBaseRepository<UserToken> _userTokenRepository;
    private readonly IBaseRepository<User> _userRepository;
    private readonly IBaseRepository<Role> _roleRepository;
    private readonly ITokenService _tokenService;
    private readonly ILogger _logger;
    private readonly IMapper _mapper;

    public AuthService(IBaseRepository<User> userRepository, ILogger logger, IMapper mapper,
        IBaseRepository<UserToken> userTokenRepository, ITokenService tokenService,
        IBaseRepository<Role> roleRepository, IUnitofWorkRepository unitofWorkRepository)
    {
        _unitofWorkRepository = unitofWorkRepository;
        _userTokenRepository = userTokenRepository;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _tokenService = tokenService;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<BaseResult<UserDto>> Register(RegisterUserDto dto)
    {
        if (dto.Password != dto.PasswordConfirm)
        {
            return new BaseResult<UserDto>()
            {
                ErrorMessage = "Пароли не равны",
                ErrorCode = 21
            };
        }

        var user = await _userRepository
                .GetAll()
                .FirstOrDefaultAsync(x => x.Login == dto.Login);

        if (user != null)
        {
            return new BaseResult<UserDto>()
            {
                ErrorMessage = "Пользователь с такими данными уже существует",
                ErrorCode = 12
            };
        }

        var hashUserPassword = HashPassword(dto.Password);

        using (var transaction = await _unitofWorkRepository.BeginTransactionAsync())
        {
            try
            {
                user = new User()
                {
                    Login = dto.Login,
                    Password = hashUserPassword,
                };
                await _unitofWorkRepository.Users.CreateAsync(user);
                await _unitofWorkRepository.SaveChangesAsync();

                var role = await _roleRepository.GetAll().FirstOrDefaultAsync(x => x.Name == nameof(Roles.Admin));
                if (role == null)
                {
                    return new BaseResult<UserDto>
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
                await _unitofWorkRepository.UserRoles.CreateAsync(userRole);
                await _unitofWorkRepository.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
            }
        }

        return new BaseResult<UserDto>()
        {
            Data = _mapper.Map<UserDto>(user)
        };
    }

    public async Task<BaseResult<TokenDto>> Login(LoginUserDto dto)
    {
        var user = await _userRepository
                .GetAll()
                .Include(r => r.Roles)
                .FirstOrDefaultAsync(x => x.Login == dto.Login);
        if (user == null)
        {
            return new BaseResult<TokenDto>()
            {
                ErrorMessage = "Пользователь с такими данными не найден",
                ErrorCode = 11
            };
        }

        if (!IsVerifyPassword(user.Password, dto.Password))
        {
            return new BaseResult<TokenDto>()
            {
                ErrorMessage = "Неверный пароль",
                ErrorCode = 22
            };
        }

        var userToken = await _userTokenRepository.GetAll().FirstOrDefaultAsync(x => x.UserId == user.Id);

        var userRoles = user.Roles;
        var claims = userRoles.Select(r => new Claim(ClaimTypes.Role, r.Name)).ToList();
        claims.Add(new Claim(ClaimTypes.Name, user.Login));

        var accessToken = _tokenService.GenerateAccessToken(claims);
        var refreshToken = _tokenService.GenerateRefreshToken();
        if (userToken == null)
        {
            userToken = new UserToken()
            {
                UserId = user.Id,
                RefreshToken = refreshToken,
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7)
            };

            await _userTokenRepository.CreateAsync(userToken);
            await _userTokenRepository.SaveChangesAsync();
        }
        else
        {
            userToken.RefreshToken = refreshToken;
            userToken.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            _userTokenRepository.Update(userToken);
            await _userRepository.SaveChangesAsync();
        }

        return new BaseResult<TokenDto>()
        {
            Data = new TokenDto()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            }
        };
    }

    private string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }

    private bool IsVerifyPassword(string userPasswordHash, string password)
    {
        var hash = HashPassword(password);
        return userPasswordHash == hash;
    }
}
