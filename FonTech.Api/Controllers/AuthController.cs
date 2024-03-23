using Asp.Versioning;
using FonTech.Application.Services;
using FonTech.Domain.DTO;
using FonTech.Domain.DTO.User;
using FonTech.Domain.Entities;
using FonTech.Domain.Interfaces.Services;
using FonTech.Domain.Result;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    /// <returns></returns>
    [HttpPost("register")]
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
    /// Авторизация пользователя
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost("login")]
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
