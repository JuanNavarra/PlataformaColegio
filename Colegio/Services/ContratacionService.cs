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
                    persona.Estado = "A";//Pendienete
                    persona.FechaCreacion = DateTime.Now;
                    persona.Genero = "M";
                    persona.FechaActualizacion = null;
                    persona.Empleado = true;
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

        public async Task<ApiCallResult> GuardarAfiliacion(List<Col_Afiliacion> afiliaciones, int rol, int laboralId, string primerNombre, string primerApellido, string numeroDocumento)
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
                    usuario.RolId = _rol.RolId;
                    usuario.Contrasena = numeroDocumento;
                    usuario.Usuario = primerNombre.Substring(0, 1) + primerApellido + id.ToString();
                    usuario.Estado = "A";
                    usuario.Id = Convert.ToInt32(id);
                    usuario.UltimaContrasena = null;
                    usuario.UltimoLogin = null;
                    usuario.FechaActualizacion = null;
                    usuario.FechaCreacion = DateTime.Now;
                    await context.AddAsync<Col_Usuarios>(usuario);
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
    }
}
