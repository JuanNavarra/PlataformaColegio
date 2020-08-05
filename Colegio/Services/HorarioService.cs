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
    public class HorarioService : IHorario
    {
        private readonly ColegioContext context;
        public HorarioService(ColegioContext context)
        {
            this.context = context;
        }

        public async Task<ApiCallResult> GuardarMaterias(Col_Materias materia)
        {
            try
            {
                int? maxId = await context.Col_Materias.MaxAsync(m => (int?)m.MateriaId);
                int? id = maxId == null ? 1 : maxId + 1;
                materia.MateriaId = Convert.ToInt32(id);
                materia.FechaCreacion = DateTime.Now;
                await context.AddAsync<Col_Materias>(materia);
                await context.SaveChangesAsync();
                return new ApiCallResult
                {
                    Status = true,
                    Title = "Éxito",
                    Message = $"La materia {materia.Nombre} - {materia.Codigo} se ha guardado con éxito"
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

        public async Task<List<Col_Materias>> MostrarMarterias()
        {
            var materias = await context.Col_Materias.ToListAsync();
            return materias.OrderBy(o => o.Nombre).ToList();
        }

        public async Task<List<Col_Cursos>> MostrarCursos()
        {
            return await context.Col_Cursos.OrderBy(o => o.Nombre).ToListAsync();
        }

        public async Task<ApiCallResult> AgregarMateriasHorario(Col_Horarios _horario)
        {
            try
            {
                int? maxId = await context.Col_Horarios.MaxAsync(m => (int?)m.HorarioId);
                int? id = maxId == null ? 1 : maxId + 1;
                _horario.HorarioId = Convert.ToInt32(id);
                await context.AddAsync<Col_Horarios>(_horario);
                await context.SaveChangesAsync();
                return new ApiCallResult
                {
                    Status = true,
                    Title = "Éxito",
                    Message = $"Se agregó una hora de materia al horario con éxito"
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

        public async Task<List<Horarios>> MostrarHorasMaterias(int cursoId)
        {
            try
            {
                List<Horarios> _horarios = new List<Horarios>();

                var query = await (from t0 in context.Col_Cursos
                                   join t1 in context.Col_Horarios on t0.CursoId equals t1.CursoId
                                   join t2 in context.Col_Materias on t1.MateriaId equals t2.MateriaId
                                   where t0.CursoId.Equals(cursoId)
                                   select new Horarios
                                   {
                                       Color = t2.Color,
                                       HoraFin = t1.HoraFin,
                                       HoraIni = t1.HoraIni,
                                       Materia = t2.Nombre
                                   }).ToListAsync();

                foreach (var item in query)
                {
                    Horarios _horario = new Horarios();
                    var horaIni = item.HoraIni.Split('-');
                    var _horaIni = horaIni[1].Split('_');
                    var ini = horaIni[2].Equals("AM") || _horaIni[0] == "12" ? Convert.ToInt32(_horaIni[0]) : Convert.ToInt32(_horaIni[0]) + 12;
                    var horaFin = item.HoraFin.Split('-');
                    var _horaFin = horaFin[1].Split('_');
                    var fin = horaFin[2].Equals("AM") || _horaFin[0] == "12" ? Convert.ToInt32(_horaFin[0]) : Convert.ToInt32(_horaFin[0]) + 12;
                    var restaFin = Convert.ToInt32(fin.ToString() + _horaFin[1]);
                    var restaIni = Convert.ToInt32(ini.ToString() + _horaIni[1]);
                    if ((restaFin - restaIni) > 30)
                    {
                        _horario.HoraFin = item.HoraFin;
                        _horario.HoraIni = item.HoraIni;
                        int cont = 0;
                        List<string> _intervalos = new List<string>();
                        for (int i = ini; i <= fin; i++)
                        {
                            var ampm = i < 12 ? "AM" : "PM";
                            if (i > 12)
                            {
                                var hora = i - 12;
                                if (item.HoraIni.Contains("30"))
                                {
                                    if (cont != 0)
                                    {
                                        if (i != fin)
                                            _intervalos.Add($"{horaIni[0]}-{hora}_30-{ampm}");
                                        if (!$"{horaIni[0]}-{hora}_00-{ampm}".Equals(item.HoraFin))
                                            _intervalos.Add($"{horaIni[0]}-{hora}_00-{ampm}");
                                    }
                                }
                                else
                                {
                                    if (i != fin)
                                        _intervalos.Add($"{horaIni[0]}-{hora}_30-{ampm}");
                                    if (cont != 0)
                                    {
                                        if (!$"{horaIni[0]}-{hora}_00-{ampm}".Equals(item.HoraFin))
                                            _intervalos.Add($"{horaIni[0]}-{hora}_00-{ampm}");
                                    }
                                }
                                cont++;
                            }
                            if (i < 13)
                            {
                                if (item.HoraIni.Contains("30"))
                                {
                                    if (cont != 0)
                                    {
                                        if (i != fin)
                                            _intervalos.Add($"{horaIni[0]}-{i}_30-{ampm}");
                                        if (!$"{horaIni[0]}-{i}_00-{ampm}".Equals(item.HoraFin))
                                            _intervalos.Add($"{horaIni[0]}-{i}_00-{ampm}");
                                    }
                                }
                                else
                                {
                                    if (i != fin)
                                        _intervalos.Add($"{horaIni[0]}-{i}_30-{ampm}");
                                    if (cont != 0)
                                    {
                                        if (!$"{horaIni[0]}-{i}_00-{ampm}".Equals(item.HoraFin))
                                            _intervalos.Add($"{horaIni[0]}-{i}_00-{ampm}");
                                    }
                                }
                                cont++;
                            }
                        }
                        _horario.Materia = item.Materia;
                        _horario.Color = item.Color;
                        _horario.Intervalo = _intervalos;
                        _horarios.Add(_horario);
                    }
                    else
                    {
                        _horario.Color = item.Color;
                        _horario.HoraFin = item.HoraFin;
                        _horario.HoraIni = item.HoraIni;
                        _horario.Materia = item.Materia;
                        _horarios.Add(_horario);
                    }
                }
                return _horarios;
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
