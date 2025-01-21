using DataAccessL.DTOdata;
using DataAccessL.Interfaces;
using DomainL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessL.Repositories
{
    public class DbFileRepository : IDbFileRepository
    {
        private readonly FileManagerContext _context;

        public DbFileRepository(FileManagerContext context)
        {
            _context = context; 
        }

        public async Task<IEnumerable<DbFile>> GetAllAsync()
        {
            return await _context.Files.ToListAsync();
        }

        public async Task<DbFile?> GetByIdAsync(int id)
        {
            var file = await _context.Files.FindAsync(id);
            return file;
        }

        public async Task<DbFile> AddFileInDb(DbFile dbFile)
        {
            await _context.AddAsync(dbFile);
            await _context.SaveChangesAsync();
            return new DbFile
            {
                Id = dbFile.Id,
                Name = dbFile.Name,
                FileType = dbFile.FileType,
                FileSize = dbFile.FileSize,
                FileData = dbFile.FileData,
                UserId = dbFile.UserId, 
                FolderId = dbFile.FolderId
            };
        }

        public async Task RemoveAsync(DbFile dbFile)
        {        
            _context.Files.Remove(dbFile);
            await _context.SaveChangesAsync();
        }
        public async Task<List<DbFile>> GetFolderFiles(int folderId)
        {
            var files = await _context.Files.
                Where(file => file.FolderId == folderId).
                Select(file => new DbFile
                {
                    Id = file.Id,
                    Name = file.Name,
                    FileSize = file.FileSize,
                    FileType = file.FileType,
                    FileData = file.FileData,
                    UserId = file.UserId,
                    FolderId = file.FolderId
                }).
                ToListAsync();
                
                return files;
        }

        public async Task<bool> MoveFileInFolder(int fileId,  int folderId)
        {
            var file = await GetByIdAsync(fileId);

            if (file == null)
            {
                throw new Exception("File not found!");
            }

            file.FolderId = folderId;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<UpdateFileDTO> UpdateFileInDb(DbFile dbFile, UpdateFileDTO ufd)
        {

            if (dbFile.Name == ufd.Name && dbFile.FileSize == ufd.FileSize && dbFile.FileType == ufd.FileType && dbFile.FileData == ufd.FileData)
                throw new Exception("File is the same");

            try
            {
                var oldVersion = new FileVersion
                {
                    DbFileId = dbFile.Id,
                    Name = dbFile.Name,
                    FileType = dbFile.FileType,
                    FileSize = dbFile.FileSize,
                    FileData = dbFile.FileData
                };
                
                await _context.AddAsync(oldVersion);

                dbFile.Name = ufd.Name;
                dbFile.FileType = ufd.FileType;
                dbFile.FileSize = ufd.FileSize;
                dbFile.FileData = ufd.FileData;

                await _context.SaveChangesAsync();
                return ufd;
            }
            catch (Exception ex)
            {
                throw new Exception("Internal server error", ex);
            }
        }

        public async Task<bool> RollbackFileVersion(DbFile dbFile)
        {
            var fileVersions = await _context.FileVersion.Where(fv => fv.DbFileId == dbFile.Id)
                .OrderByDescending(fv => fv.Id)
                .ToListAsync();

            if( fileVersions.Count == 0)
            {
                throw new Exception("There are no previous versions");
            }
            
            var previousFileVersion = fileVersions[0];

            dbFile.Name = previousFileVersion.Name;
            dbFile.FileType = previousFileVersion.FileType;
            dbFile.FileSize = previousFileVersion.FileSize;
            dbFile.FileData = previousFileVersion.FileData;

            _context.FileVersion.Remove(previousFileVersion);

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
