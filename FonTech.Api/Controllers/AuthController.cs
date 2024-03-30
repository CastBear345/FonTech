using FonTech.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using FonTech.Application.Services;
using Microsoft.AspNetCore.Mvc;
using FonTech.Domain.DTO.User;
using FonTech.Domain.Entities;
using FonTech.Domain.Result;
using FonTech.Domain.DTO;
using Asp.Versioning;

namespace FonTech.Api.Controllers;

/// <summary>
/// 
/// </summary>
[ApiController]
public class AuthController : Controller
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Регистрация пользователя
    /// </summary>
    /// <param name="dto"></param>
    /// <remarks>
    /// Request for register user:
    /// 
    ///     POST
    ///     {
    ///        "Login": "ZachKing",
    ///        "Password": "123456",
    ///        "PasswordConfirm": "123456"
    ///     }
    ///     
    /// </remarks>
    /// <response code="200">Если регистрация пользователя прошла</response>
    /// <response code="400">Если регистрация пользователя не прошла</response>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BaseResult<UserDto>>> Register([FromBody]RegisterUserDto dto)
    {
        var response = await _authService.Register(dto);

        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }

    /// <summary>
    /// Аутентификация пользователя
    /// </summary>
    /// <param name="dto"></param>
    /// <remarks>
    /// Request for login user:
    /// 
    ///     POST
    ///     {
    ///        "Login": "ZachKing",
    ///        "Password": "123456"
    ///     }
    ///     
    /// </remarks>
    /// <response code="200">Если аутентификация пользователя прошла</response>
    /// <response code="400">Если аутентификация пользователя не прошла</response>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BaseResult<TokenDto>>> LogIn([FromBody]LoginUserDto dto)
    {
        var response = await _authService.Login(dto);

        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }
}
