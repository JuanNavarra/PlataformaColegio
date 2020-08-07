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
            List<Col_Materias> materias = await context.Col_Materias.ToListAsync();
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
                List<Horarios> horarioMaterias = await MostrarHorasMaterias(_horario.CursoId);
                bool agregar = horarioMaterias
                            .Where(w => w.Intervalo.Contains(_horario.HoraIni) ||
                                        w.HoraIni.Equals(_horario.HoraIni) ||
                                        w.Intervalo.Contains(_horario.HoraFin)).Any();
                if (!agregar)
                {
                    int menor = _horario.HoraIni.Contains("10") || _horario.HoraIni.Contains("11") || (_horario.HoraIni.Contains("PM") && _horario.HoraIni.Contains("12")) ?
                                Convert.ToInt32(_horario.HoraIni.Substring(2, 2) + _horario.HoraIni.Substring(5, 2)) :
                                _horario.HoraIni.Contains("PM") && !_horario.HoraIni.Contains("12") ?
                                Convert.ToInt32((Convert.ToInt32(_horario.HoraIni.Substring(2, 1)) + 12).ToString() + _horario.HoraIni.Substring(4, 2)) :
                                Convert.ToInt32(_horario.HoraIni.Substring(2, 1) + _horario.HoraIni.Substring(4, 2));
                    int mayor = _horario.HoraFin.Contains("10") || _horario.HoraFin.Contains("11") || (_horario.HoraFin.Contains("PM") && _horario.HoraFin.Contains("12")) ?
                                Convert.ToInt32(_horario.HoraFin.Substring(2, 2) + _horario.HoraFin.Substring(5, 2)) :
                                _horario.HoraFin.Contains("PM") && !_horario.HoraFin.Contains("12") ?
                                Convert.ToInt32((Convert.ToInt32(_horario.HoraFin.Substring(2, 1)) + 12).ToString() + _horario.HoraFin.Substring(4, 2)) :
                                Convert.ToInt32(_horario.HoraFin.Substring(2, 1) + _horario.HoraFin.Substring(4, 2));
                    int i = 0;
                    List<List<string>> rangos = horarioMaterias
                        .Where(w => w.HoraIni.Contains(_horario.HoraIni.Substring(0, 1)))
                        .Select(s => s.Intervalo).ToList();
                    foreach (List<string> item in rangos)
                    {
                        int numero = item[i].Contains("10") || item[i].Contains("11") || (item[i].Contains("PM") && item[i].Contains("12")) ?
                                     Convert.ToInt32(item[i].Substring(2, 2) + item[i].Substring(5, 2)) :
                                     item[i].Contains("PM") && !item[i].Contains("12") ?
                                     Convert.ToInt32(((Convert.ToInt32(item[i].Substring(2, 1)) + 12).ToString() + item[i].Substring(4, 2))) :
                                     Convert.ToInt32(item[i].Substring(2, 1) + item[i].Substring(4, 2));
                        bool pregunta = menor <= numero && numero <= mayor;
                        if (pregunta)
                        {
                            return new ApiCallResult
                            {
                                Status = false,
                                Title = "Error",
                                Message = $"Solo puede insertar una mateteria en estas horas"
                            };
                        }
                        i++;
                    }
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
                else
                {
                    return new ApiCallResult
                    {
                        Status = false,
                        Title = "Error",
                        Message = $"Solo puede insertar una mateteria en estas horas"
                    };
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

                List<Horarios> query = await (from t0 in context.Col_Cursos
                                   join t1 in context.Col_Horarios on t0.CursoId equals t1.CursoId
                                   join t2 in context.Col_Materias on t1.MateriaId equals t2.MateriaId
                                   where t0.CursoId.Equals(cursoId)
                                   select new Horarios
                                   {
                                       Color = t2.Color,
                                       HoraFin = t1.HoraFin,
                                       HoraIni = t1.HoraIni,
                                       Materia = t2.Nombre,
                                       Id = t1.HorarioId,
                                   }).ToListAsync();

                foreach (Horarios item in query)
                {
                    Horarios _horario = new Horarios();
                    string[] horaIni = item.HoraIni.Split('-');
                    string[] _horaIni = horaIni[1].Split('_');
                    int ini = horaIni[2].Equals("AM") || _horaIni[0] == "12" ? Convert.ToInt32(_horaIni[0]) : Convert.ToInt32(_horaIni[0]) + 12;
                    string[] horaFin = item.HoraFin.Split('-');
                    string[] _horaFin = horaFin[1].Split('_');
                    int fin = horaFin[2].Equals("AM") || _horaFin[0] == "12" ? Convert.ToInt32(_horaFin[0]) : Convert.ToInt32(_horaFin[0]) + 12;
                    int restaFin = Convert.ToInt32(fin.ToString() + _horaFin[1]);
                    int restaIni = Convert.ToInt32(ini.ToString() + _horaIni[1]);
                    if ((restaFin - restaIni) > 30)
                    {
                        _horario.HoraFin = item.HoraFin;
                        _horario.HoraIni = item.HoraIni;
                        _horario.Id = item.Id;
                        _horario.orden = ini;
                        int cont = 0;
                        List<string> _intervalos = new List<string>();
                        for (int i = ini; i <= fin; i++)
                        {
                            string ampm = i < 12 ? "AM" : "PM";
                            if (i > 12)
                            {
                                int hora = i - 12;
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
                        _horario.Id = item.Id;
                        _horario.orden = ini;
                        _horarios.Add(_horario);
                    }
                }
                return _horarios;
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

        public async Task<ApiCallResult> EliminarHoriorMateria(int horarioId)
        {
            try
            {
                Col_Horarios horario = await context.Col_Horarios.Where(w => w.HorarioId.Equals(horarioId)).FirstOrDefaultAsync();
                context.Col_Horarios.Remove(horario);
                await context.SaveChangesAsync();
                return new ApiCallResult
                {
                    Status = true,
                    Title = "Éxito",
                    Message = $"Se ha eliminado con éxito la materia de ese horario"
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
    }
}
