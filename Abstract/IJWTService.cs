using JWTAuthentication_with_Refresh_Token.Models;
using System.Collections.Generic;
using System.Security.Claims;

namespace JWTAuthentication_with_Refresh_Token.Abstract
{
    public interface IJWTService
    {
        Token GenerateJSONWebToken(IEnumerable<Claim> claims);
        Token RefreshJSONWebToken(RefreshTokenRequest refreshTokenRequest);
    }
}
