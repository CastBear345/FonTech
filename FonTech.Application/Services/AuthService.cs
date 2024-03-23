using AutoMapper;
using FonTech.Domain.DTO;
using FonTech.Domain.DTO.Report;
using FonTech.Domain.DTO.User;
using FonTech.Domain.Entities;
using FonTech.Domain.Interfaces.Repositories;
using FonTech.Domain.Interfaces.Services;
using FonTech.Domain.Result;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Buffers.Text;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace FonTech.Application.Services;

public class AuthService : IAuthService
{
    private readonly IBaseRepository<UserToken> _userTokenRepository;
    private readonly IBaseRepository<User> _userRepository;
    private readonly ITokenService _tokenService;
    private readonly ILogger _logger;
    private readonly IMapper _mapper;

    public AuthService(IBaseRepository<User> userRepository, ILogger logger, IMapper mapper, IBaseRepository<UserToken> userTokenRepository, ITokenService tokenService)
    {
        _userTokenRepository = userTokenRepository;
        _userRepository = userRepository;
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

        try
        {
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

            user = new User()
            {
                Login = dto.Login,
                Password = hashUserPassword,
            };

            await _userRepository.CreateAsync(user);
            return new BaseResult<UserDto>()
            {
                Data = _mapper.Map<UserDto>(user)
            };
        }
        catch (Exception ex)
        {
            _logger.Error(ex, ex.Message);
            return new BaseResult<UserDto>
            {
                ErrorMessage = "Внутренняя ошибка сервера",
                ErrorCode = 10,
            };
        }
    }

    public async Task<BaseResult<TokenDto>> Login(LoginUserDto dto)
    {
        try
        {
            var user = await _userRepository
                .GetAll()
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
                    ErrorCode = 13
                };
            }

            var userToken = await _userTokenRepository.GetAll().FirstOrDefaultAsync(x => x.UserId == user.Id);

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.Login),
                new Claim(ClaimTypes.Role, "User")
            };
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
            }
            else
            {
                userToken.RefreshToken = refreshToken;
                userToken.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

                await _userTokenRepository.UpdateAsync(userToken);
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
        catch (Exception ex)
        {
            _logger.Error(ex, ex.Message);
            return new BaseResult<TokenDto>
            {
                ErrorMessage = "Внутренняя ошибка сервера",
                ErrorCode = 10,
            };
        }
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
