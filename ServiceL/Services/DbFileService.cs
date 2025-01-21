using DataAccessL.DTOdata;
using DataAccessL.Interfaces;
using DomainL.Entities;
using Microsoft.EntityFrameworkCore;
using ServiceL.DTO;
using ServiceL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceL.Services
{
    public class DbFileService : IDbFileService
    {
        private readonly IDbFileRepository _dbFileRepository;
        private readonly IFoldersRepository _foldersRepository;

        public DbFileService(IDbFileRepository dbFileRepository, IFoldersRepository foldersRepository)
        {
            _dbFileRepository = dbFileRepository;
            _foldersRepository = foldersRepository;
        }
        public Task<IEnumerable<DbFile>> GetAllFilesAsync()
        {
            return _dbFileRepository.GetAllAsync();
        }

        public async Task<DbFile?> GetFileByIdAsync(int id)
        {
            var file = await _dbFileRepository.GetByIdAsync(id);

            if (file == null)
            {
                return null;
            }

            return file;
        }

        public async Task<DbFile> AddFileAsync(FileDTO fileDTO)
        {
            var folder = await _foldersRepository.GetByIdAsync(fileDTO.FolderId);

            if (folder == null)
            {
                throw new Exception("Folder does not exist.");
            }
            
            if(folder.UserId != fileDTO.UserId)
            {
                throw new Exception("Files must belong to the user of the folder.");
            }

            var folderFiles = await _dbFileRepository.GetFolderFiles(fileDTO.FolderId);            

            if (folderFiles.Any(f => f.Name == fileDTO.Name))
            {
                throw new Exception("A file with the same name already exists in this directory.");
            }

            var file = await _dbFileRepository.AddFileInDb(new DbFile
            {
                Name = fileDTO.Name,
                FileSize = fileDTO.FileSize,
                FileType = fileDTO.FileType,
                FileData = fileDTO.FileData,
                UserId = fileDTO.UserId,
                FolderId = fileDTO.FolderId
            });

            return file;
        }

        public async Task<RemovingFileDTO> RemoveFileAsync(int[] id)
        {
            string success = "";
            string errors = "";

            for (int i = 0; i < id.Length; i++)
            {
                var file = await _dbFileRepository.GetByIdAsync(id[i]);
                if (file != null)
                {
                    await _dbFileRepository.RemoveAsync(file);
                    success += $"File (Id = {id[i]} deleted successfully.) {Environment.NewLine}";

                }
                else
                { 
                    errors += $"File (Id = {id[i]}) does not exist! {Environment.NewLine}";
                }
            }

            return new RemovingFileDTO
            {
                Response = success,
                Errors = errors
            };
        }

        public async Task<IEnumerable<DbFile>> GetFolderFilesAsync(int folderId)
        {
            var files = await _dbFileRepository.GetFolderFiles(folderId);
            return files;
        }

        public async Task<bool> MoveFileAsync(int fileId, int folderId)
        {
            var folder = await _foldersRepository.GetByIdAsync(folderId);

            var file = await _dbFileRepository.GetByIdAsync(fileId);

            if (folder == null || file == null)
            {
                throw new Exception("Folder or file does not exist!");
            }

            if (file.FolderId == folderId)
            {
                return false;
            }

            var folderFiles = await _dbFileRepository.GetFolderFiles(folderId);

            if(folderFiles.Any(f => f.Name == file!.Name))
            {
                throw new Exception("A file with the same name already exists in this directory.");
            }

            return await _dbFileRepository.MoveFileInFolder(fileId, folderId);
        }

        public async Task<IEnumerable<DbFile>> SearchFilesAsync(string name, int userId)
        {
            var files = await _dbFileRepository.GetAllAsync();

            return files.Where(f => f.Name.StartsWith(name, StringComparison.OrdinalIgnoreCase) && f.UserId == userId)
                   .OrderBy(f => f.Name)
                   .ToList();
        }

        public async Task<UpdateFileDTO?> UpdateFileAsync(UpdateFileDTO ufd)
        {
            var file = await _dbFileRepository.GetByIdAsync(ufd.Id);

            if (file == null)  
                return null;
            try
            {
                var response = await _dbFileRepository.UpdateFileInDb(file, ufd);
                return response;
            }
            catch (Exception ex) 
            {
                throw new Exception(ex.Message);
            }

        }
        public async Task<bool> RollbackToPreviousVersionAsync(int fileId)
        {
            try
            {
                var file = await _dbFileRepository.GetByIdAsync(fileId);

                if (file == null)
                    throw new Exception("File does not exist");

                var response = await _dbFileRepository.RollbackFileVersion(file);
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public string GetMimeType(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            return extension switch
            {
                ".txt" => "text/plain",
                ".pdf" => "application/pdf",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".json" => "application/json",
                ".xml" => "application/xml",
                ".zip" => "application/zip",
                _ => "application/octet-stream" // Default MIME type for unknown file types
            };
        }

    }
}
