using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceL.DTO
{
    public class RenameFolderDTO
    {
        public required int FolderId { get; set; } 
        public required string NewName { get; set; }
        
    }
}
