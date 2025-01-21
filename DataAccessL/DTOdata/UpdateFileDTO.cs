using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessL.DTOdata
{
    public class UpdateFileDTO
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string FileType { get; set; }
        public required long FileSize { get; set; }
        public required byte[] FileData { get; set; }

    }
}
