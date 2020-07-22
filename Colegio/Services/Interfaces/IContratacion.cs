using Colegio.Models;
using Colegio.Models.ModelHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colegio.Services.Interfaces
{
    public interface IContratacion
    {
        public Task<int> GuardarPersonales(Col_Personas persona, List<Col_InfoAcademica> infoAcademicas);
    }
}
