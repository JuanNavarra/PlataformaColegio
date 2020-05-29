using Colegio.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;


namespace Colegio.Services
{
    public class TokenProvider
    {
        public TokenProvider()
        {

        }
        public string LoginUser(string UserID, string Password)
        {
            //Get user details for the user who is trying to login
            var user = UserList.SingleOrDefault(x => x.UserId == UserID);

            //Authenticate User, Check if it’s a registered user in Database 
            if (user == null)
                return null;

            //If it is registered user, check user password stored in Database
            //For demo, password is not hashed. It is just a string comparision 
            //In reality, password would be hashed and stored in Database. 
            //Before comparing, hash the password again.
            if (Password == user.Password)
            {
                //Authentication successful, Issue Token with user credentials 
                //Provide the security key which is given in 
                //Startup.cs ConfigureServices() method 
                var key = Encoding.ASCII.GetBytes
                ("YourKey-2374-OFFKDI940NG7:56753253-tyuw-5769-0921-kfirox29zoxv");
                //Generate Token for user 
                var JWToken = new JwtSecurityToken(
                    issuer: "http://localhost:45092/",
                    audience: "http://localhost:45092/",
                    claims: GetUserClaims(user),
                    notBefore: new DateTimeOffset(DateTime.Now).DateTime,
                    expires: new DateTimeOffset(DateTime.Now.AddDays(1)).DateTime,
                    //Using HS256 Algorithm to encrypt Token  
                    signingCredentials: new SigningCredentials
                    (new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                );
                var token = new JwtSecurityTokenHandler().WriteToken(JWToken);
                return token;
            }
            else
            {
                return null;
            }
        }

        //Using hard coded collection list as Data Store for demo. 
        //In reality, User details would come from Database.
        private List<User> UserList = new List<User>
        {
            new User {
                    UserId = "jsmith@email.com",
                    Password = "test", Email = "jsmith@email.com",
                    FirstName = "John", LastName = "Smith",
                    Phone = "356-735-2748", AccesLevel = "Director",
                    ReadOnly = "true"
            }
        };

        //Using hard coded collection list as Data Store for demo. 
        //In reality, User data comes from Database or other Data Source 
        private IEnumerable<Claim> GetUserClaims(User user)
        {
            IEnumerable<Claim> claims = new Claim[]
                    {
                    new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName),
                    new Claim("USERID", user.UserId),
                    new Claim("EMAILID", user.Email),
                    new Claim("PHONE", user.Phone),
                    new Claim("ACCESS_LEVEL", user.AccesLevel.ToUpper()),
                    new Claim("READ_ONLY", user.ReadOnly.ToUpper())
                    };
            return claims;
        }
    }
}
