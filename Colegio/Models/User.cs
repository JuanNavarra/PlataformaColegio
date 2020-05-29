using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colegio.Models
{
    public class User
    {
        public string UserId { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string AccesLevel { get; set; }
        public string ReadOnly { get; set; }
    }
}
