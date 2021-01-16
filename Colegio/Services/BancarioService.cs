using Colegio.Data;
using Colegio.Models.ModelHelper;
using Colegio.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colegio.Services
{
    public class BancarioService : IBancario
    {
        private readonly ColegioContext context;

        public BancarioService(ColegioContext context)
        {
            this.context = context;
        }

        public async Task<ApiCallResult> GuardarCuenta(string nombre, string titular, string numero)
        {
            return null;
        }
    }
}
