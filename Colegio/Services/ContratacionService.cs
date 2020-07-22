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
                    persona.Estado = "A";
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
    }
}
