using FonTech.Domain.Interfaces.Services;
using FonTech.Application.Services;
using Microsoft.AspNetCore.Mvc;
using FonTech.Domain.Result;
using FonTech.Domain.DTO;

namespace FonTech.Api.Controllers;

/// <summary>
/// 
/// </summary>
[ApiController]
public class TokenController : Controller
{
    private readonly ITokenService _tokenService;

    public TokenController(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }
    
    /// <summary>
    /// Обновление токена
    /// </summary>
    /// <param name="dto"></param>
    /// <remarks>
    /// Request for refresh token:
    /// 
    ///     POST
    ///     {
    ///        "AccessToken": "273gfbih8Gb8ifvhw87halokf09e..."
    ///        "RefreshToken": "tfgd7g73gerh8H37gfy"
    ///     }
    ///     
    /// </remarks>
    /// <response code="200">Если токен успешно обновлён</response>
    /// <response code="400">Если токен не был обновлён</response>
    [HttpPost]
    [Route("refresh")]
    public async Task<ActionResult<BaseResult<TokenDto>>> RefreshToken([FromBody] TokenDto dto)
    {
        var response = await _tokenService.RefreshToken (dto);

        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }
}
