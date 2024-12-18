using DomainL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceL.DTO
{
    public class UserDTO
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public List<Folder> Folders { get; set; }
    }
}
