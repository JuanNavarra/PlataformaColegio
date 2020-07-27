using Colegio.Data;
using Colegio.Models;
using Colegio.Models.ModelHelper;
using Colegio.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Colegio.Services
{
    public class PerfilesService : IPerfilesService
    {
        private readonly ColegioContext context;
        public PerfilesService(ColegioContext context)
        {
            this.context = context;
        }

        public async Task<List<Col_Modulos>> CargarModulos()
        {

            var modulos = await context.Col_Modulos
                .Where(w => w.Estado.Equals("A"))
                .Select(s => new Col_Modulos
                {
                    ModuloId = s.ModuloId,
                    Nombre = s.Nombre
                }).ToListAsync();
            return modulos;
        }

        public async Task<List<Col_SubModulos>> CargarSubModulos(int[] modulos)
        {
            var subModulos = await context.Col_SubModulos
                .Where(w => w.Estado.Equals("A") && modulos.Contains(w.ModuloId))
                .Select(s => new Col_SubModulos
                {
                    Nombre = s.Nombre,
                    SubModuloId = s.SubModuloId,
                    Descripcion = context.Col_Modulos
                        .Where(w => w.ModuloId == s.ModuloId)
                        .Select(s => s.Nombre).FirstOrDefault(),
                    ModuloId = s.ModuloId
                })
                .ToListAsync();
            return subModulos;
        }

        public async Task<List<Col_Roles>> MostrarAutorizaciones()
        {
            try
            {
                var query = await context.Col_Roles
                    .Select(s => new Col_Roles
                    {
                        NombreRol = s.NombreRol,
                        RolId = s.RolId,
                        FechaActualizacion = s.FechaActualizacion,
                        FechaCreacion = s.FechaCreacion,
                        Estado = s.Estado == "A" ? "ACTIVO" : "INACTIVO",
                        Descripcion = s.Descripcion.Length > 45 ? $"{s.Descripcion.Substring(0, 45)}..." : s.Descripcion,
                        UltimoLogin = context.Col_Usuarios
                                .Where(w => w.Estado.Equals("A") && w.RolId.Equals(s.RolId))
                                .OrderByDescending(o => o.UltimoLogin)
                                .Select(s => s.UltimoLogin).FirstOrDefault()
                    }).ToListAsync();

                return query
                    .OrderByDescending(o => o.FechaActualizacion)
                    .ThenByDescending(t => t.FechaCreacion)
                    .ToList();
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

        private async Task<Col_Roles> VerDetalleRol(int idRol)
        {
            try
            {
                var query = await context.Col_Roles
                    .Where(w => w.RolId.Equals(idRol))
                    .Select(s => new Col_Roles
                    {
                        NombreRol = s.NombreRol,
                        FechaActualizacion = s.FechaActualizacion,
                        FechaCreacion = s.FechaCreacion,
                        Estado = s.Estado == "A" ? "ACTIVO" : "INACTIVO",
                        Descripcion = s.Descripcion ?? "",
                        UltimoLogin = context.Col_Usuarios
                                    .Where(w => w.Estado.Equals("A") && w.RolId.Equals(s.RolId))
                                    .OrderByDescending(o => o.UltimoLogin)
                                    .Select(s => s.UltimoLogin).FirstOrDefault()
                    }).FirstOrDefaultAsync();
                return query;
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

        private async Task<List<ModulosToSubModulos>> VerDetalleModulos(int idRol)
        {
            try
            {
                List<ModulosToSubModulos> relaciones = new List<ModulosToSubModulos>();

                var query = await (from t0 in context.Col_Roles
                                   join t1 in context.Col_RolModulos on t0.RolId equals t1.RolId
                                   join t2 in context.Col_Modulos on t1.ModuloId equals t2.ModuloId
                                   join t3 in context.Col_SubModuloModulo on new { t2.ModuloId, t0.RolId } equals new { t3.ModuloId, t3.RolId }
                                   join t4 in context.Col_SubModulos on t3.SubModuloId equals t4.SubModuloId into ModSub
                                   from t5 in ModSub.DefaultIfEmpty()
                                   where t0.RolId.Equals(idRol)
                                   select new ModulosToSubModulos
                                   {
                                       NombreRol = t0.NombreRol,
                                       NombreModulo = t2.Nombre,
                                       NombreSubModulo = t5.Nombre,
                                       ModuloId = t2.ModuloId,
                                       PermisoSubModulo = t3.PermisosCrud,
                                       PermisoModulo = t1.PermisosCrud,
                                       SubModuloId = t5.SubModuloId
                                   }).ToListAsync();

                if (query.Count() == 0)
                {
                    query = await (from t0 in context.Col_Roles
                                   join t1 in context.Col_RolModulos on t0.RolId equals t1.RolId
                                   join t2 in context.Col_Modulos on t1.ModuloId equals t2.ModuloId
                                   where t0.RolId.Equals(idRol)
                                   select new ModulosToSubModulos
                                   {
                                       NombreRol = t0.NombreRol,
                                       NombreModulo = t2.Nombre,
                                       ModuloId = t2.ModuloId,
                                       PermisoModulo = t1.PermisosCrud
                                   }).ToListAsync();
                }

                foreach (var item in query)
                {
                    ModulosToSubModulos relacion = new ModulosToSubModulos();
                    if (!relaciones.Where(w => w.NombreModulo == item.NombreModulo).Any())
                    {
                        List<ModulosToSubModulos> _relaciones = new List<ModulosToSubModulos>();
                        List<string> _permisos = new List<string>();
                        relacion.NombreModulo = item.NombreModulo;
                        relacion.ModuloId = item.ModuloId;
                        relacion.NombreRol = item.NombreRol;
                        if (item.PermisoModulo != null)
                        {
                            string reemplazarModulo = item.PermisoModulo.Replace('\n', ' ');
                            List<string> permisosModulo = reemplazarModulo.Split(',').ToList();
                            foreach (var temp in permisosModulo)
                            {
                                _permisos.Add(temp);
                            }
                        }
                        else
                        {
                            foreach (var temp in query.Where(w => w.NombreModulo == item.NombreModulo && w.NombreSubModulo != null).ToList())
                            {
                                ModulosToSubModulos _relacion = new ModulosToSubModulos();
                                string reemplazarSubModulo = temp.PermisoSubModulo.Replace('\n', ' ');
                                List<string> permisosSubModulo = reemplazarSubModulo.Split(',').ToList();
                                _relacion.NombreSubModulo = temp.NombreSubModulo;
                                _relacion.Permisos = permisosSubModulo;
                                _relacion.ModuloId = temp.ModuloId;
                                _relacion.SubModuloId = temp.SubModuloId;
                                _relaciones.Add(_relacion);
                            }
                        }
                        relacion.Permisos = _permisos;
                        relacion.Relaciones = _relaciones;
                        relaciones.Add(relacion);
                    }
                }
                return relaciones;
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

        private async Task<List<UsuariosPerfiles>> VerListadoUsuariosPerfil(int rolId)
        {
            try
            {
                var query = await (from t0 in context.Col_Roles
                                   join t1 in context.Col_Usuarios on t0.RolId equals t1.RolId
                                   join t2 in context.Col_Personas on t1.Id equals t2.UsuarioId
                                   where t0.RolId.Equals(rolId) && t0.Estado.Equals("A")
                                   select new UsuariosPerfiles
                                   {
                                       Estado = t1.Estado == "A" ? "ACTIVO" : t1.Estado == "P" ? "PENDIENTE" : "INACTIVO",
                                       NombreUsuario = t1.Usuario,
                                       NombrePersona = t2.PrimerNombre + " " + t2.PrimerApellido + " " + t2.SegundoApellido,
                                       FechaCreacion = t1.FechaCreacion,
                                       FechaActualizacion = t2.FechaActualizacion,
                                       UltimoLogin = t1.UltimoLogin
                                   }).ToListAsync();
                return query;
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

        public async Task<PerfilesViewModel> MostrarDetallePerfil(int rolId)
        {
            PerfilesViewModel perfiles = new PerfilesViewModel();

            perfiles.Roles = await VerDetalleRol(rolId);
            perfiles.Modulos = await VerDetalleModulos(rolId);
            perfiles.Usuarios = await VerListadoUsuariosPerfil(rolId);
            return perfiles;
        }

        public async Task<ApiCallResult> EliminarPerfiles(int rolId, bool op)
        {
            try
            {
                bool status = false;
                string title = "Error";
                string message = "Error al eliminar el perfil";

                var query = await context.Col_Roles
                    .Join(context.Col_Usuarios,
                        r => r.RolId,
                        u => u.RolId,
                        (r, u) => new { Col_Roles = r, Col_Usuarios = u })
                    .Where(w => w.Col_Roles.RolId.Equals(rolId)).CountAsync();
                var usuario = await context.Col_Usuarios.Where(w => w.RolId.Equals(rolId)).ToListAsync();
                var rol = await context.Col_Roles.Where(w => w.RolId.Equals(rolId)).FirstOrDefaultAsync();
                if (op)
                {
                    var subModulo = await context.Col_SubModuloModulo.Where(w => w.RolId.Equals(rolId)).ToListAsync();
                    var modulo = await context.Col_RolModulos.Where(w => w.RolId.Equals(rolId)).ToListAsync();
                    if (query > 0)
                    {
                        context.Col_Usuarios.RemoveRange(usuario);
                    }
                    if (subModulo.Count() > 0)
                    {
                        context.Col_SubModuloModulo.RemoveRange(subModulo);
                    }
                    context.Col_RolModulos.RemoveRange(modulo);
                    context.Col_Roles.Remove(rol);
                    status = true;
                    title = "Éxito";
                    message = "El perfil se ha eliminado exitosamente";
                }
                else
                {
                    if (query > 0)
                    {
                        foreach (var item in usuario)
                        {
                            item.FechaActualizacion = DateTime.Now;
                            item.Estado = "I";
                            context.Col_Usuarios.Update(item);
                        }
                    }
                    rol.Estado = "I";
                    rol.FechaActualizacion = DateTime.Now;
                    context.Col_Roles.Update(rol);
                    status = true;
                    title = "Éxito";
                    message = $"El perfil {rol.NombreRol} se ha inhabilitado exitosamente";
                }

                await context.SaveChangesAsync();

                return new ApiCallResult { Status = status, Title = title, Message = message };
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
                return new ApiCallResult { Status = false, Title = "Error al eliminar", Message = "Favor contacte éste con el administrador" };
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
                return new ApiCallResult { Status = false, Title = "Error al eliminar", Message = "Favor contacte éste con el administrador" };
            }
            #endregion  
        }

        public async Task<ApiCallResult> GuardarAutorizaciones(List<Col_RolModulos> modulo, List<Col_SubModuloModulo> subModulo, string rolNombre, string descripcion)
        {
            try
            {
                bool status = false;
                string title = "Error";
                string message = "Ya existe un rol con este nombre";

                var existeNombre = await context.Col_Roles.Where(w => w.NombreRol.Equals(rolNombre.ToUpper())).AnyAsync();
                if (!existeNombre)
                {
                    Col_Roles rol = new Col_Roles();
                    List<Col_RolModulos> rolModulos = new List<Col_RolModulos>();
                    List<Col_SubModuloModulo> subModuloModulos = new List<Col_SubModuloModulo>();

                    status = true;
                    title = "Proceso exitoso";
                    message = "Los datos fueron guardados correctamente en la plataforma";

                    int? maxId = await context.Col_Roles.MaxAsync(m => (int?)m.RolId);
                    int? id = maxId == null ? 1 : maxId + 1;
                    rol.Estado = "A";
                    rol.FechaActualizacion = null;
                    rol.FechaCreacion = DateTime.Now;
                    rol.NombreRol = rolNombre.ToUpper();
                    rol.RolId = Convert.ToInt32(id);
                    rol.Descripcion = descripcion;
                    rol.Restringir = subModulo.Count() > 0 ? true : false;
                    await context.AddAsync<Col_Roles>(rol);

                    maxId = await context.Col_RolModulos.MaxAsync(m => (int?)m.Id);
                    id = maxId == null ? 1 : maxId + 1;
                    foreach (var item in modulo)
                    {
                        Col_RolModulos _rolModulo = new Col_RolModulos();
                        _rolModulo.Id = Convert.ToInt32(id);
                        _rolModulo.RolId = rol.RolId;
                        _rolModulo.ModuloId = item.ModuloId;
                        _rolModulo.PermisosCrud = item.PermisosCrud?.Trim();
                        rolModulos.Add(_rolModulo);
                        id++;
                    }
                    await context.AddRangeAsync(rolModulos);
                    if (subModulo.Count() > 0)
                    {
                        maxId = await context.Col_SubModuloModulo.MaxAsync(m => (int?)m.Id);
                        id = maxId == null ? 1 : maxId + 1;
                        var queryModulo = modulo.Where(w => !subModulo.Where(s => s.ModuloId == w.ModuloId).Any()).ToList();
                        foreach (var item in queryModulo)
                        {
                            Col_SubModuloModulo _subModulo = new Col_SubModuloModulo();
                            _subModulo.Id = Convert.ToInt32(id);
                            _subModulo.ModuloId = item.ModuloId;
                            _subModulo.SubModuloId = null;
                            _subModulo.RolId = rol.RolId;
                            _subModulo.PermisosCrud = item.PermisosCrud.Trim();
                            subModuloModulos.Add(_subModulo);
                            id++;
                        }

                        foreach (var item in subModulo)
                        {
                            Col_SubModuloModulo _subModulo = new Col_SubModuloModulo();
                            _subModulo.Id = Convert.ToInt32(id);
                            _subModulo.ModuloId = item.ModuloId;
                            _subModulo.SubModuloId = item.SubModuloId;
                            _subModulo.RolId = rol.RolId;
                            _subModulo.PermisosCrud = item.PermisosCrud.Trim();
                            subModuloModulos.Add(_subModulo);
                            id++;
                        }
                        await context.AddRangeAsync(subModuloModulos);
                    }
                    await context.SaveChangesAsync();
                }

                return new ApiCallResult
                {
                    Status = status,
                    Title = title,
                    Message = message
                };
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
                return new ApiCallResult { Status = false, Title = "Error al guardar", Message = "Favor contacte éste con el administrador" };
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
                return new ApiCallResult { Status = false, Title = "Error al guardar", Message = "Favor contacte éste con el administrador" };
            }
            #endregion
        }

        public async Task<ModulosSelect> CargaDatosActualizar(int rolId)
        {
            try
            {
                ModulosSelect select = new ModulosSelect();

                #region Primer Tab
                var query = await (from t0 in context.Col_Roles
                                   join t1 in context.Col_RolModulos on t0.RolId equals t1.RolId
                                   join t2 in context.Col_Modulos on t1.ModuloId equals t2.ModuloId
                                   select new ModulosSelect { ModuloId = t2.ModuloId, Nombre = t2.Nombre, RolId = t0.RolId }).ToListAsync();

                select.Rol = await context.Col_Roles.Where(w => w.RolId.Equals(rolId)).FirstOrDefaultAsync();
                select.Modulos = query.Where(w => w.RolId.Equals(rolId)).ToList();

                List<ModulosSelect> subModulos = new List<ModulosSelect>();
                foreach (var grupo in query.GroupBy(g => g.ModuloId))
                {
                    ModulosSelect subModulo = new ModulosSelect();
                    subModulo.ModuloId = grupo.Select(s => s.ModuloId).FirstOrDefault();
                    subModulo.RolId = grupo.Select(s => Convert.ToInt32(s.Rol)).FirstOrDefault();
                    subModulo.Nombre = grupo.Select(s => s.Nombre).FirstOrDefault();
                    subModulos.Add(subModulo);
                }
                select.AllModulos = subModulos.Where(w => !select.Modulos.Where(s => s.ModuloId == w.ModuloId).Any()).ToList();
                #endregion

                #region Segundo Tab
                select.Seleccionados = await (from t0 in context.Col_Roles
                                              join t1 in context.Col_SubModuloModulo on t0.RolId equals t1.RolId
                                              join t2 in context.Col_SubModulos on t1.SubModuloId equals t2.SubModuloId
                                              join t3 in context.Col_Modulos on t2.ModuloId equals t3.ModuloId
                                              where t0.RolId.Equals(rolId)
                                              select new ModulosSelect
                                              {
                                                  Nombre = t2.Nombre,
                                                  SubModuloId = t2.SubModuloId,
                                                  ModuloId = t2.ModuloId,
                                                  Descripcion = t3.Nombre
                                              }).ToListAsync();

                var AllSubModulos = await (from t0 in context.Col_Roles
                                           join t1 in context.Col_RolModulos on t0.RolId equals t1.RolId
                                           join t2 in context.Col_Modulos on t1.ModuloId equals t2.ModuloId
                                           join t3 in context.Col_SubModulos on t2.ModuloId equals t3.ModuloId
                                           where t0.RolId.Equals(rolId)
                                           select new ModulosSelect
                                           {
                                               Nombre = t3.Nombre,
                                               SubModuloId = t3.SubModuloId,
                                               ModuloId = t2.ModuloId,
                                               Descripcion = t2.Nombre
                                           }).ToListAsync();

                select.NoSeleccionados = AllSubModulos.Where(w => !select.Seleccionados.Where(m => m.SubModuloId == w.SubModuloId).Any()).ToList();
                #endregion

                #region Tercer Tab
                select.Permisos = await VerDetalleModulos(rolId);
                #endregion

                return select;
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

        public async Task<ApiCallResult> ActualizarAutorizaciones(List<Col_RolModulos> modulos, List<Col_SubModuloModulo> subModulos, string rolNombre, string descripcion, int idRol)
        {
            try
            {
                bool status = false;
                string title = "Error";
                string message = "Ya existe un rol con este nombre";

                var existeNombre = await context.Col_Roles.Where(w => w.NombreRol.Equals(rolNombre.ToUpper()) && w.RolId != idRol).AnyAsync();

                if (!existeNombre)
                {
                    status = true;
                    title = "Proceso exitoso";
                    message = "Los datos fueron actualizados correctamente en la plataforma";

                    #region Roles
                    var _rol = await context.Col_Roles.Where(w => w.RolId.Equals(idRol)).FirstOrDefaultAsync();
                    _rol.NombreRol = rolNombre.ToUpper();
                    _rol.Restringir = subModulos.Count() > 0 ? true : false;
                    _rol.FechaActualizacion = DateTime.Now;
                    _rol.Descripcion = descripcion;
                    context.Col_Roles.Update(_rol);
                    #endregion

                    #region Modulos
                    var _rolModulos = await context.Col_RolModulos.Where(w => w.RolId.Equals(idRol)).ToListAsync();
                    context.Col_RolModulos.RemoveRange(_rolModulos);
                    var maxId = await context.Col_RolModulos.MaxAsync(m => (int?)m.Id);
                    var id = maxId == null ? 1 : maxId + 1;
                    List<Col_RolModulos> rolModulos = new List<Col_RolModulos>();

                    foreach (var item in modulos)
                    {
                        Col_RolModulos _rolModulo = new Col_RolModulos();
                        _rolModulo.Id = Convert.ToInt32(id);
                        _rolModulo.RolId = _rol.RolId;
                        _rolModulo.ModuloId = item.ModuloId;
                        _rolModulo.PermisosCrud = item.PermisosCrud?.Trim();
                        rolModulos.Add(_rolModulo);
                        id++;
                    }
                    await context.AddRangeAsync(rolModulos);
                    #endregion

                    #region SubModulos
                    var _subModulos = await context.Col_SubModuloModulo.Where(w => w.RolId.Equals(idRol)).ToListAsync();
                    context.Col_SubModuloModulo.RemoveRange(_subModulos);
                    if (subModulos.Count() > 0)
                    {
                        maxId = await context.Col_SubModuloModulo.MaxAsync(m => (int?)m.Id);
                        id = maxId == null ? 1 : maxId + 1;
                        var queryModulo = modulos.Where(w => !subModulos.Where(s => s.ModuloId == w.ModuloId).Any()).ToList();
                        List<Col_SubModuloModulo> subModuloModulos = new List<Col_SubModuloModulo>();

                        foreach (var item in queryModulo)
                        {
                            Col_SubModuloModulo _subModulo = new Col_SubModuloModulo();
                            _subModulo.Id = Convert.ToInt32(id);
                            _subModulo.ModuloId = item.ModuloId;
                            _subModulo.SubModuloId = null;
                            _subModulo.RolId = _rol.RolId;
                            _subModulo.PermisosCrud = item.PermisosCrud.Trim();
                            subModuloModulos.Add(_subModulo);
                            id++;
                        }

                        foreach (var item in subModulos)
                        {
                            Col_SubModuloModulo _subModulo = new Col_SubModuloModulo();
                            _subModulo.Id = Convert.ToInt32(id);
                            _subModulo.ModuloId = item.ModuloId;
                            _subModulo.SubModuloId = item.SubModuloId;
                            _subModulo.RolId = _rol.RolId;
                            _subModulo.PermisosCrud = item.PermisosCrud.Trim();
                            subModuloModulos.Add(_subModulo);
                            id++;
                        }
                        await context.AddRangeAsync(subModuloModulos);
                    }
                    #endregion

                    await context.SaveChangesAsync();
                }

                return new ApiCallResult
                {
                    Status = status,
                    Title = title,
                    Message = message
                };
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
                return new ApiCallResult { Status = false, Title = "Error al guardar", Message = "Favor contacte éste con el administrador" };
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
                return new ApiCallResult { Status = false, Title = "Error al guardar", Message = "Favor contacte éste con el administrador" };
            }
            #endregion
        }

        public async Task<ApiCallResult> ActivarPerfil(int rolId)
        {
            var rol = await context.Col_Roles.Where(w => w.RolId.Equals(rolId)).FirstOrDefaultAsync();
            rol.Estado = "A";
            rol.FechaActualizacion = DateTime.Now;
            context.Col_Roles.Update(rol);

            await context.SaveChangesAsync();

            return new ApiCallResult
            {
                Status = true,
                Title = "Exitos",
                Message = $"El Perfil {rol.NombreRol} se habitlitó exitosamente"
            };
        }
    }
}