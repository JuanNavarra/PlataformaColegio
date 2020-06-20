using System.Security.Claims;

namespace Colegio.Services.Interfaces
{
    public interface ITokenProvider
    {
        ClaimsIdentity LoginUser(string usuario, string contrasena);
    }
}
