using JWTAuthentication_with_Refresh_Token.Models;
using Microsoft.AspNetCore.Mvc;

namespace JWTAuthentication_with_Refresh_Token.Abstract
{
    public interface IUserService
    {
        User Login([FromBody] User user);
    }
}
