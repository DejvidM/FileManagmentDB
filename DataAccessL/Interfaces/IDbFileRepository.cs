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
        Task<DbFile> AddAsync(DbFile dbFile);
        Task<bool> RemoveAsync(DbFile dbFile);
        Task<IEnumerable<DbFile>> GetAllAsync();
        Task<DbFile> GetByIdAsync(int id);

        Task<List<DbFile>> GetFolderFiles(int folderId);
    }
}
