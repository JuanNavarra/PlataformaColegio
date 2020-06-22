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
using System.Transactions;

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
            try
            {
                var roles = await context.Col_Roles.Where(w => w.Estado.Equals("Y")).ToListAsync();
                return roles;
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


        public async Task<ApiCallResult> GuardarAutorizaciones(Col_Modulos modulo, List<Col_SubModulos> subModulo, string rol, bool swRol)
        {
            try
            {
                int? rolId = 0;
                bool existeRol = false;
                if (!swRol)
                {
                    existeRol = await context.Col_Roles
                        .Where(w => w.Estado.Equals("Y") && w.NombreRol.Equals(rol.ToUpper()))
                        .AnyAsync();

                }
                #region guardar roles
                if (!existeRol || swRol)
                {
                    if (!swRol)
                    {
                        var maxIdRol = await context.Col_Roles.MaxAsync(m => (int?)m.RolId);
                        rolId = maxIdRol == null ? 1 : maxIdRol + 1;
                        Col_Roles roles = new Col_Roles();
                        roles.RolId = Convert.ToInt32(rolId);
                        roles.NombreRol = rol.ToUpper();
                        roles.Estado = "Y";
                        roles.FechaActualizacion = null;
                        roles.FechaCreacion = DateTime.Now;
                        await context.AddAsync<Col_Roles>(roles);
                    }

                    #region guardar modulos
                    var extisteModulo = await context.Col_Modulos
                        .Where(w => w.Estado.Equals("Y") && w.Nombre.Equals(modulo.Nombre.ToUpper()))
                        .AnyAsync();
                    if (!extisteModulo)
                    {
                        var maxId = await context.Col_Modulos.MaxAsync(m => (int?)m.ModuloId);
                        maxId = maxId == null ? 1 : maxId + 1;
                        Col_Modulos _modulo = new Col_Modulos();
                        _modulo.ModuloId = Convert.ToInt32(maxId);
                        _modulo.Nombre = modulo.Nombre.ToUpper();
                        _modulo.RolId = !swRol ? Convert.ToInt32(rolId) : Convert.ToInt32(rol);
                        _modulo.FechaCreacion = DateTime.Now;
                        _modulo.Estado = "Y";
                        _modulo.Descripcion = modulo.Descripcion;
                        _modulo.EsPadre = modulo.EsPadre;
                        _modulo.EtiquetaDom = modulo.EtiquetaDom;
                        await context.AddAsync<Col_Modulos>(_modulo);

                        #region guardar submoulos
                        if (modulo.EsPadre)
                        {
                            List<Col_SubModulos> subModulos = new List<Col_SubModulos>();
                            var maxIdSub = await context.Col_SubModulos.MaxAsync(m => (int?)m.SubModuloId);
                            maxIdSub = maxIdSub == null ? 1 : maxIdSub + 1;
                            foreach (var item in subModulo)
                            {
                                Col_SubModulos _subModulo = new Col_SubModulos();
                                _subModulo.SubModuloId = Convert.ToInt32(maxIdSub);
                                _subModulo.ModuloId = Convert.ToInt32(maxId);
                                subModulos.Add(_subModulo);
                                maxIdSub++;
                            }
                            await context.AddRangeAsync(subModulos);
                        }
                        #endregion
                    }
                    else
                    {
                        return new ApiCallResult
                        {
                            Status = false,
                            Title = "Error al guardar",
                            Message = "No se pudo realizar el guardado porque éste módulo ya existe"
                        };
                    }
                    #endregion
                    await context.SaveChangesAsync();
                    return new ApiCallResult
                    {
                        Status = true,
                        Title = "Exito",
                        Message = "Los datos se guardaron éxitosamente"
                    };
                }
                else
                {
                    return new ApiCallResult
                    {
                        Status = false,
                        Title = "Error al guardar",
                        Message = "No se pudo realizar el guardado porque éste rol ya existe"
                    };
                }
                #endregion
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
    }
}
