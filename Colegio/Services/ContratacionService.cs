using Colegio.Data;
using Colegio.Models;
using Colegio.Models.ModelHelper;
using Colegio.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Threading.Tasks;

namespace Colegio.Services
{
    public class ContratacionService : IContratacion
    {
        private readonly ColegioContext context;
        public ContratacionService(ColegioContext context)
        {
            this.context = context;
        }

        public async Task<int> GuardarPersonales(Col_Personas persona, List<Col_InfoAcademica> infoAcademicas)
        {
            try
            {
                var existeUsuario = await context.Col_Personas
                    .Where(w => w.NumeroDocumento.Equals(persona.NumeroDocumento) && w.TipoDocumento.Equals(persona.TipoDocumento))
                    .AnyAsync();

                if (!existeUsuario)
                {
                    List<Col_InfoAcademica> _infoAcademicas = new List<Col_InfoAcademica>();

                    int? maxId = await context.Col_Personas.MaxAsync(m => (int?)m.PersonaId);
                    int? id = maxId == null ? 1 : maxId + 1;
                    persona.PersonaId = Convert.ToInt32(id);
                    persona.Estado = "P";//Pendienete
                    persona.FechaCreacion = DateTime.Now;
                    persona.Genero = "M";
                    persona.FechaActualizacion = null;
                    persona.Empleado = true;
                    persona.Progreso = 'P';
                    await context.AddAsync<Col_Personas>(persona);

                    maxId = await context.Col_InfoAcademica.MaxAsync(m => (int?)m.AcademicoId);
                    id = maxId == null ? 1 : maxId + 1;
                    foreach (var item in infoAcademicas)
                    {
                        Col_InfoAcademica infoAcademica = new Col_InfoAcademica();
                        infoAcademica.AcademicoId = Convert.ToInt32(id);
                        infoAcademica.FechaGradua = item.FechaGradua;
                        infoAcademica.NivelFormacion = item.NivelFormacion;
                        infoAcademica.NombreIns = item.NombreIns;
                        infoAcademica.PersonaId = persona.PersonaId;
                        infoAcademica.TituloObtenido = item.TituloObtenido;
                        _infoAcademicas.Add(infoAcademica);
                        id++;
                    }
                    await context.AddRangeAsync(_infoAcademicas);

                    await context.SaveChangesAsync();

                    return persona.PersonaId;
                }
                return -1;
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
                return 0;
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
                return 0;
            }
            #endregion
        }

        public async Task<bool> GuardarExperiencia(List<Col_Experiencia> experiencias, int personaId)
        {
            try
            {
                int? maxId = await context.Col_Experiencia.MaxAsync(m => (int?)m.ExperienciaId);
                int? id = maxId == null ? 1 : maxId + 1;
                List<Col_Experiencia> _experiencias = new List<Col_Experiencia>();
                foreach (var item in experiencias)
                {
                    Col_Experiencia experiencia = new Col_Experiencia();
                    experiencia.ExperienciaId = Convert.ToInt32(id);
                    experiencia.PersonaId = personaId;
                    experiencia.Cargo = item.Cargo;
                    experiencia.Empresa = item.Empresa;
                    experiencia.FechaFin = item.FechaFin;
                    experiencia.FechaInicio = item.FechaInicio;
                    experiencia.Funciones = item.Funciones;
                    experiencia.Logros = item.Logros;
                    _experiencias.Add(experiencia);
                    id++;
                }
                await context.AddRangeAsync(_experiencias);

                var persona = await context.Col_Personas.Where(w => w.PersonaId == personaId).FirstOrDefaultAsync();
                persona.Progreso = 'E';
                context.Col_Personas.Update(persona);

                await context.SaveChangesAsync();
                return true;
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
                return false;
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
                return false;
            }
            #endregion
        }

        public async Task<int> GuardarLaboborales(Col_Laborales laboral, List<Col_InsumoLaboral> insumos, int personaId)
        {
            try
            {
                int? maxId = await context.Col_Laborales.MaxAsync(m => (int?)m.LaboralId);
                int? id = maxId == null ? 1 : maxId + 1;

                laboral.LaboralId = Convert.ToInt32(id);
                laboral.PersonaId = personaId;
                await context.AddAsync<Col_Laborales>(laboral);

                List<Col_InsumoLaboral> _insumos = null;
                if (insumos != null)
                {
                    _insumos = new List<Col_InsumoLaboral>();
                    maxId = await context.Col_InsumoLaboral.MaxAsync(m => (int?)m.InsLabId);
                    id = maxId == null ? 1 : maxId + 1;
                    foreach (var item in insumos)
                    {
                        Col_InsumoLaboral insumo = new Col_InsumoLaboral();
                        insumo.InsLabId = Convert.ToInt32(id);
                        insumo.Nombre = item.Nombre;
                        insumo.LaboralId = laboral.LaboralId;
                        _insumos.Add(insumo);
                        id++;
                    }
                    await context.AddRangeAsync(_insumos);
                }
                var persona = await context.Col_Personas.Where(w => w.PersonaId.Equals(personaId)).FirstOrDefaultAsync();
                persona.Progreso = 'L';
                context.Col_Personas.Update(persona);
                await context.SaveChangesAsync();

                return laboral.LaboralId;
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
                return 0;
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
                return 0;
            }
            #endregion
        }

        public async Task<List<Col_Roles>> MostrarRoles()
        {
            var roles = await context.Col_Roles
                .Where(w => w.Estado.Equals("A"))
                .Select(s => new Col_Roles
                {
                    RolId = s.RolId,
                    NombreRol = s.NombreRol
                }).ToListAsync();
            return roles;
        }

        public async Task<ApiCallResult> GuardarAfiliacion(List<Col_Afiliacion> afiliaciones, int rol, int laboralId, Col_Personas persona)
        {
            try
            {
                bool status = false;
                string title = "Error";
                string message = "Se presento un error al asignar un Usuario y el Perfil a este Empleado";

                var _rol = await context.Col_Roles.Where(w => w.RolId.Equals(rol) && w.Estado.Equals("A")).FirstOrDefaultAsync();
                if (_rol != null)
                {
                    int? maxId = await context.Col_Afiliacion.MaxAsync(m => (int?)m.AfiliacionId);
                    int? id = maxId == null ? 1 : maxId + 1;
                    List<Col_Afiliacion> _afiliaciones = new List<Col_Afiliacion>();
                    foreach (var item in afiliaciones)
                    {
                        Col_Afiliacion afiliacion = new Col_Afiliacion();
                        afiliacion.AfiliacionId = Convert.ToInt32(id);
                        afiliacion.FechaActualizacion = null;
                        afiliacion.FechaAfiliacion = item.FechaAfiliacion;
                        afiliacion.FechaCreacion = DateTime.Now;
                        afiliacion.NombreEntidad = item.NombreEntidad;
                        afiliacion.TipoEntidad = item.TipoEntidad;
                        afiliacion.LaboralId = laboralId;
                        _afiliaciones.Add(afiliacion);
                        id++;
                    }
                    await context.AddRangeAsync(_afiliaciones);

                    maxId = await context.Col_Usuarios.MaxAsync(m => (int?)m.Id);
                    id = maxId == null ? 1 : maxId + 1;
                    Col_Usuarios usuario = new Col_Usuarios();
                    var numero = await context.Col_Personas
                        .Where(w => w.PrimerApellido.ToLower() == persona.PrimerApellido.ToLower())
                        .Select(s => s.PersonaId)
                        .MaxAsync();
                    usuario.RolId = _rol.RolId;
                    usuario.Contrasena = TokenProvider.SHA256(persona.NumeroDocumento);
                    usuario.Usuario = (persona.PrimerNombre.Substring(0, 1) + persona.PrimerApellido + (numero + 1).ToString()).ToLower();
                    usuario.Estado = "A";
                    usuario.Id = Convert.ToInt32(id);
                    usuario.UltimaContrasena = null;
                    usuario.UltimoLogin = null;
                    usuario.FechaActualizacion = null;
                    usuario.FechaCreacion = DateTime.Now;
                    await context.AddAsync<Col_Usuarios>(usuario);

                    var _persona = await context.Col_Personas
                        .Where(w => w.NumeroDocumento.Equals(persona.NumeroDocumento) && w.TipoDocumento.Equals(persona.TipoDocumento))
                        .FirstOrDefaultAsync();
                    _persona.Estado = "A";
                    _persona.FechaActualizacion = DateTime.Now;
                    _persona.UsuarioId = usuario.Id;
                    _persona.Progreso = 'A';
                    context.Col_Personas.Update(_persona);

                    await context.SaveChangesAsync();

                    status = true;
                    title = "Éxitos";
                    message = "¡Se han guardado los datos con éxito!";
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

        public async Task<List<EmpleadosContratados>> MostrarEmpleados()
        {
            try
            {
                var empleados = await (from t0 in context.Col_Roles
                                       join t1 in context.Col_Usuarios on t0.RolId equals t1.RolId
                                       join t2 in context.Col_Personas on t1.Id equals t2.UsuarioId
                                       join t3 in context.Col_Laborales on t2.PersonaId equals t3.PersonaId
                                       where t0.Estado.Equals("A") && !t1.Estado.Equals("P") && !t2.Estado.Equals("P")
                                       select new EmpleadosContratados
                                       {
                                           Id = t2.PersonaId,
                                           Usuario = t1.Usuario,
                                           Estado = t2.Estado == "A" ? "ACTIVO" : "INACTIVO",
                                           Celular = t2.Celular,
                                           Correo = t3.CorreoCorporativo == "" ? t2.CorreoPersonal : t3.CorreoCorporativo,
                                           Creacion = t2.FechaCreacion,
                                           Ingreso = t3.FechaIngreso,
                                           Nombre = $"{t2.PrimerNombre} {t2.PrimerApellido}",
                                           Perfil = t0.NombreRol,
                                           UltimoLogin = t1.UltimoLogin,
                                           Progreso = t2.Progreso,
                                       }).ToListAsync();

                var postulados = await context.Col_Personas.Where(w => w.Estado.Equals("P"))
                    .Select(s => new EmpleadosContratados
                    {
                        Id = s.PersonaId,
                        Estado = "PENDIENTE",
                        Celular = s.Celular,
                        Correo = s.CorreoPersonal,
                        Creacion = s.FechaCreacion,
                        Nombre = $"{s.PrimerNombre} {s.PrimerApellido}",
                        Progreso = s.Progreso,
                    }).ToListAsync();

                var personas = empleados.Union(postulados);

                return personas.OrderByDescending(o => o.Creacion).ToList();
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

        public async Task<Progresos> MostrarPendientes(char progreso, int idPersona)
        {
            try
            {
                Progresos _progreso = new Progresos();
                _progreso.Persona = await context.Col_Personas.Where(w => w.PersonaId.Equals(idPersona) && !w.Estado.Equals("I"))
                   .Select(s => new Col_Personas
                   {
                       PersonaId = s.PersonaId,
                       PrimerNombre = s.PrimerNombre,
                       SegundoNombre = s.SegundoNombre,
                       PrimerApellido = s.PrimerApellido,
                       SegundoApellido = s.SegundoApellido,
                       FechaNacimiento = s.FechaNacimiento,
                       Barrio = s.Barrio,
                       TipoDocumento = s.TipoDocumento,
                       NumeroDocumento = s.NumeroDocumento,
                       Estado = s.Estado,
                       Celular = s.Celular,
                       EstadoCivil = s.EstadoCivil,
                       CorreoPersonal = s.CorreoPersonal,
                       Direccion = s.Direccion,
                       Progreso = s.Progreso,
                   }).FirstOrDefaultAsync();
                if (_progreso.Persona != null)
                {
                    _progreso.InfoAcademicas = await context.Col_InfoAcademica.Where(w => w.PersonaId.Equals(idPersona))
                        .Select(s => new Col_InfoAcademica
                        {
                            AcademicoId = s.AcademicoId,
                            PersonaId = s.PersonaId,
                            FechaGradua = s.FechaGradua,
                            NivelFormacion = s.NivelFormacion,
                            NombreIns = s.NombreIns,
                            TituloObtenido = s.TituloObtenido,
                        }).ToListAsync();
                    if (progreso.Equals('E') || progreso.Equals('L') || progreso.Equals('A'))
                    {
                        _progreso.Experiencias = await context.Col_Experiencia.Where(w => w.PersonaId.Equals(idPersona))
                            .Select(s => new Col_Experiencia
                            {
                                PersonaId = s.PersonaId,
                                Cargo = s.Cargo,
                                Empresa = s.Empresa,
                                ExperienciaId = s.ExperienciaId,
                                FechaFin = s.FechaFin,
                                Funciones = s.Funciones,
                                FechaInicio = s.FechaInicio,
                                Logros = s.Logros,
                                Meses = (s.FechaFin.Year - s.FechaInicio.Year) * 12 + s.FechaFin.Month - s.FechaInicio.Month
                            }).ToListAsync();
                        if (progreso.Equals('L') || progreso.Equals('A'))
                        {
                            _progreso.Laboral = await context.Col_Laborales.Where(w => w.PersonaId.Equals(idPersona))
                                .Select(s => new Col_Laborales
                                {
                                    AuxilioTransporte = s.AuxilioTransporte,
                                    CorreoCorporativo = s.CorreoCorporativo,
                                    FechaBaja = s.FechaBaja,
                                    FechaIngreso = s.FechaIngreso,
                                    Horas = s.Horas,
                                    JornadaLaboral = s.JornadaLaboral,
                                    LaboralId = s.LaboralId,
                                    NombreCargo = s.NombreCargo,
                                    Salario = s.Salario,
                                    TipoContrato = s.TipoContrato,
                                }).FirstOrDefaultAsync();
                            _progreso.InsumosLaborales = await context.Col_InsumoLaboral
                                .Where(w => w.LaboralId.Equals(_progreso.Laboral.LaboralId))
                                .Select(s => new Col_InsumoLaboral
                                {
                                    InsLabId = s.InsLabId,
                                    LaboralId = s.LaboralId,
                                    Nombre = s.Nombre,
                                }).ToListAsync();
                            if (progreso.Equals('A'))
                            {
                                _progreso.Afiliaciones = await context.Col_Afiliacion
                                    .Where(w => w.LaboralId.Equals(_progreso.Laboral.LaboralId))
                                    .Select(s => new Col_Afiliacion
                                    {
                                        AfiliacionId = s.AfiliacionId,
                                        LaboralId = s.LaboralId,
                                        FechaActualizacion = s.FechaActualizacion,
                                        FechaAfiliacion = s.FechaAfiliacion,
                                        FechaCreacion = s.FechaCreacion,
                                        NombreEntidad = s.NombreEntidad,
                                        TipoEntidad = s.TipoEntidad,
                                    }).ToListAsync();
                            }
                        }
                    }
                }
                return _progreso;
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

        public async Task<int> ActualizarPersonales(Col_Personas persona, List<Col_InfoAcademica> infoAcademicas, int personaActualizar)
        {
            try
            {
                var _persona = await context.Col_Personas.Where(w => w.PersonaId.Equals(personaActualizar)).FirstOrDefaultAsync();
                if (_persona != null)
                {
                    _persona.PersonaId = personaActualizar;
                    _persona.PrimerNombre = persona.PrimerNombre;
                    _persona.Barrio = persona.Barrio;
                    _persona.NumeroDocumento = persona.NumeroDocumento;
                    _persona.Celular = persona.Celular;
                    _persona.CorreoPersonal = persona.CorreoPersonal;
                    _persona.Direccion = persona.Direccion;
                    _persona.EstadoCivil = persona.EstadoCivil;
                    _persona.FechaNacimiento = persona.FechaNacimiento;
                    _persona.TipoDocumento = persona.TipoDocumento;
                    _persona.SegundoNombre = persona.SegundoNombre;
                    _persona.PrimerApellido = persona.PrimerApellido;
                    _persona.SegundoApellido = persona.SegundoApellido;
                    _persona.FechaActualizacion = DateTime.Now;
                    context.Col_Personas.Update(_persona);
                    var _infoAcademica = await context.Col_InfoAcademica.Where(w => w.PersonaId.Equals(personaActualizar)).ToListAsync();
                    if (_infoAcademica.Count() > 0)
                    {
                        context.Col_InfoAcademica.RemoveRange(_infoAcademica);
                    }
                    int? maxId = await context.Col_InfoAcademica.MaxAsync(m => (int?)m.AcademicoId);
                    int? id = maxId == null ? 1 : maxId + 1;
                    List<Col_InfoAcademica> _infoAcademicas = new List<Col_InfoAcademica>();
                    foreach (var item in infoAcademicas)
                    {
                        Col_InfoAcademica infoAcademica = new Col_InfoAcademica();
                        infoAcademica.AcademicoId = Convert.ToInt32(id);
                        infoAcademica.FechaGradua = item.FechaGradua;
                        infoAcademica.NivelFormacion = item.NivelFormacion;
                        infoAcademica.NombreIns = item.NombreIns;
                        infoAcademica.PersonaId = _persona.PersonaId;
                        infoAcademica.TituloObtenido = item.TituloObtenido;
                        _infoAcademicas.Add(infoAcademica);
                        id++;
                    }
                    await context.AddRangeAsync(_infoAcademicas);
                    await context.SaveChangesAsync();
                    return _persona.PersonaId;
                }
                return 0;
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
                return 0;
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
                return 0;
            }
            #endregion
        }

        public async Task<bool> ActualizarExperiencia(List<Col_Experiencia> experiencias, int experienciaActualizar)
        {
            try
            {
                var _experiencia = await context.Col_Experiencia.Where(w => w.PersonaId.Equals(experienciaActualizar)).ToListAsync();
                if (_experiencia.Count() > 0)
                {
                    context.Col_Experiencia.RemoveRange(_experiencia);
                    int? maxId = await context.Col_Experiencia.MaxAsync(m => (int?)m.ExperienciaId);
                    int? id = maxId == null ? 1 : maxId + 1;
                    List<Col_Experiencia> _experiencias = new List<Col_Experiencia>();
                    foreach (var item in experiencias)
                    {
                        Col_Experiencia experiencia = new Col_Experiencia();
                        experiencia.ExperienciaId = Convert.ToInt32(id);
                        experiencia.PersonaId = experienciaActualizar;
                        experiencia.Cargo = item.Cargo;
                        experiencia.Empresa = item.Empresa;
                        experiencia.FechaFin = item.FechaFin;
                        experiencia.FechaInicio = item.FechaInicio;
                        experiencia.Funciones = item.Funciones;
                        experiencia.Logros = item.Logros;
                        _experiencias.Add(experiencia);
                        id++;
                    }
                    await context.AddRangeAsync(_experiencias);
                    await context.SaveChangesAsync();
                    return true;
                }
                return false;
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
                return false;
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
                return false;
            }
            #endregion
        }

        public async Task<int> ActualizarLaboborales(Col_Laborales laboral, List<Col_InsumoLaboral> insumos, int laboralActualizar)
        {
            try
            {
                var _laboral = await context.Col_Laborales.Where(w => w.LaboralId.Equals(laboralActualizar)).FirstOrDefaultAsync();
                if (_laboral != null)
                {
                    _laboral.AuxilioTransporte = laboral.AuxilioTransporte;
                    _laboral.CorreoCorporativo = laboral.CorreoCorporativo;
                    _laboral.FechaBaja = laboral.FechaBaja;
                    _laboral.FechaIngreso = laboral.FechaIngreso;
                    _laboral.Horas = laboral.Horas;
                    _laboral.JornadaLaboral = laboral.JornadaLaboral;
                    _laboral.NombreCargo = laboral.NombreCargo;
                    _laboral.Salario = laboral.Salario;
                    _laboral.TipoContrato = laboral.TipoContrato;
                    context.Col_Laborales.Update(_laboral);
                    var _insumoEstado = await context.Col_InsumoLaboral.Where(w => w.LaboralId.Equals(laboralActualizar)).ToListAsync();
                    context.Col_InsumoLaboral.RemoveRange(_insumoEstado);
                    if (insumos != null)
                    {
                        List<Col_InsumoLaboral> _insumos = null;
                        _insumos = new List<Col_InsumoLaboral>();
                        int? maxId = await context.Col_InsumoLaboral.MaxAsync(m => (int?)m.InsLabId);
                        int? id = maxId == null ? 1 : maxId + 1;
                        foreach (var item in insumos)
                        {
                            Col_InsumoLaboral insumo = new Col_InsumoLaboral();
                            insumo.InsLabId = Convert.ToInt32(id);
                            insumo.Nombre = item.Nombre;
                            insumo.LaboralId = _laboral.LaboralId;
                            _insumos.Add(insumo);
                            id++;
                        }
                        await context.AddRangeAsync(_insumos);
                    }
                    await context.SaveChangesAsync();
                    return _laboral.LaboralId;
                }
                return 0;
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
                return 0;
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
                return 0;
            }
            #endregion
        }

        public async Task<ApiCallResult> ActualizarAfiliciacion(List<Col_Afiliacion> afiliaciones, int afiliacioneActualizar)
        {
            try
            {
                var _afiliacion = await context.Col_Afiliacion.Where(w => w.LaboralId.Equals(afiliacioneActualizar)).ToListAsync();
                if (_afiliacion.Count() > 0)
                {
                    context.Col_Afiliacion.RemoveRange(_afiliacion);
                }
                int? maxId = await context.Col_Afiliacion.MaxAsync(m => (int?)m.AfiliacionId);
                int? id = maxId == null ? 1 : maxId + 1;
                List<Col_Afiliacion> _afiliaciones = new List<Col_Afiliacion>();
                foreach (var item in afiliaciones)
                {
                    Col_Afiliacion afiliacion = new Col_Afiliacion();
                    afiliacion.AfiliacionId = Convert.ToInt32(id);
                    afiliacion.FechaActualizacion = null;
                    afiliacion.FechaAfiliacion = item.FechaAfiliacion;
                    afiliacion.FechaCreacion = DateTime.Now;
                    afiliacion.NombreEntidad = item.NombreEntidad;
                    afiliacion.TipoEntidad = item.TipoEntidad;
                    afiliacion.LaboralId = afiliacioneActualizar;
                    _afiliaciones.Add(afiliacion);
                    id++;
                }
                await context.AddRangeAsync(_afiliaciones);
                await context.SaveChangesAsync();

                bool status = true;
                string title = "Éxitos";
                string message = "¡Se han actualizado los datos con éxito!";
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

        public async Task<ApiCallResult> EliminarEmpleado(int personaId, bool op)
        {
            try
            {
                bool status = false;
                string title = "Error";
                string message = "Error al eliminar el empleado";
                var persona = await context.Col_Personas.Where(w => w.PersonaId.Equals(personaId)).FirstOrDefaultAsync();
                switch (op)
                {
                    case true:
                        var academicos = await context.Col_InfoAcademica.Where(w => w.PersonaId.Equals(persona.PersonaId)).ToListAsync();
                        var usuario = await context.Col_Usuarios.Where(w => w.Id.Equals(persona.UsuarioId)).FirstOrDefaultAsync();
                        var experiencia = await context.Col_Experiencia.Where(w => w.PersonaId.Equals(persona.PersonaId)).ToListAsync();
                        var laboral = await context.Col_Laborales.Where(w => w.PersonaId.Equals(persona.PersonaId)).FirstOrDefaultAsync();
                        context.Col_Personas.Remove(persona);
                        if (academicos.Count() > 0) context.Col_InfoAcademica.RemoveRange(academicos);
                        if (experiencia.Count() > 0) context.Col_Experiencia.RemoveRange(experiencia);
                        if (laboral != null)
                        {
                            var insumos = await context.Col_InsumoLaboral.Where(w => w.LaboralId.Equals(laboral.LaboralId)).ToListAsync();
                            var afiliciones = await context.Col_Afiliacion.Where(w => w.LaboralId.Equals(laboral.LaboralId)).ToListAsync();
                            if (insumos.Count() > 0) context.Col_InsumoLaboral.RemoveRange(insumos);
                            if (afiliciones.Count() > 0)
                            {
                                context.Col_Afiliacion.RemoveRange(afiliciones);
                                context.Col_Usuarios.Remove(usuario);
                            }
                            context.Col_Laborales.Remove(laboral);
                        }
                        status = true;
                        title = "Éxito";
                        message = $"El empleado con # de documento {persona.NumeroDocumento} se ha eliminado exitosamente";
                        break;
                    default:
                        persona.Estado = "I";
                        persona.FechaActualizacion = DateTime.Now;
                        context.Col_Personas.Update(persona);
                        status = true;
                        title = "Éxito";
                        message = $"El empleado con # de documento {persona.NumeroDocumento} se ha inhabilitó exitosamente";
                        break;
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

        public async Task<ApiCallResult> ActivarPerfil(int personaId)
        {
            var empleado = await context.Col_Personas.Where(w => w.PersonaId.Equals(personaId)).FirstOrDefaultAsync();
            empleado.Estado = "A";
            empleado.FechaActualizacion = DateTime.Now;
            context.Col_Personas.Update(empleado);
            await context.SaveChangesAsync();

            return new ApiCallResult
            {
                Status = true,
                Title = "Exitos",
                Message = $"El empleado {empleado.NumeroDocumento} se habitlitó exitosamente"
            };
        }
    }
}
