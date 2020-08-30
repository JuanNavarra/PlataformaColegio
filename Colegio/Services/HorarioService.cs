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

        #region Horarios
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

        public async Task<List<Col_Materias>> MostrarMarterias(int? cursoId)
        {
            List<Col_Materias> materias = new List<Col_Materias>();
            if (cursoId == null) materias = await context.Col_Materias.ToListAsync();
            else
            {
                var _materias = await context.Col_Materias
                    .Join(context.Col_Horarios,
                        m => m.MateriaId,
                        h => h.MateriaId,
                        (m, h) => new { Col_Materias = m, Col_Horarios = h })
                    .Where(w => w.Col_Horarios.CursoId == cursoId)
                    .Select(s => new Col_Materias
                    {
                        MateriaId = s.Col_Materias.MateriaId,
                        Nombre = s.Col_Materias.Nombre
                    }).ToListAsync();
                foreach (var item in _materias.GroupBy(g => g.MateriaId))
                {
                    Col_Materias _materia = new Col_Materias();
                    _materia.MateriaId = item.Select(s => s.MateriaId).FirstOrDefault();
                    _materia.Nombre = item.Select(s => s.Nombre).FirstOrDefault();
                    materias.Add(_materia);
                }
            }
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
                    foreach (List<string> _rango in rangos)
                    {
                        int j = 0;
                        foreach (var item in _rango)
                        {
                            int numero = item.Contains("10") || item.Contains("11") || (item.Contains("PM") && item.Contains("12")) ?
                                         Convert.ToInt32(item.Substring(2, 2) + item.Substring(5, 2)) :
                                         item.Contains("PM") && !item.Contains("12") ?
                                         Convert.ToInt32(((Convert.ToInt32(item.Substring(2, 1)) + 12).ToString() + item.Substring(4, 2))) :
                                         Convert.ToInt32(item.Substring(2, 1) + item.Substring(4, 2));
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
                            j++;
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
        #endregion

        #region Enlaces
        public async Task<List<Col_Personas>> CargarProfesores()
        {
            var profesores = await context.Col_Personas
                .Join(context.Col_Laborales,
                    p => p.PersonaId,
                    l => l.PersonaId,
                    (p, l) => new { Col_Personas = p, Col_Laborales = l })
                .Where(w => w.Col_Laborales.NombreCargo.Equals("profesor") && w.Col_Personas.Estado.Equals("A")
                             && w.Col_Personas.UsuarioId != -1)
                .Select(s => new Col_Personas
                {
                    PrimerNombre = $"{s.Col_Personas.PrimerNombre} {s.Col_Personas.SegundoNombre} " +
                                   $"{s.Col_Personas.PrimerApellido} {s.Col_Personas.SegundoApellido}",
                    PersonaId = s.Col_Personas.PersonaId,
                }).ToListAsync();
            return profesores;
        }

        public async Task<Horarios> MostrarHorarios(int busqueda)
        {
            Col_Personas profesores = await context.Col_Personas
                    .Join(context.Col_Laborales,
                        p => p.PersonaId,
                        l => l.PersonaId,
                        (p, l) => new { Col_Personas = p, Col_Laborales = l })
                    .Where(w => w.Col_Laborales.NombreCargo.Equals("profesor") && w.Col_Personas.Estado.Equals("A")
                                    && w.Col_Personas.UsuarioId != -1
                                    && (w.Col_Personas.PersonaId.Equals(busqueda) || w.Col_Personas.NumeroDocumento.Equals(busqueda.ToString())))
                    .Select(s => new Col_Personas
                    {
                        PersonaId = s.Col_Personas.PersonaId,
                    }).FirstOrDefaultAsync();

            if (profesores != null)
            {
                Horarios horarios = new Horarios();
                horarios.Profesor = await CargarProfesorEspecifico(profesores);
                horarios.Cursos = await MostrarCursos();
                return horarios;
            }
            return null;
        }

        private async Task<Col_Personas> CargarProfesorEspecifico(Col_Personas persona)
        {
            var profesor = await (from t0 in context.Col_Usuarios
                                  join t1 in context.Col_Personas on t0.Id equals t1.UsuarioId
                                  join t2 in context.Col_Laborales on t1.PersonaId equals t2.PersonaId
                                  where t1.NumeroDocumento.Equals(persona.NumeroDocumento) || t1.PersonaId.Equals(persona.PersonaId)
                                        && t2.NombreCargo.Equals("profesor")
                                  select new Col_Personas
                                  {
                                      NumeroDocumento = t1.NumeroDocumento,
                                      CorreoPersonal = string.IsNullOrEmpty(t2.CorreoCorporativo) ? t1.CorreoPersonal : t2.CorreoCorporativo,
                                      PrimerNombre = $"{t1.PrimerNombre} {t1.PrimerApellido} {t1.SegundoApellido}",
                                      Usuario = t0.Usuario,
                                  }).FirstOrDefaultAsync();
            return profesor;
        }

        public async Task<List<Col_Horarios>> MostrarDiasSemana(int materiaId, int cursoId)
        {
            try
            {
                var diasSemana = new Dictionary<string, string>
                {
                    ["l"] = "Lunes",
                    ["m"] = "Martes",
                    ["x"] = "Miércoles",
                    ["j"] = "Juéves",
                    ["v"] = "Viernes",
                    ["s"] = "Sábado",
                };
                var _dias = await (from t0 in context.Col_Materias
                                   join t1 in context.Col_Horarios on t0.MateriaId equals t1.MateriaId
                                   join t2 in context.Col_Cursos on t1.CursoId equals t2.CursoId
                                   where t2.CursoId == cursoId && t0.MateriaId == materiaId
                                   select new Col_Horarios
                                   {
                                       HorarioId = t1.HorarioId,
                                       HoraIni = t1.HoraIni,
                                   }).ToListAsync();

                List<Col_Horarios> dias = new List<Col_Horarios>();
                foreach (var item in _dias.GroupBy(g => g.HoraIni.Substring(0, 1)))
                {
                    Col_Horarios dia = new Col_Horarios();
                    dia.HorarioId = item.Select(s => s.HorarioId).FirstOrDefault();
                    dia.Dia = diasSemana.Where(w => w.Key.StartsWith(item.Select(s => s.HoraIni.Substring(0, 1)).FirstOrDefault()))
                        .Select(s => s.Value).FirstOrDefault();
                    dias.Add(dia);
                }
                return dias.OrderBy(o => o.Dia).ToList();
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

        public async Task<List<Horarios>> MostrarHorarios(string dia, int materiaId, int cursoId)
        {
            try
            {
                var horas = new Dictionary<string, string>
                {
                    ["6_30-AM"] = "6:30 AM",
                    ["7_00-AM"] = "7:00 AM",
                    ["7_30-AM"] = "7:30 AM",
                    ["8_00-AM"] = "8:00 AM",
                    ["8_30-AM"] = "8:30 AM",
                    ["9_00-AM"] = "9:00 AM",
                    ["9_30-AM"] = "9:30 AM",
                    ["10_00-AM"] = "10:00 AM",
                    ["10_30-AM"] = "10:30 AM",
                    ["11_00-AM"] = "11:00 AM",
                    ["11_30-AM"] = "11:30 AM",
                    ["12_00-PM"] = "12:00 PM",
                    ["12_30-PM"] = "12:30 PM",
                    ["1_00-PM"] = "1:00 PM",
                    ["1_30-PM"] = "1:30 PM",
                    ["2_00-PM"] = "2:00 PM",
                    ["2_30-PM"] = "2:30 PM",
                    ["3_00-PM"] = "3:00 PM",
                    ["3_30-PM"] = "3:30 PM",
                    ["4_00-PM"] = "4:00 PM",
                    ["4_30-PM"] = "4:30 PM",
                    ["5_00-PM"] = "5:00 PM",
                    ["5_30-PM"] = "5:30 PM",
                    ["6_00-PM"] = "6:00 PM",
                };
                dia = dia.Equals("Miércoles") ? "x" : dia;
                var _horarios = await (from t0 in context.Col_Cursos
                                       join t1 in context.Col_Horarios on t0.CursoId equals t1.CursoId
                                       join t2 in context.Col_Materias on t1.MateriaId equals t2.MateriaId
                                       where t0.CursoId == cursoId && t2.MateriaId == materiaId
                                        && t1.HoraIni.StartsWith(dia.Substring(0, 1))
                                       select new Horarios
                                       {
                                           Materia = t2.Nombre,
                                           HoraFin = t1.HoraFin,
                                           HoraIni = t1.HoraIni,
                                           Id = t1.HorarioId,
                                       }).ToListAsync();
                List<Horarios> horarios = new List<Horarios>();
                foreach (var temp in _horarios)
                {
                    Horarios _horario = new Horarios();
                    _horario.Id = temp.Id;
                    _horario.Horario = ($" {horas.Where(w => w.Key.Contains(temp.HoraIni.Substring(2))).Select(s => s.Value).FirstOrDefault()} -" +
                                        $" {horas.Where(w => w.Key.Contains(temp.HoraFin.Substring(2))).Select(s => s.Value).FirstOrDefault()}");
                    horarios.Add(_horario);
                }
                return horarios;
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

        public async Task<ApiCallResult> AgregarEnlaceProfesorHorario(int idHorario, string documento)
        {
            try
            {
                bool exiteProfesor = await context.Col_Horarios
                    .Where(w => w.HorarioId == idHorario && w.PersonaId != 0)
                    .AnyAsync();
                if (!exiteProfesor)
                {
                    var horario = await context.Col_Horarios
                        .Where(w => w.HorarioId == idHorario)
                        .FirstOrDefaultAsync();
                    horario.PersonaId = (await context.Col_Personas.Where(w => w.NumeroDocumento == documento).FirstOrDefaultAsync()).PersonaId;
                    context.Col_Horarios.Update(horario);
                    await context.SaveChangesAsync();
                    return new ApiCallResult
                    {
                        Status = true,
                        Title = "Exito",
                        Message = "El enlace Profesor - Horario se ha creado con exito"
                    };
                }
                else
                {
                    return new ApiCallResult
                    {
                        Status = false,
                        Title = "Error",
                        Message = "Ya existe un profesor con ese horario"
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

        public async Task<List<Horarios>> MostrarEnlaceProfesorHorario(string documento)
        {
            try
            {
                var horas = new Dictionary<string, string>
                {
                    ["6_30-AM"] = "6:30 AM",
                    ["7_00-AM"] = "7:00 AM",
                    ["7_30-AM"] = "7:30 AM",
                    ["8_00-AM"] = "8:00 AM",
                    ["8_30-AM"] = "8:30 AM",
                    ["9_00-AM"] = "9:00 AM",
                    ["9_30-AM"] = "9:30 AM",
                    ["10_00-AM"] = "10:00 AM",
                    ["10_30-AM"] = "10:30 AM",
                    ["11_00-AM"] = "11:00 AM",
                    ["11_30-AM"] = "11:30 AM",
                    ["12_00-PM"] = "12:00 PM",
                    ["12_30-PM"] = "12:30 PM",
                    ["1_00-PM"] = "1:00 PM",
                    ["1_30-PM"] = "1:30 PM",
                    ["2_00-PM"] = "2:00 PM",
                    ["2_30-PM"] = "2:30 PM",
                    ["3_00-PM"] = "3:00 PM",
                    ["3_30-PM"] = "3:30 PM",
                    ["4_00-PM"] = "4:00 PM",
                    ["4_30-PM"] = "4:30 PM",
                    ["5_00-PM"] = "5:00 PM",
                    ["5_30-PM"] = "5:30 PM",
                    ["6_00-PM"] = "6:00 PM",
                };
                var diasSemana = new Dictionary<string, string>
                {
                    ["l"] = "Lunes",
                    ["m"] = "Martes",
                    ["x"] = "Miércoles",
                    ["j"] = "Juéves",
                    ["v"] = "Viernes",
                    ["s"] = "Sábado",
                };
                var _horarios = await (from t0 in context.Col_Cursos
                                       join t1 in context.Col_Horarios on t0.CursoId equals t1.CursoId
                                       join t2 in context.Col_Materias on t1.MateriaId equals t2.MateriaId
                                       join t3 in context.Col_Personas on t1.PersonaId equals t3.PersonaId
                                       where t3.NumeroDocumento.Equals(documento) || t3.PersonaId.Equals(Convert.ToInt32(documento))
                                       select new Horarios
                                       {
                                           Curso = t0.Nombre,
                                           Materia = t2.Nombre,
                                           HoraFin = t1.HoraFin,
                                           HoraIni = t1.HoraIni,
                                       }).ToListAsync();
                List<Horarios> horarios = new List<Horarios>();
                foreach (var temp in _horarios)
                {
                    Horarios _horario = new Horarios
                    {
                        Id = temp.Id,
                        Curso = temp.Curso,
                        Materia = temp.Materia,
                        Dia = diasSemana.Where(w => w.Key.StartsWith(temp.HoraIni.Substring(0, 1))).Select(s => s.Value).FirstOrDefault(),
                        Horario = ($" {horas.Where(w => w.Key.Contains(temp.HoraIni.Substring(2))).Select(s => s.Value).FirstOrDefault()} -" +
                                        $" {horas.Where(w => w.Key.Contains(temp.HoraFin.Substring(2))).Select(s => s.Value).FirstOrDefault()}")
                    };
                    horarios.Add(_horario);
                }
                return horarios.OrderBy(o => o.Dia).ThenBy(t => t.Horario).ToList();
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
        #endregion
    }
}