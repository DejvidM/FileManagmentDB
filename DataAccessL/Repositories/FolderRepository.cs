using DataAccessL.Interfaces;
using DomainL.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessL.Repositories
{
    public class FolderRepository : IFoldersRepository
    {
        private readonly FileManagerContext _context;


        public FolderRepository(FileManagerContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Folder>> GetAllAsync()
        {
            return await _context.Folders.ToListAsync();
            
        }

        public async Task<Folder> GetByIdAsync(int id)
        {
            var folder = await _context.Folders.FindAsync(id);
            if (folder == null)
            {
                return null;
            }

            return folder;
           
        }

       public async Task<Folder> AddAsync(Folder folder)
        {
            await _context.Folders.AddAsync(folder);
            _context.SaveChanges();
            return folder;
        }                  


        public async Task<bool> DeleteAsync(Folder folder)
        {

            _context.Remove(folder);
            await _context.SaveChangesAsync();
            return true;
            
        }

        public async Task<Folder> RenameFolder(Folder folder, string newName)
        {
            folder.Name = newName;
            await _context.SaveChangesAsync();
            return new Folder
            {
                Id = folder.Id,
                Name = folder.Name,
                UserId = folder.UserId,
                ParentId = folder.ParentId
            };
        }

        public async Task<List<Folder>> GetAllUserFoldersFiles(int userId)
        {
            // Fetch all root folders for the user
            var folders = await _context.Folders
                .Where(folder => folder.UserId == userId && folder.ParentId == null)
                .Select(folder => new Folder
                {
                    Id = folder.Id,
                    Name = folder.Name,
                    UserId = folder.UserId,
                    ParentId = folder.ParentId
                })
                .ToListAsync();

            foreach (var folder in folders)
            {
                // Fetch nested folders for each root folder
                folder.Files = await _context.Files
                    .Where(file => file.FolderId == folder.Id)
                    .Select(file => new DbFile
                    {
                        Id = file.Id,
                        Name = file.Name,
                        FileSize = file.FileSize,
                        FileType = file.FileType,
                        FileData = file.FileData,
                        UserId = file.UserId,
                        FolderId = file.FolderId
                    })
                    .ToListAsync();

                folder.Folders = await FindNestedFolders(folder.Id, new HashSet<int>());
            }

            return folders;
        }

        public async Task<List<Folder>> FindNestedFolders(int folderId, HashSet<int> visitedFolderIds)
        {
            if (visitedFolderIds.Contains(folderId))
                return new List<Folder>();

            visitedFolderIds.Add(folderId);

            // Fetch child folders
            var nestedFolders = await _context.Folders
                .Where(f => f.ParentId == folderId)
                .Select(f => new Folder
                {
                    Id = f.Id,
                    Name = f.Name,
                    UserId = f.UserId,
                    ParentId = f.ParentId
                })
                .ToListAsync();

            foreach (var nestedFolder in nestedFolders)
            {
                // Fetch files for the nested folder
                nestedFolder.Files = await _context.Files
                    .Where(file => file.FolderId == nestedFolder.Id)
                    .Select(file => new DbFile
                    {
                        Id = file.Id,
                        Name = file.Name,
                        FileSize = file.FileSize,
                        FileType = file.FileType,
                        FileData = file.FileData,
                        UserId = file.UserId,
                        FolderId = file.FolderId
                    })
                    .ToListAsync();

                // Recursively fetch child folders
                nestedFolder.Folders = await FindNestedFolders(nestedFolder.Id, visitedFolderIds);
            }

            return nestedFolders;
        }

        public async Task<Folder> MoveFolder(Folder originFolder, Folder destinationFolder)
        {
            originFolder.ParentId = destinationFolder.Id;
            await _context.SaveChangesAsync();
            return new Folder
            {
                Id = originFolder.Id,
                Name = originFolder.Name,
                UserId = originFolder.UserId,
                ParentId = originFolder.ParentId
            };
        }

    }
}

