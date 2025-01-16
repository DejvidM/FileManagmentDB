using DomainL.Entities;
using ServiceL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceL.Interfaces
{
    public interface IDbFileService
    {
        Task<IEnumerable<DbFile>> GetAllFilesAsync();
        Task<DbFile> GetFileByIdAsync(int id);
        Task<DbFile> AddFileAsync(FileDTO fileDTO);
        Task<RemovingFileDTO> RemoveFileAsync(int[] id);
        Task<IEnumerable<DbFile>> GetFolderFilesAsync(int folderId);
        Task<int> MoveFileAsync(int fileId, int folderId);
        public string GetMimeType(string filePath);

    }
}
