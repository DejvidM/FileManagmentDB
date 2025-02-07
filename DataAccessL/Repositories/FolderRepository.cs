﻿using DataAccessL.Interfaces;
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
        private readonly IDbFileRepository _dbFileRepository;

        public FolderRepository(FileManagerContext context, IDbFileRepository dbFileRepository)
        {
            _context = context;
            _dbFileRepository = dbFileRepository;   
        }
        public async Task<IEnumerable<Folder>> GetAllAsync()
        {
            return await _context.Folders.ToListAsync();  
        }

        public async Task<Folder?> GetByIdAsync(int id)
        {
            var folder = await _context.Folders.FindAsync(id);

            if (folder == null)
                return null;

            return folder;  
        }
        public async Task<List<Folder>> FindNestedFolders(int folderId, HashSet<int> visitedFolderIds)
        {
            if (visitedFolderIds.Contains(folderId))
                return new List<Folder>();

            visitedFolderIds.Add(folderId);

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
                nestedFolder.Files = await _dbFileRepository.GetFolderFiles(nestedFolder.Id);

                nestedFolder.Folders = await FindNestedFolders(nestedFolder.Id, visitedFolderIds);
            }
            return nestedFolders;
        }

        public async Task<Folder> AddAsyncInDb(Folder folder)
        {
            await _context.Folders.AddAsync(folder);
            _context.SaveChanges();
            return folder;
        }

        public async Task<bool> ExistsWithNameInDirectory(string name, int? parentId, int userId)
        {
            return await _context.Folders
                .Where(f => f.ParentId ==  parentId)
                .AnyAsync(f => f.Name == name && f.UserId == userId);
        }

        public async Task<bool> ExistsWithNameInDirectory(string name, int? parentId, int userId, int folderId)
        {
            return await _context.Folders
                .Where(f => f.ParentId == parentId)
                .AnyAsync(f => f.Name == name && f.Id != folderId && f.UserId == userId);
        }  

        public async Task DeleteAsync(int folderId)
        {
            var folder = await _context.Folders.FindAsync(folderId);
            _context.Remove(folder);
            await _context.SaveChangesAsync();          
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
        public async Task<IEnumerable<Folder>> GetChildrenFolders(Folder parentFolder)
        {
            return await _context.Folders.Where(f => f.ParentId == parentFolder.Id).ToListAsync();
        }

        public async Task<List<Folder>> GetAllUserFoldersFiles(int userId)
        {
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
                folder.Files = await _dbFileRepository.GetFolderFiles(folder.Id);

                folder.Folders = await FindNestedFolders(folder.Id, new HashSet<int>());
            }

            return folders;
        }


    }
}

