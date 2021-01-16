using Colegio.Models.ModelHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colegio.Services.Interfaces
{
    public interface IBancario
    {
        public Task<ApiCallResult> GuardarCuenta(string nombre, string titular, string numero);
    }
}
