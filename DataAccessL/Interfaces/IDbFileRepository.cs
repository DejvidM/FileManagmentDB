using DataAccessL.DTOdata;
using DomainL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessL.Interfaces
{
    public interface IDbFileRepository
    {
        Task<IEnumerable<DbFile>> GetAllAsync();
        Task<DbFile?> GetByIdAsync(int id);
        Task<DbFile> AddFileInDb(DbFile dbFile);
        Task RemoveAsync(DbFile dbFile);
        Task<List<DbFile>> GetFolderFiles(int folderId);
        Task<bool> MoveFileInFolder(int fileId, int folderId);
        Task<UpdateFileDTO> UpdateFileInDb(DbFile dbFile, UpdateFileDTO ufd);
        Task<bool> RollbackFileVersion(DbFile dbFile);
    }
}
