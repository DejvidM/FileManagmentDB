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
using Microsoft.AspNetCore.Mvc;

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

            return folders.Select(folder => new FolderDTO
                {
                    Id = folder.Id,
                    Name = folder.Name,
                    UserId = folder.UserId,
                    ParentId = folder.ParentId
                }).ToList();   
        }
        public async Task<Folder?> GetFolderByIdAsync(int id)
        {
            var visitedFolderIds = new HashSet<int>();

            return await GetFolderByIdWithCycleCheckAsync(id, visitedFolderIds);
        }

        private async Task<Folder?> GetFolderByIdWithCycleCheckAsync(int id, HashSet<int> visitedFolderIds)
        {
            var folder = await _foldersRepository.GetByIdAsync(id);

            if (folder == null)
                return null;

            folder.Files = await _dbFileRepository.GetFolderFiles(id);

            folder.Folders = await _foldersRepository.FindNestedFolders(id, visitedFolderIds);

            return folder;
        }
        public async Task<CreatingFolderDTO> AddFolderAsync(CreatingFolderDTO creatingFolderDTO)
        {
            if (creatingFolderDTO == null) 
                throw new Exception("Folder can not be null!");

            if (String.IsNullOrEmpty(creatingFolderDTO.Name))
                throw new Exception("Name can not be empty");

            var duplicateExists = await _foldersRepository.ExistsWithNameInDirectory(
                creatingFolderDTO.Name,
                creatingFolderDTO.ParentId,
                creatingFolderDTO.UserId
            );
            
            if (duplicateExists)
                throw new Exception($"A folder named '{creatingFolderDTO.Name}' already exists in this directory");
            
            var folder = new Folder
            {
                Name = creatingFolderDTO.Name,
                UserId = creatingFolderDTO.UserId,
                ParentId = creatingFolderDTO.ParentId
            };

            var addedFolder = await _foldersRepository.AddAsyncInDb(folder);

            return new CreatingFolderDTO 
            { 
                Id = addedFolder.Id,
                Name = addedFolder.Name,
                UserId = addedFolder.UserId,
                ParentId = addedFolder.ParentId 
            };
        }

        public async Task<bool> DeleteFoldersAsync(int[] folderIds)
        {
            if (folderIds.Length == 0)
            {
                return false;
            }
            
            foreach(int id in folderIds)
            {
                var folder = await _foldersRepository.GetByIdAsync(id);

                if (folder == null)
                {
                    throw new Exception($"Folder with Id {id} does not exist. Enter correct Ids!");
                }

                var visitedFolderIds = new HashSet<int>();
                var nestedFolders = await _foldersRepository.FindNestedFolders(id, visitedFolderIds);

                var folderListIds = visitedFolderIds.ToList();

                if (folderListIds.Skip(1).Any(fId => folderIds.Contains(fId)))
                {
                    throw new Exception("Subfolders will be deleted automatically. Delete only the main ones!");
                }

                folderListIds.Reverse();

                foreach(var folderId in folderListIds)
                {
                    await _foldersRepository.DeleteAsync(folderId);
                }
            }
            return true;
        }



        public async Task<Folder?> RenameFolderAsync(RenameFolderDTO renameFolderDTO, int userId)
        {
            var folder = await _foldersRepository.GetByIdAsync(renameFolderDTO.FolderId);

            if (folder == null)
                return null;
                
            var duplicateExists = await _foldersRepository.ExistsWithNameInDirectory(   
                renameFolderDTO.NewName,
                folder.ParentId,
                userId,
                folder.Id
                );

            if (duplicateExists)                
                throw new Exception($"A folder named '{renameFolderDTO.NewName}' already exists in this directory");

            return await _foldersRepository.RenameFolder(folder, renameFolderDTO.NewName);
        }

        public async Task<Folder> MoveFolderAsync(int originFolderId, int destinationFolderId)
        {
            var originFolder = await _foldersRepository.GetByIdAsync(originFolderId);

            var destinationFolder = await _foldersRepository.GetByIdAsync(destinationFolderId);


            if(originFolder == null || destinationFolder == null)
            {
                throw new Exception("One of the folders' Id is incorrect");
            }

            if (originFolder.ParentId == destinationFolderId)
            {
                throw new Exception("Can not move");
            }

            var visitedFoldersId = new HashSet<int>();

            var nestedOriginFolders = await _foldersRepository.FindNestedFolders(originFolderId, visitedFoldersId);
            
            if (visitedFoldersId.Contains(destinationFolderId))
            {
                throw new Exception("Parent folder can not go to child folder");
            }

            var destinationFolderChildren = await _foldersRepository.GetChildrenFolders(destinationFolder);

            if (destinationFolderChildren.Any(f => f.Name == originFolder.Name))
                throw new Exception("A folder with the same name already exists in this directory");

            return await _foldersRepository.MoveFolder(originFolder, destinationFolder);
        }

        public async Task<IEnumerable<Folder>> SearchForFolderAsync(string folderName, int userId)
        {
            var folders = await GetAllFoldersAsync();

            var matchingFolders = folders
                .Where(f => f.Name.StartsWith(folderName, StringComparison.OrdinalIgnoreCase) && f.UserId == userId)
                .OrderBy(f => f.Name)
                .ToList();

            var newList = new List<Folder>();

            foreach (FolderDTO folder in matchingFolders)
            {
                newList.Add((await GetFolderByIdAsync(folder.Id))!);
            }

            return newList;
        }

        public async Task<bool> UploadFolderAsync(FolderDTO folderDTO)
        {
            var folderCreateF = await AddFolderAsync(new CreatingFolderDTO
            {
                Name = folderDTO.Name,
                ParentId = folderDTO.ParentId,
                UserId = folderDTO.UserId
            });

            if (folderDTO.FileList!.Count > 0)
            {
                foreach (var file in folderDTO.FileList)
                {

                    var response = await _dbFileRepository.AddFileInDb(new DbFile
                    {
                        Name = file.Name,
                        FileType = file.FileType,
                        FileSize = file.FileSize,
                        FileData = file.FileData,
                        UserId = folderDTO.UserId,
                        FolderId = folderCreateF.Id
                    });
                }
            }

            if (folderDTO.FolderChildrenList!.Count > 0)
            {
                foreach (var child in folderDTO.FolderChildrenList)
                {
                    var childFolder = await UploadFolderAsync(new FolderDTO
                    {
                        Name = child.Name,
                        ParentId = folderCreateF.Id,
                        UserId = child.UserId,
                        FileList = child.FileList,
                        FolderChildrenList = child.FolderChildrenList
                    });
                }
            }

            return true;
        }

    }
}
