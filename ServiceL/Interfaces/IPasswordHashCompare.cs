using DomainL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceL.Interfaces
{
    public interface IPasswordHashCompare
    {
        public string HashPasswordAsync(string password);
        public bool ComparePasswords(string password, User user);
        
    }
}
