using System.Security.Claims;
using System.Threading.Tasks;

namespace Colegio.Services.Interfaces
{
    public interface ITokenProvider
    {
        public Task<ClaimsIdentity> LoginUser(string usuario, string contrasena);
    }
}
