using FonTech.Domain.DTO;
using FonTech.Domain.Result;
using System.Security.Claims;

namespace FonTech.Domain.Interfaces.Services;

public interface ITokenService
{
    string GenerateAccessToken(IEnumerable<Claim> claims);

    string GenerateRefreshToken();

    ClaimsPrincipal GetPrincipalFromExpiredToken(string accessToken);

    Task<BaseResult<TokenDto>> RefreshToken(TokenDto dto);
}
