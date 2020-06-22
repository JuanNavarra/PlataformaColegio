using Colegio.Data;
using Colegio.Models;
using Colegio.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Colegio.Services
{
    public class TokenProvider : ITokenProvider
    {
        private readonly ColegioContext contexto;
        public TokenProvider(ColegioContext contexto)
        {
            this.contexto = contexto;
        }

        /// <summary>
        /// Pregunta por un usuario existente, verifica la contraseña encriptada, genera codigo token que 
        /// expira en un dia, este token servira para la url local
        /// </summary>
        /// <param name="usuario">Nombre del usuario</param>
        /// <param name="contrasena">Contraseña del usuario</param>
        /// <returns>Objeto tipo ClaimsIdentity</returns>
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
            #region catch
            catch (DbEntityValidationException e)
            {
                string err = "";
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        err += ve.ErrorMessage;
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                return null;
            }

            catch (Exception e)
            {
                string err = "";
                if (e.InnerException != null)
                {
                    if (e.InnerException.Message != null)
                    {
                        err = (e.InnerException.Message);
                        if (e.InnerException.InnerException != null)
                        {
                            err += e.InnerException.InnerException.Message;
                        }
                    }
                }
                else
                {
                    err = (e.Message);
                }
                return null;
            }
            #endregion

        }

        /// <summary>
        /// Crea los Clamis para generar el jwt
        /// </summary>
        /// <param name="user">Objeto de tipo Col_Usuarios</param>
        /// <returns></returns>
        private IEnumerable<Claim> GetUserClaims(Col_Usuarios user)
        {
            IEnumerable<Claim> claims = new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Usuario),
                new Claim("Usuario", user.Usuario),
                new Claim("Id", user.Id.ToString())
            };
            return claims;
        }

        /// <summary>
        /// Metodo static que encripta la contraseña a SHA256
        /// </summary>
        /// <param name="str">Parametro tipo string que se desea encriptar</param>
        /// <returns>Cadena encriptada</returns>
        public static string SHA256(string contraseña)
        {
            SHA256 sha256 = SHA256Managed.Create();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] stream = null;
            StringBuilder sb = new StringBuilder();
            stream = sha256.ComputeHash(encoding.GetBytes(contraseña));
            for (int i = 0; i < stream.Length; i++) sb.AppendFormat("{0:x2}", stream[i]);
            return sb.ToString();
        }
    }
}
