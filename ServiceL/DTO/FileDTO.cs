namespace ServiceL.DTO
{
    public class FileDTO
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string FileType { get; set; }
        public required long FileSize { get; set; }
        public required byte[] FileData { get; set; }
        public required int UserId { get; set; }
        public required int FolderId { get; set; }

    }
}
