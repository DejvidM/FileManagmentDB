using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainL.Entities
{
    public class FileVersion
    {
        public int Id { get; set; }
        public int DbFileId { get; set; } 
        public DbFile DbFile { get; set; }
        public required string Name { get; set; }
        public required string FileType { get; set; }
        public long FileSize { get; set; }
        public required byte[] FileData { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? ChangeDescription { get; set; }
    }
}
