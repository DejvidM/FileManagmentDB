using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ServiceL.DTO
{
    public class CreatingFolderDTO
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required int UserId { get; set; }
        public required int? ParentId { get; set; }
    }
}
