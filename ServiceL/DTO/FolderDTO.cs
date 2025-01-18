using DomainL.Entities;

namespace ServiceL.DTO
{
    public class FolderDTO
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required int UserId { get; set; }
        public int? ParentId { get; set; }
        public List<FolderDTO>? FolderChildrenList { get; set; }
        public List<FileDTO>? FileList { get; set; }

    }
}
