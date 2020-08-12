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
                List<Col_Suministros> suministros = await context.Col_Suministros
                    .Select(s => new Col_Suministros
                    {
                        Descripcion = s.Descripcion,
                        FechaActualizacion = s.FechaActualizacion,
                        FechaCreacion = s.FechaCreacion,
                        Nombre = s.Nombre,
                        Stock = s.Stock,
                        SuministroId = s.SuministroId,
                        Talla = s.Talla,
                        Linea = s.Linea,
                        TipoSuministro = s.TipoSuministro,
                        Prestado = context.Col_Prestamos
                            .Where(w => w.SuministroId.Equals(s.SuministroId))
                            .Select(s => s.Cantidad).Sum(),
                        Total = s.Stock + context.Col_Prestamos
                            .Where(w => w.SuministroId.Equals(s.SuministroId))
                            .Select(s => s.Cantidad).Sum(),
                        UltimoPrestamo = context.Col_Prestamos
                            .Where(w => w.SuministroId.Equals(s.SuministroId))
                            .OrderByDescending(o => o.FechaPrestamo)
                            .Select(s => s.FechaPrestamo).FirstOrDefault()
                    }).ToListAsync();
                return suministros;
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

        public async Task<List<Col_Suministros>> MostrarSuministros()
        {
            List<Col_Suministros> suministros = await context.Col_Suministros
                .Where(w => w.Stock > 0)
                .Select(s => new Col_Suministros { Linea = s.Linea, Stock = s.Stock, Nombre = s.Nombre, SuministroId = s.SuministroId })
                .ToListAsync();
            return suministros;
        }

        public async Task<ApiCallResult> PrestarInsumos(List<Col_Prestamos> prestamos, string documento)
        {
            try
            {
                bool status = false;
                string title = "Error";
                string message = "Error al ingresar el prestamo";
                Col_Personas personaId = await context.Col_Personas
                    .Where(w => w.NumeroDocumento.Equals(documento) && w.Estado.Equals("A"))
                    .FirstOrDefaultAsync();
                if (personaId != null)
                {
                    List<Col_Suministros> suministros = await context.Col_Suministros.ToListAsync();
                    List<Col_Suministros> cantidadSuperior = suministros
                        .Where(w => w.Stock < prestamos.Where(p => p.SuministroId == w.SuministroId)
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
                    foreach (Col_Prestamos item in prestamos)
                    {
                        Col_Prestamos _prestamo = new Col_Prestamos();
                        _prestamo.PrestamoId = Convert.ToInt32(id);
                        _prestamo.PersonaId = personaId.PersonaId;
                        _prestamo.SuministroId = item.SuministroId;
                        _prestamo.Motivo = item.Motivo;
                        _prestamo.FechaPrestamo = item.FechaPrestamo;
                        _prestamo.FechaCreacion = DateTime.Now;
                        _prestamo.Cantidad = item.Cantidad;
                        _prestamo.Estado = "A";
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

        public async Task<List<Devoluciones>> BuscarDevolucion(string documento)
        {
            try
            {
                List<Devoluciones> query = await (from t0 in context.Col_Personas
                                                  join t1 in context.Col_Prestamos on t0.PersonaId equals t1.PersonaId
                                                  join t2 in context.Col_Suministros on t1.SuministroId equals t2.SuministroId
                                                  where t0.NumeroDocumento.Equals(documento) && t1.Estado.Equals("A")
                                                  select new Devoluciones
                                                  {
                                                      IdPrestamo = t1.PrestamoId,
                                                      IdPersona = t1.PersonaId,
                                                      Cantidad = t1.Cantidad,
                                                      Insumo = $"{t2.Nombre} - {t2.Linea}",
                                                      SuministroId = t2.SuministroId,
                                                  }).ToListAsync();
                IEnumerable<Devoluciones> devoluciones = query
                    .GroupBy(g => g.SuministroId)
                    .Select(s => new Devoluciones
                    {
                        Cantidad = s.Select(s => s.Cantidad).Sum(),
                        SuministroId = s.Select(s => s.SuministroId).FirstOrDefault(),
                        Insumo = s.Select(s => s.Insumo).FirstOrDefault(),
                        IdPrestamo = s.Select(s => s.IdPrestamo).FirstOrDefault(),
                        IdPersona = s.Select(s => s.IdPersona).FirstOrDefault(),
                    });
                return devoluciones.OrderByDescending(o => o.FechaPrestamo).ToList();
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

        public async Task<ApiCallResult> DevolverInsumos(List<Devoluciones> devoluciones)
        {
            try
            {
                bool status = false;
                string title = "Error";
                string message = "Error al devolver el insumo";

                bool query = devoluciones.Where(w => !w.Devolver.Equals(0)).Any();
                if (query)
                {
                    foreach (Devoluciones item in devoluciones)
                    {
                        List<Col_Prestamos> _prestamos = await context.Col_Prestamos
                            .Where(w => w.PersonaId.Equals(item.IdPersona)
                            && w.SuministroId.Equals(item.SuministroId) && w.Estado.Equals("A"))
                            .ToListAsync();
                        int faltante = item.Devolver;
                        foreach (Col_Prestamos temp in _prestamos)
                        {
                            if (faltante > 0)
                            {
                                int cantidadRestar = 0;
                                for (int i = 1; i <= item.Devolver; i++)
                                {
                                    if ((temp.Cantidad - i) >= 0)
                                    {
                                        cantidadRestar = i;
                                    }
                                }
                                faltante -= cantidadRestar;
                                Col_Prestamos _prestamo = await context.Col_Prestamos
                                    .Where(w => w.PrestamoId.Equals(temp.PrestamoId)).FirstOrDefaultAsync();
                                _prestamo.Cantidad -= cantidadRestar;
                                _prestamo.Estado = _prestamo.Cantidad == 0 ? "I" : "A";
                                _prestamo.FechaActualizacion = DateTime.Now;
                                context.Col_Prestamos.Update(_prestamo);
                                await context.SaveChangesAsync();
                            }
                        }
                        Col_Suministros suministro = await context.Col_Suministros
                            .Where(w => w.SuministroId.Equals(item.SuministroId)).FirstOrDefaultAsync();
                        suministro.Stock += item.Devolver;
                        suministro.FechaActualizacion = DateTime.Now;
                        context.Col_Suministros.Update(suministro);
                    }
                    await context.SaveChangesAsync();
                    status = true;
                    title = "Proceso éxitosa";
                    message = "¡Si hizo la devolución de los insumos satisfactoriamente!";

                }
                else
                {
                    status = false;
                    title = "Proceso incorrecto";
                    message = "¡Al menos tienes que devolver un insumo!";
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

        public async Task<ApiCallResult> VaciarStock(int suministroId, int stock)
        {
            Col_Suministros suministro = await context.Col_Suministros
                .Where(w => w.SuministroId.Equals(suministroId))
                .FirstOrDefaultAsync();
            suministro.Stock = 0;
            context.Col_Suministros.Update(suministro);
            await context.SaveChangesAsync();
            return new ApiCallResult
            {
                Status = true,
                Title = "Éxito",
                Message = $"El stock del suministro {suministro.Nombre} - {suministro.Linea} se ha vaciado con éxito"
            };
        }

        public async Task<ApiCallResult> EliminarSuministros(int suministroId)
        {
            Col_Suministros suministro = await context.Col_Suministros
                .Where(w => w.SuministroId.Equals(suministroId))
                .FirstOrDefaultAsync();
            context.Col_Suministros.Remove(suministro);
            await context.SaveChangesAsync();
            return new ApiCallResult
            {
                Status = true,
                Title = "Éxito",
                Message = $"El suministro {suministro.Nombre} - {suministro.Linea} se ha eliminado con éxito"
            };
        }
    }
}
