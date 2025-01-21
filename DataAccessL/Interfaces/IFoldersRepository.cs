using DomainL.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessL.Interfaces
{
    public interface IFoldersRepository
    {
        Task<IEnumerable<Folder>> GetAllAsync();
        Task<Folder> AddAsyncInDb(Folder folder);
        Task<bool> ExistsWithNameInDirectory(string name, int? parentId, int userId);
        Task<bool> ExistsWithNameInDirectory(string name, int? parentId, int userId, int folderId);
        Task<IEnumerable<Folder>> GetChildrenFolders(Folder parentFolder);
        Task<Folder?> GetByIdAsync(int id);
        Task DeleteAsync(int folderId);
        Task<Folder> RenameFolder(Folder folder, string newName);
        Task<List<Folder>> GetAllUserFoldersFiles(int userId);
        Task<List<Folder>> FindNestedFolders(int folderId, HashSet<int> visitedFolderIds);
        Task<Folder> MoveFolder(Folder originFolder, Folder destinationFolder);

    }
}
