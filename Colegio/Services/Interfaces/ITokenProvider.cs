using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Colegio.Services.Interfaces
{
    public interface ITokenProvider
    {
        ClaimsIdentity LoginUser(string usuario, string contrasena);
    }
}
