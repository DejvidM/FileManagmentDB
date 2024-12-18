using DomainL.Entities;
using ServiceL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceL.Services
{
    public class PasswordHashCompare : IPasswordHashCompare
    {
        public string HashPasswordAsync(string passsword) 
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(passsword);
            return hashedPassword; 
        } 

        public bool ComparePasswords(string password, User user )
        {
            bool isMatch = BCrypt.Net.BCrypt.Verify(password, user.Password);
            return isMatch;
        }
    }
}
