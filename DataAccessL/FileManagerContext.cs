using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DomainL.Entities;

namespace DataAccessL
{
    public class FileManagerContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<DbFile> Files { get; set; }
        public DbSet<Folder> Folders { get; set; }

        public FileManagerContext(DbContextOptions<FileManagerContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure File -> User relationship
            modelBuilder.Entity<DbFile>()
                .HasOne(f => f.User)
                .WithMany(u => u.Files)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Change from Cascade to Restrict or NoAction

            // Configure Folder -> User relationship
            modelBuilder.Entity<Folder>()
                .HasOne(f => f.User)
                .WithMany(u => u.Folders)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Keep Cascade if required

            // Configure File -> Folder relationship
            modelBuilder.Entity<DbFile>()
                .HasOne(f => f.Folder)
                .WithMany(fol => fol.Files)
                .HasForeignKey(f => f.FolderId)
                .OnDelete(DeleteBehavior.Cascade); // Keep Cascade or adjust as needed

            // Configure Folder -> Parent Folder relationship (Self-referencing relationship)
            modelBuilder.Entity<Folder>()
                .HasOne(f => f.ParentFolder) // Parent folder
                .WithMany(f => f.Folders) // Subfolders collection
                .HasForeignKey(f => f.ParentId) // Foreign key to the parent folder
                .OnDelete(DeleteBehavior.Restrict); // Prevent deletion of parent folder if there are subfolders
        }


    }
}
