using JWTAuthentication_with_Refresh_Token.Models;
using Microsoft.AspNetCore.Mvc;

namespace JWTAuthentication_with_Refresh_Token.Abstract
{
    public class UserService : IUserService
    {
        public User Login([FromBody] User user)
        {
            User _user = default;

            if (user.Username == "ilyas")
            {
                _user = new User { Username = "İlyas Varol", Password = "ilyas.varol@outlook.com.tr" };
            }

            return _user;
        }
    }
}
