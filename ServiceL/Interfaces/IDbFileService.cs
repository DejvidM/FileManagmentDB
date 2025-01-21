using DataAccessL.DTOdata;
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
        Task<DbFile?> GetFileByIdAsync(int id);
        Task<DbFile> AddFileAsync(FileDTO fileDTO);
        Task<RemovingFileDTO> RemoveFileAsync(int[] id);
        Task<IEnumerable<DbFile>> GetFolderFilesAsync(int folderId);
        Task<bool> MoveFileAsync(int fileId, int folderId);
        Task<IEnumerable<DbFile>> SearchFilesAsync(string name, int userId);
        Task<UpdateFileDTO?> UpdateFileAsync(UpdateFileDTO ufd);
        Task<bool> RollbackToPreviousVersionAsync(int fileId);
        public string GetMimeType(string filePath);

    }
}
