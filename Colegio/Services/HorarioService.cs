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
                    Message = $"El suministro {materia.Nombre} - {materia.Codigo} se ha eliminado con éxito"
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
    }
}
