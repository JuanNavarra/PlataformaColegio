using Colegio.Data;
using Colegio.Models;
using Colegio.Services.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Colegio.Services
{
    public class TokenProvider : ITokenProvider
    {
        private readonly ColegioContext contexto;
        public TokenProvider(ColegioContext contexto)
        {
            this.contexto = contexto;
        }
        public ClaimsIdentity LoginUser(string usuario, string contrasena)
        {
            try
            {
                var user = contexto.Col_Usuarios.Where(w => w.Usuario.Equals(usuario)).FirstOrDefault();
                if (user == null)
                    return null;
                if (contrasena == user.Contrasena)
                {
                    //Authentication successful, Issue Token with user credentials 
                    //Provide the security key which is given in 
                    //Startup.cs ConfigureServices() method 
                    var key = Encoding.ASCII.GetBytes
                    ("YourKey-2374-OFFKDI940NG7:56753253-tyuw-5769-0921-kfirox29zoxv");
                    //Generate Token for user 
                    var JWToken = new JwtSecurityToken(
                        issuer: "http://localhost:45092/",
                        audience: "http://localhost:45092/",
                        claims: GetUserClaims(user),
                        notBefore: new DateTimeOffset(DateTime.Now).DateTime,
                        expires: new DateTimeOffset(DateTime.Now.AddDays(1)).DateTime,
                        //Using HS256 Algorithm to encrypt Token  
                        signingCredentials: new SigningCredentials
                        (new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                    );
                    string token = new JwtSecurityTokenHandler().WriteToken(JWToken);
                    var claimsIdentity = new ClaimsIdentity(GetUserClaims(user), token);

                    return claimsIdentity;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        private IEnumerable<Claim> GetUserClaims(Col_Usuarios user)
        {
            IEnumerable<Claim> claims = new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Usuario),
                new Claim("Usuario", user.Usuario),
            };
            return claims;
        }
    }
}
