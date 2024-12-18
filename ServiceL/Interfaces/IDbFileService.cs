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
        Task<DbFile> AddFileAsync(FileDTO fileDTO);
        Task<bool> RemoveFileAsync(int id);
        Task<IEnumerable<DbFile>> GetAllFilesAsync();
        Task<DbFile> GetFileByIdAsync(int id);
        public string GetMimeType(string filePath);
        Task<IEnumerable<DbFile>> GetFolderFiles(int folderId);

    }
}
