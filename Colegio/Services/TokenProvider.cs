using Colegio.Data;
using Colegio.Models.ModelHelper;
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
                var user = contexto.Col_Usuarios
                    .Where(w => w.Usuario.Equals(usuario) && w.Estado.Equals("A"))
                    .FirstOrDefault();
                if (user == null)
                    return null;
                if (contrasena == user.Contrasena)
                {
                    var query = (from t0 in contexto.Col_Roles
                                 join t6 in contexto.Col_Usuarios on t0.RolId equals t6.RolId
                                 join t1 in contexto.Col_RolModulos on t0.RolId equals t1.RolId
                                 join t2 in contexto.Col_Modulos on t1.ModuloId equals t2.ModuloId
                                 join t3 in contexto.Col_SubModuloModulo on new { t2.ModuloId, t0.RolId } equals new { t3.ModuloId, t3.RolId }
                                 join t4 in contexto.Col_SubModulos on t3.SubModuloId equals t4.SubModuloId into ModSub
                                 from t5 in ModSub.DefaultIfEmpty()
                                 where t6.Id.Equals(user.Id)
                                 select new UsuariosPerfiles
                                 {
                                     NombreUsuario = t6.Usuario,
                                     PermisosModulo = t1.PermisosCrud.Replace("\n", ""),
                                     PermisosSubModulo = t3.PermisosCrud.Replace("\n", ""),
                                     Modulos = t2.Nombre,
                                     SubModulos = t5.Nombre,
                                 }).ToList();

                    if (query.Count() == 0)
                    {
                        query = (from t0 in contexto.Col_Roles
                                 join t3 in contexto.Col_Usuarios on t0.RolId equals t3.RolId
                                 join t1 in contexto.Col_RolModulos on t0.RolId equals t1.RolId
                                 join t2 in contexto.Col_Modulos on t1.ModuloId equals t2.ModuloId
                                 where t3.Id.Equals(user.Id)
                                 select new UsuariosPerfiles
                                 {
                                     NombreUsuario = t3.Usuario,
                                     PermisosModulo = t1.PermisosCrud.Replace("\n", ""),
                                     Modulos = t2.Nombre
                                 }).ToList();
                    }

                    //Authentication successful, Issue Token with user credentials 
                    //Provide the security key which is given in 
                    //Startup.cs ConfigureServices() method 
                    var key = Encoding.ASCII.GetBytes
                    ("YourKey-2374-OFFKDI940NG7:56753253-tyuw-5769-0921-kfirox29zoxv");
                    //Generate Token for user 
                    var JWToken = new JwtSecurityToken(
                        issuer: "http://localhost:45092/",
                        audience: "http://localhost:45092/",
                        claims: GetUserClaims(query),
                        notBefore: new DateTimeOffset(DateTime.Now).DateTime,
                        expires: new DateTimeOffset(DateTime.Now.AddDays(1)).DateTime,
                        //Using HS256 Algorithm to encrypt Token  
                        signingCredentials: new SigningCredentials
                        (new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                    );
                    string token = new JwtSecurityTokenHandler().WriteToken(JWToken);
                    var claimsIdentity = new ClaimsIdentity(GetUserClaims(query), token);
                    var _usuario = contexto.Col_Usuarios.Where(w => w.Id.Equals(user.Id)).FirstOrDefault();
                    _usuario.UltimoLogin = DateTime.Now;
                    contexto.Col_Usuarios.Update(_usuario);
                    contexto.SaveChanges();
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
        private List<Claim> GetUserClaims(List<UsuariosPerfiles> user)
        {
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim("Usuario", user.Select(s => s.NombreUsuario).FirstOrDefault()));
            foreach (var item in user)
            {
                claims.Add(new Claim("PermisoModulo", $"{item.Modulos}-{item.PermisosModulo ?? "0"}"));
                claims.Add(new Claim("PermisoSubModulo", $"{item.Modulos}-{item.PermisosSubModulo ?? "0"}-{item.SubModulos ?? "0"}"));
            }
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