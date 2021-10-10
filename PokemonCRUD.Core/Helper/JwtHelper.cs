using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace PokemonCRUD.Core.Helper
{
    public class JwtHelper
    {
        private const string SecretKey = "thisIsASecureKeyOfAtLeast12Characters";
        public static readonly SymmetricSecurityKey SigningKey =
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));

        public static string GenerateJwtToken()
        {
            var credentials = new SigningCredentials(SigningKey
                , SecurityAlgorithms.HmacSha256);

            var header = new JwtHeader(credentials);

            DateTime expire = DateTime.UtcNow.AddMinutes(60);
            int ts = (int)(expire - new DateTime(1970, 1, 1)).TotalSeconds;

            var payload = new JwtPayload
            {
                { "sub", "testSubject" },
                { "Name", "Roberto"},
                { "email", "roberto.pozo.andrade@gmail.com" },
                { "exp", ts},
                { "iss", "http://localhost:58933"},
                { "aud", "http://localhost:58933"}
            };

            var secToken = new JwtSecurityToken(header, payload);

            var handler = new JwtSecurityTokenHandler();

            var tokenString = handler.WriteToken(secToken);

            Console.WriteLine(tokenString);

            return tokenString;
        }
    }
}
