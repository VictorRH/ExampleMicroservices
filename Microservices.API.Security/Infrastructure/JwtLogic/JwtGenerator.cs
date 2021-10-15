using Microservices.API.Security.Core.Entities;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Microservices.API.Security.Infrastructure.JwtLogic
{
    public class JwtGenerator : IJwtGenerator
    {


        public string CreateToken(Users users,List<string> rols )
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, users.UserName)
            };
            if (rols != null)
            {
                foreach (var rol in rols)
                {
                    claims.Add(new Claim(ClaimTypes.Role, rol));
                }

            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("this is secret password for jwt in example microservices"));
            var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(3),
                SigningCredentials = credential
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescription);

            return tokenHandler.WriteToken(token);
        }
    }
}
