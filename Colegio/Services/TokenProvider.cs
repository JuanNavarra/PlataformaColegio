using Colegio.Data;
using Colegio.Models.ModelHelper;
using Colegio.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Colegio.Services
{
    public class TokenProvider : ITokenProvider
    {
        private readonly ColegioContext contexto;

        public IConfiguration Configuration { get; }
        public TokenProvider(ColegioContext contexto, IConfiguration configuration)
        {
            this.contexto = contexto;
            Configuration = configuration;
        }

        private UsuariosPerfiles MapToValue(SqlDataReader reader)
        {
            return new UsuariosPerfiles()
            {
                PermisosModulo = reader["Value2"].ToString(),
                PermisosSubModulo = reader["Value2"].ToString(),
                Modulos = reader["Value2"].ToString(),
                SubModulos = reader["Value2"].ToString(),
            };
        }

        /// <summary>
        /// Pregunta por un usuario existente, verifica la contraseña encriptada, genera codigo token que 
        /// expira en un dia, este token servira para la url local
        /// </summary>
        /// <param name="usuario">Nombre del usuario</param>
        /// <param name="contrasena">Contraseña del usuario</param>
        /// <returns>Objeto tipo ClaimsIdentity</returns>
        public async Task<ClaimsIdentity> LoginUser(string usuario, string contrasena)
        {
            try
            {
                Models.Col_Usuarios user = await contexto.Col_Usuarios
                    .Where(w => w.Usuario.Equals(usuario) && w.Estado.Equals("A"))
                    .FirstOrDefaultAsync();
                if (user == null)
                    return null;
                if (SHA256(contrasena) == user.Contrasena)
                {
                    //List<UsuariosPerfiles> _usuario = new List<UsuariosPerfiles>();
                    //using (SqlConnection sql = new SqlConnection(Configuration.GetConnectionString("DefaultConnection")))
                    //{
                    //    using (SqlCommand cmd = new SqlCommand("LoginPermisos", sql))
                    //    {
                    //        cmd.CommandType = CommandType.StoredProcedure;
                    //        cmd.Parameters.Add(new SqlParameter("@Id", user.Id));
                    //        await sql.OpenAsync();

                    //        using (var reader = await cmd.ExecuteReaderAsync())
                    //        {
                    //            while (await reader.ReadAsync())
                    //            {
                    //                _usuario.Add(MapToValue(reader));
                    //            }
                    //        }
                    //    }
                    //}

                    List<UsuariosPerfiles> query = await (from t0 in contexto.Col_Roles
                                                          join t6 in contexto.Col_Usuarios on t0.RolId equals t6.RolId
                                                          join t1 in contexto.Col_RolModulos on t0.RolId equals t1.RolId
                                                          join t2 in contexto.Col_Modulos on t1.ModuloId equals t2.ModuloId
                                                          join t3 in contexto.Col_SubModuloModulo on new { t2.ModuloId, t0.RolId } equals new { t3.ModuloId, t3.RolId }
                                                          join t4 in contexto.Col_SubModulos on t3.SubModuloId equals t4.SubModuloId into ModSub
                                                          from t5 in ModSub.DefaultIfEmpty()
                                                          where t6.Id.Equals(user.Id)
                                                          select new UsuariosPerfiles
                                                          {
                                                              PermisosModulo = t1.PermisosCrud.Replace("\n", ""),
                                                              PermisosSubModulo = t3.PermisosCrud.Replace("\n", ""),
                                                              Modulos = t2.Nombre,
                                                              SubModulos = t5.Nombre,
                                                          }).ToListAsync();

                    if (query.Count() == 0)
                    {
                        query = await (from t0 in contexto.Col_Roles
                                       join t3 in contexto.Col_Usuarios on t0.RolId equals t3.RolId
                                       join t1 in contexto.Col_RolModulos on t0.RolId equals t1.RolId
                                       join t2 in contexto.Col_Modulos on t1.ModuloId equals t2.ModuloId
                                       where t3.Id.Equals(user.Id)
                                       select new UsuariosPerfiles
                                       {
                                           PermisosModulo = t1.PermisosCrud.Replace("\n", ""),
                                           Modulos = t2.Nombre
                                       }).ToListAsync();
                    }

                    //Authentication successful, Issue Token with user credentials 
                    //Provide the security key which is given in 
                    //Startup.cs ConfigureServices() method 
                    byte[] key = Encoding.ASCII.GetBytes
                    ("YourKey-2374-OFFKDI940NG7:56753253-tyuw-5769-0921-kfirox29zoxv");
                    //Generate Token for user 
                    List<Claim> claimsData = GetUserClaims(query, user.Usuario);
                    JwtSecurityToken JWToken = new JwtSecurityToken(
                        issuer: "http://localhost:45092/",
                        audience: "http://localhost:45092/",
                        claims: claimsData,
                        notBefore: new DateTimeOffset(DateTime.Now).DateTime,
                        expires: new DateTimeOffset(DateTime.Now.AddDays(1)).DateTime,
                        //Using HS256 Algorithm to encrypt Token  
                        signingCredentials: new SigningCredentials
                        (new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                    );
                    string token = new JwtSecurityTokenHandler().WriteToken(JWToken);
                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claimsData, token);
                    Models.Col_Usuarios _usuario = await contexto.Col_Usuarios.Where(w => w.Id.Equals(user.Id)).FirstOrDefaultAsync();
                    _usuario.UltimoLogin = DateTime.Now;
                    contexto.Col_Usuarios.Update(_usuario);
                    await contexto.SaveChangesAsync();
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
                foreach (DbEntityValidationResult eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (DbValidationError ve in eve.ValidationErrors)
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
        private List<Claim> GetUserClaims(List<UsuariosPerfiles> user, string nombre)
        {
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim("Usuario", nombre));
            foreach (UsuariosPerfiles item in user)
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
        public static string SHA256(string pass)
        {
            SHA256 sha256 = SHA256Managed.Create();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] stream = null;
            StringBuilder sb = new StringBuilder();
            stream = sha256.ComputeHash(encoding.GetBytes(pass));
            for (int i = 0; i < stream.Length; i++) sb.AppendFormat("{0:x2}", stream[i]);
            return sb.ToString();
        }
    }
}