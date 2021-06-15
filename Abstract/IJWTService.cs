
using System.Security.Claims;

namespace JWTAuthentication_with_Refresh_Token.Abstract
{
    public interface IJWTService
    {
        string GenerateJSONWebToken(Claim[] claims);
    }
}
