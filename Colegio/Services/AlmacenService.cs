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
    public class AlmacenService : IAlmacen
    {
        private readonly ColegioContext context;
        public AlmacenService(ColegioContext context)
        {
            this.context = context;
        }

        public async Task<ApiCallResult> GuardarSuministros(Col_Suministros suministros)
        {
            try
            {
                int? maxId = await context.Col_Suministros.MaxAsync(m => (int?)m.SuministroId);
                int? id = maxId == null ? 1 : maxId + 1;
                suministros.FechaCreacion = DateTime.Now;
                suministros.SuministroId = Convert.ToInt32(id);
                await context.AddAsync<Col_Suministros>(suministros);
                await context.SaveChangesAsync();
                return new ApiCallResult
                {
                    Title = "Éxito",
                    Message = "Se guardaron los insumos correctamente",
                    Status = true,
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

        public async Task<List<Col_Suministros>> ListarSuministros()
        {
            try
            {
                var suministros = await context.Col_Suministros.Select(s => new Col_Suministros
                {
                    Descripcion = s.Descripcion,
                    FechaActualizacion = s.FechaActualizacion,
                    FechaCreacion = s.FechaCreacion,
                    Nombre = s.Nombre,
                    Stock = s.Stock,
                    SuministroId = s.SuministroId,
                    Talla = s.Talla,
                    TipoSuministro = s.TipoSuministro,
                    Prestado = s.Prestado // la cantidad prestada que viene de la tabla prestamos
                }).ToListAsync();
                return suministros;
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

        public async Task<Col_Personas> BuscarEmpleado(string documento)
        {
            try
            {
                //var empleado = await context.Col_Personas
                //    .Join(context.Col_Laborales)
                return null;
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
