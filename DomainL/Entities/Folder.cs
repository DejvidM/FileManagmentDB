using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DomainL.Entities
{
    public class Folder
    {
        public int Id { get; set; }
        public required string Name { get; set; } 
        public required int UserId { get; set; }
        public User User { get; set; }
        public int? ParentId { get; set; }
        [JsonIgnore]
        public Folder? ParentFolder { get; set; }
        public List<Folder> Folders { get; set; } = null!;
        public List<DbFile> Files { get; set; } = null!;
    }
}
