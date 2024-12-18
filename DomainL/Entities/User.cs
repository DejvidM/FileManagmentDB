using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainL.Entities
{
    public class User
    {
        public int Id { get; set; }
        public required string Username { get; set; } 
        public required string Email { get; set; } 
        public required string Password { get; set; } 
        public List<Folder> Folders { get; set; } = new List<Folder>();
        public List<DbFile> Files { get; set; } = new List<DbFile>();

    }
}
