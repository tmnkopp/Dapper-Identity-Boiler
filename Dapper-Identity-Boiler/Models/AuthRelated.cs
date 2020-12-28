using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DIB.Models
{
    public class AuthRelated
    {
    } 
    public class RegisterUser
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
