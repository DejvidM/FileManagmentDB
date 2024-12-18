using DataAccessL.Interfaces;
using DomainL.Entities;
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

        public DbFileService(IDbFileRepository dbFileRepository)
        {
            _dbFileRepository = dbFileRepository;
        }

        public async Task<DbFile> AddFileAsync(FileDTO fileDTO)
        {
            var file = await _dbFileRepository.AddAsync(new DbFile
            {
                Id = fileDTO.Id,
                Name = fileDTO.Name,
                FileSize = fileDTO.FileSize,
                FileType = fileDTO.FileType,
                FileData = fileDTO.FileData,
                UserId = fileDTO.UserId,
                FolderId = fileDTO.FolderId,
            });

            return file;
        }

        public async Task<bool> RemoveFileAsync(int id)
        {
            var file = await _dbFileRepository.GetByIdAsync(id);
            if (file != null)
            {
                var response = await _dbFileRepository.RemoveAsync(file);
                return response;
            }

            return false;
        }

        public Task<IEnumerable<DbFile>> GetAllFilesAsync()
        {
            return _dbFileRepository.GetAllAsync();
        }

        public async Task<DbFile> GetFileByIdAsync(int id)
        {
            var file = await _dbFileRepository.GetByIdAsync(id);

            if(file == null)
            {
                return null;
            }

            return file;
        }

        public async Task<IEnumerable<DbFile>> GetFolderFiles(int folderId)
        {
            var files = await _dbFileRepository.GetFolderFiles(folderId);
            return files;
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
