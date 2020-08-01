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

        public async Task<EmpleadosContratados> BuscarEmpleado(string documento)
        {
            try
            {
                EmpleadosContratados empleado = new EmpleadosContratados();
                empleado.Empleado = await (from t0 in context.Col_Personas
                                           join t1 in context.Col_Laborales on t0.PersonaId equals t1.PersonaId
                                           where t0.NumeroDocumento.Equals(documento) && t0.Estado.Equals("A")
                                           select new EmpleadosContratados
                                           {
                                               Nombre = $"{t0.PrimerNombre} {t0.PrimerApellido}",
                                               Documento = t0.NumeroDocumento,
                                               NombreCargo = t1.NombreCargo,
                                               Id = t1.LaboralId
                                           }).FirstOrDefaultAsync();
                empleado.Insumos = await context.Col_InsumoLaboral
                    .Where(w => w.LaboralId.Equals(empleado.Empleado.Id))
                    .Select(s => new Col_InsumoLaboral { Nombre = s.Nombre })
                    .ToListAsync();

                return empleado;
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

        public async Task<List<Col_Suministros>> MostrarSuministros()
        {
            try
            {
                var suministros = await context.Col_Suministros
                    .Where(w => w.Stock > 0)
                    .Select(s => new Col_Suministros { Linea = s.Linea, Stock = s.Stock, Nombre = s.Nombre, SuministroId = s.SuministroId })
                    .ToListAsync();
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

        public async Task<ApiCallResult> PrestarInsumos(List<Col_Prestamos> prestamos, string documento)
        {
            try
            {
                bool status = false;
                string title = "Error";
                string message = "Error al ingresar el prestamo";
                var personaId = await context.Col_Personas
                    .Where(w => w.NumeroDocumento.Equals(documento) && w.Estado.Equals("A"))
                    .FirstOrDefaultAsync();
                if (personaId != null)
                {
                    var suministros = await context.Col_Suministros.ToListAsync();
                    var cantidadSuperior = suministros.Where(w => w.Stock < prestamos.Where(p => p.SuministroId == w.SuministroId)
                        .Select(s => s.Cantidad).Sum()).ToList();
                    if (cantidadSuperior.Any())
                    {
                        return new ApiCallResult
                        {
                            Status = false,
                            Title = "Error de stock",
                            Message = "Asegurate de que la cantidad elegida del insumo quitado no sobrepase el stock",
                            Comodin = cantidadSuperior.Select(s => s.SuministroId).FirstOrDefault()
                        };
                    }
                    status = true;
                    title = "Proceso exitoso";
                    message = $"El prestamo se guardó correctamente al empleado con documento {documento}";
                    int? maxId = await context.Col_Prestamos.MaxAsync(m => (int?)m.PrestamoId);
                    int? id = maxId == null ? 1 : maxId + 1;
                    List<Col_Prestamos> _prestamos = new List<Col_Prestamos>();
                    foreach (var item in prestamos)
                    {
                        Col_Prestamos _prestamo = new Col_Prestamos();
                        _prestamo.PrestamoId = Convert.ToInt32(id);
                        _prestamo.PersonaId = personaId.PersonaId;
                        _prestamo.SuministroId = item.SuministroId;
                        _prestamo.Motivo = item.Motivo;
                        _prestamo.FechaPrestamo = item.FechaPrestamo;
                        _prestamo.FechaCreacion = DateTime.Now;
                        _prestamo.Cantidad = item.Cantidad;
                        _prestamos.Add(_prestamo);
                        id++;
                        Col_Suministros _suministro = await context.Col_Suministros
                            .Where(w => w.SuministroId.Equals(item.SuministroId)).FirstOrDefaultAsync();
                        _suministro.Stock -= item.Cantidad;
                        context.Col_Suministros.Update(_suministro);
                    }
                    await context.AddRangeAsync(_prestamos);
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
    }
}
