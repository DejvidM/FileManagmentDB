using DomainL.Entities;
using ServiceL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceL.Interfaces
{
    public interface IFolderService
    {
        Task<FolderDTO> AddFolderAsync(FolderDTO folderDTO);
        Task<Folder> GetFolderByIdAsync(int id);
        Task<IEnumerable<FolderDTO>> GetAllFoldersAsync();
        Task<bool> DeleteFoldersAsync(int id);
        Task<Folder> RenameFolderAsync(RenameFolderDTO renameFolderDTO);
    }
}
