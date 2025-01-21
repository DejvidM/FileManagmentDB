using DomainL.Entities;
using Microsoft.AspNetCore.Mvc;
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
        Task<CreatingFolderDTO> AddFolderAsync(CreatingFolderDTO creatingFolderDTO);
        Task<Folder?> GetFolderByIdAsync(int id);
        Task<IEnumerable<FolderDTO>> GetAllFoldersAsync();
        Task<bool> DeleteFoldersAsync(int[] folderIds);
        Task<Folder?> RenameFolderAsync(RenameFolderDTO renameFolderDTO, int userId);
        Task<Folder> MoveFolderAsync(int originFolderId, int destinationFolderId);
        Task<IEnumerable<Folder>> SearchForFolderAsync(string folderName, int userId);
        Task<bool> UploadFolderAsync(FolderDTO folderDTO);

    }
}
