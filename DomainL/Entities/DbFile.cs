using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainL.Entities
{
    public class DbFile
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string FileType { get; set; } 
        public long FileSize { get; set; }
        public required byte[] FileData { get; set; } 
        public required int UserId { get; set; }
        public User User { get; set; }
        public required int FolderId { get; set; }
        public Folder Folder { get; set; }
        public ICollection<FileVersion> Versions { get; set; } = new List<FileVersion>();


    }
}
