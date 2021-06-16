using JWTAuthentication_with_Refresh_Token.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace JWTAuthentication_with_Refresh_Token.Abstract
{
    public class JWTService : IJWTService
    {
        //Normally it should be stored in db for each user. It just an example not more :)
        public static string RefreshToken = "";

        private readonly IConfiguration _config;

        public JWTService(IConfiguration config)
        {
            _config = config;
        }

        public Token GenerateJSONWebToken(IEnumerable<Claim> claims)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var exprationDateTime = DateTime.Now.AddMinutes(1);

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                                             _config["Jwt:Issuer"],
                                             claims,
                                             expires: exprationDateTime,
                                             signingCredentials: credentials);

            var refreshToken = GenerateRefreshToken();
            var JWTToken = new Token()
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                AccessTokenExpiration = exprationDateTime,
                RefreshToken = refreshToken
            };

            RefreshToken = refreshToken;

            return JWTToken;
        }

        public Token RefreshJSONWebToken(RefreshTokenRequest refreshTokenRequest) 
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _config["Jwt:Issuer"],
                ValidAudience = _config["Jwt:Issuer"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]))
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(refreshTokenRequest.AccessToken, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            var username = principal.Identity.Name;
            if (refreshTokenRequest.RefreshToken != RefreshToken)
                throw new SecurityTokenException("Invalid refresh token");

            return GenerateJSONWebToken(principal.Claims);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
    }
}