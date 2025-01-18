using DataAccessL.Interfaces;
using DomainL.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ServiceL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceL.DTO;

namespace ServiceL.Services
{
    public class FolderService : IFolderService
    {
        private readonly IFoldersRepository _foldersRepository; 
        private readonly IDbFileRepository _dbFileRepository;
        public FolderService(IFoldersRepository foldersRepository, IDbFileRepository dbFileRepository)
        {
            _foldersRepository = foldersRepository;
            _dbFileRepository = dbFileRepository;
        }

        public async Task<IEnumerable<FolderDTO>> GetAllFoldersAsync()
        {
            var folders = await _foldersRepository.GetAllAsync();
            List<FolderDTO> folderDTO = new List<FolderDTO>();

            foreach(var folder in folders)
            {
                folderDTO.Add(new FolderDTO 
                { 
                    Id = folder.Id,
                    Name = folder.Name,
                    UserId = folder.UserId,
                    ParentId = folder.ParentId
                });
            }

            return folderDTO;
        }
        public async Task<FolderDTO> AddFolderAsync(FolderDTO folderDTO)
        {
            if (folderDTO == null) 
            {
                throw new Exception("Folder can not be null!");
            }

            var folder = new Folder
            {
                Name = folderDTO.Name,
                UserId = folderDTO.UserId,
                ParentId = folderDTO.ParentId
            };

            var addedFolder = await _foldersRepository.AddAsync(folder);

            return new FolderDTO 
            { 
                Id = addedFolder.Id,
                Name = addedFolder.Name,
                UserId = addedFolder.UserId,
                ParentId = addedFolder.ParentId 
            };
        }

        public async Task<bool> DeleteFoldersAsync(int id)
        {
            var folder = await _foldersRepository.GetByIdAsync(id);

            if (folder == null)
            {
                throw new Exception("Folder could not be found.");
            }
            try
            {
                var folderIds = new HashSet<int>();
                var nestedFolders = await _foldersRepository.FindNestedFolders(id, folderIds);

                var folderList = folderIds.ToList();
                folderList.Reverse();

                foreach(var folderId in folderList)
                {
                    await _foldersRepository.DeleteAsync(folderId);
                }

                return true;
            }
            catch (Exception ex) 
            {
                throw new Exception("Error " + ex);
            }

        }

        public async Task<Folder> GetFolderByIdAsync(int id)
        {
            var visitedFolderIds = new HashSet<int>();

            return await GetFolderByIdWithCycleCheckAsync(id, visitedFolderIds);
        }

        private async Task<Folder> GetFolderByIdWithCycleCheckAsync(int id, HashSet<int> visitedFolderIds)
        {
            var folder = await _foldersRepository.GetByIdAsync(id);
            if (folder == null)
                return null;

            folder.Files = await _dbFileRepository.GetFolderFiles(id);

            folder.Folders = await _foldersRepository.FindNestedFolders(id, visitedFolderIds);

            return folder;
        }


        public async Task<Folder> RenameFolderAsync(RenameFolderDTO renameFolderDTO)
        {
            var folder = await _foldersRepository.GetByIdAsync(renameFolderDTO.FolderId);
            if (folder != null)
            {
                return await _foldersRepository.RenameFolder(folder, renameFolderDTO.NewName);
                
            }

            return null;

        }

        public async Task<Folder> MoveFolderAsync(int originFolderId, int destinationFolderId)
        {
            var originFolder = await _foldersRepository.GetByIdAsync(originFolderId);
            var destinationFolder = await _foldersRepository.GetByIdAsync(destinationFolderId);

            var visitedFoldersId = new HashSet<int>();

            if(originFolder == null || destinationFolder == null)
            {
                throw new Exception("One of the folders' Id is incorrect");
            }

            var nestedOriginFolders = await _foldersRepository.FindNestedFolders(originFolderId, visitedFoldersId);
            if (visitedFoldersId.Contains(destinationFolderId))
            {
                throw new Exception("Parent folder can not go to child folder");
            }

            return await _foldersRepository.MoveFolder(originFolder, destinationFolder);
        }
        
        public async Task<FolderDTO> UploadFolderAsync(FolderDTO folderDTO)
        {
            var folder = await AddFolderAsync(new FolderDTO
            {
                Name = folderDTO.Name,
                ParentId = folderDTO.ParentId,
                UserId = folderDTO.UserId
            });
            
            if(folderDTO.FileList.Count > 0)
            {
                foreach (var file in folderDTO.FileList) 
                {

                    var response = await _dbFileRepository.AddAsync(new DbFile
                    {
                        Name = file.Name,
                        FileType = file.FileType,
                        FileSize = file.FileSize,
                        FileData = file.FileData,
                        UserId = folderDTO.UserId,
                        FolderId = folder.Id
                    });
                }
                folder.FileList = folderDTO.FileList;
            }
            else
            {
                folder.FileList = [];
            }

            if (folderDTO.FolderChildrenList.Count > 0)
            {
                foreach (var child in folderDTO.FolderChildrenList)
                {
                    var childFolder = await UploadFolderAsync(child);
                    folder.FolderChildrenList.Add(childFolder);
                }
            }
            else
            {
                folder.FolderChildrenList = [];
            }

            return folder;
        }

    }
}
