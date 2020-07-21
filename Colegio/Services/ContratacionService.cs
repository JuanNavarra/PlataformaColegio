using Colegio.Data;
using Colegio.Services.Interfaces;
using System;
using System.Collections.Generic;
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
    }
}
