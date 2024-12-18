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
        public async Task<DbFile> AddAsync(DbFile dbFile)
        {
            await _context.AddAsync(dbFile);
            await _context.SaveChangesAsync();
            return dbFile;
        }

        public async Task<IEnumerable<DbFile>> GetAllAsync()
        {
            return await _context.Files.ToListAsync();
        }

        public async Task<DbFile> GetByIdAsync(int id)
        {
            var file = await _context.Files.FindAsync(id);
            return file;
        }

        public async Task<bool> RemoveAsync(DbFile dbFile)
        {        
            _context.Files.Remove(dbFile);
            await _context.SaveChangesAsync();
            return true;
            

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
    }
}
