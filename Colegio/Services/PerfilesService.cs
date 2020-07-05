using Colegio.Data;
using Colegio.Models;
using Colegio.Models.ModelHelper;
using Colegio.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
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

        public async Task<List<Col_Roles>> CargarRol()
        {
            var roles = await context.Col_Roles
                .Where(w => w.Estado.Equals("A"))
                .Select(s => new Col_Roles
                {
                    RolId = s.RolId,
                    NombreRol = s.NombreRol
                })
                .ToListAsync();
            return roles;
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

        public async Task<List<Col_PermisosCrud>> CargarPermisosCRUD()
        {
            var crud = await context.Col_PermisosCrud
                .Where(w => w.Estado.Equals("A"))
                .Select(s => new Col_PermisosCrud
                {
                    Descripcion = s.Descripcion,
                    Nombre = s.Nombre,
                    PermisoId = s.PermisoId
                })
                .ToListAsync();
            return crud;
        }

        public async Task<ApiCallResult> GuardarAutorizaciones(List<Col_Modulos> modulo, List<Col_SubModulos> subModulo, string rolNombre, List<Col_PermisosCrud> permisos, string descripcion)
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
                    List<Col_PermisoRol> permisoRoles = new List<Col_PermisoRol>();
                    List<Col_SubModuloModulo> subModuloModulos = new List<Col_SubModuloModulo>();

                    status = true;
                    title = "Proceso exitoso";
                    message = "Los datos fueron guardados correctamente en la plataforma";

                    int? maxIdRol = await context.Col_Roles.MaxAsync(m => (int?)m.RolId);
                    int? rolId = maxIdRol == null ? 1 : maxIdRol + 1;
                    rol.Estado = "A";
                    rol.FechaActualizacion = null;
                    rol.FechaCreacion = DateTime.Now;
                    rol.NombreRol = rolNombre.ToUpper();
                    rol.RolId = Convert.ToInt32(rolId);
                    rol.Descripcion = descripcion;
                    await context.AddAsync<Col_Roles>(rol);

                    int? maxIdPermisoRol = await context.Col_PermisoRol.MaxAsync(m => (int?)m.Id);
                    int? permisoRolId = maxIdPermisoRol == null ? 1 : maxIdPermisoRol + 1;
                    foreach (var item in permisos)
                    {
                        Col_PermisoRol permisoRol = new Col_PermisoRol();
                        permisoRol.Id = Convert.ToInt32(permisoRolId);
                        permisoRol.PermisoId = item.PermisoId;
                        permisoRol.RolId = rol.RolId;
                        permisoRoles.Add(permisoRol);
                        permisoRolId++;
                    }
                    await context.AddRangeAsync(permisoRoles);

                    int? maxRolModulo = await context.Col_RolModulos.MaxAsync(m => (int?)m.Id);
                    int? rolModuloId = maxRolModulo == null ? 1 : maxRolModulo + 1;
                    foreach (var item in modulo)
                    {
                        Col_RolModulos _rolModulo = new Col_RolModulos();
                        _rolModulo.Id = Convert.ToInt32(rolModuloId);
                        _rolModulo.RolId = rol.RolId;
                        _rolModulo.ModuloId = item.ModuloId;
                        rolModulos.Add(_rolModulo);
                        rolModuloId++;
                    }
                    await context.AddRangeAsync(rolModulos);

                    int? maxSubModuloModulo = await context.Col_SubModuloModulo.MaxAsync(m => (int?)m.Id);
                    int? rolSubModuloModuloId = maxSubModuloModulo == null ? 1 : maxSubModuloModulo + 1;
                    if (subModulo.Count() <= 0)
                    {
                        List<int> modulos = new List<int>();
                        foreach (var item in modulo)
                        {
                            modulos.Add(item.ModuloId);
                        }
                        subModulo = await context.Col_SubModulos
                            .Where(w => modulos.Contains(w.ModuloId) && w.Estado.Equals("A"))
                            .Select(s => new Col_SubModulos
                            {
                                SubModuloId = s.SubModuloId,
                                ModuloId = s.ModuloId
                            }).ToListAsync();
                    }
                    foreach (var item in subModulo)
                    {
                        Col_SubModuloModulo _subModulo = new Col_SubModuloModulo();
                        _subModulo.Id = Convert.ToInt32(rolSubModuloModuloId);
                        _subModulo.ModuloId = item.ModuloId;
                        _subModulo.SubModuloId = item.SubModuloId;
                        subModuloModulos.Add(_subModulo);
                        rolSubModuloModuloId++;
                    }
                    await context.AddRangeAsync(subModuloModulos);

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

        public async Task<List<Col_Roles>> MostrarAutorizaciones()
        {
            try
            {
                var query = await context.Col_Roles
                    .Where(w => w.Estado.Equals("A"))
                    .Select(s => new Col_Roles
                    {
                        NombreRol = s.NombreRol,
                        RolId = s.RolId,
                        FechaActualizacion = s.FechaActualizacion,
                        FechaCreacion = s.FechaCreacion,
                        Estado = s.Estado == "A" ? "ACTIVO" : "INACTIVO",
                        Descripcion = s.Descripcion ?? "",
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
    }
}