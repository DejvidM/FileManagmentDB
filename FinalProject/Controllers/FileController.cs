using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.IdentityModel.Tokens;
using ServiceL.DTO;
using ServiceL.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FinalProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IDbFileService _dbFileService;

        public FileController(IDbFileService dbFileService)
        {
            _dbFileService = dbFileService;
        }

        [HttpGet]
        public async Task<IActionResult> GetFiles()
        {
            var files = await _dbFileService.GetAllFilesAsync();
            return Ok(files);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFileById(int id)
        {
            var file = await _dbFileService.GetFileByIdAsync(id);
            if (file == null)
            {
                return NotFound();
            }

            return Ok(file);
        }

        [HttpPost]
        public async Task<IActionResult> AddFile()
        {
            var filePath = @"C:\Users\hp\OneDrive\Pictures\Screenshots 1\2025-01-09.png";

            if (!System.IO.File.Exists(filePath))
            {
                return BadRequest("File not found at the specified path.");
            }

            byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);

            string mimeType = _dbFileService.GetMimeType(filePath);            

            try
            {
                var response = await _dbFileService.AddFileAsync(new FileDTO
                {
                    Name = "File1",
                    FileSize = fileBytes.Length,
                    FileType = mimeType,
                    FileData = fileBytes,
                    UserId = 1,
                    FolderId = 15
                });

                return Ok(response);
            }
            catch (Exception ex) 
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("DeleteFile")]
        public async Task<IActionResult> DeleteFileById(int[] id)
        {
            if(id.Count() == 0) 
            {
                return BadRequest();
            }
            var response = await _dbFileService.RemoveFileAsync(id);

            return Ok(new 
            {
                response.Response,
                response.Errors
            });
            
        }

        [HttpGet("GetFolderFiles/{folderId}")]
        public async Task<IActionResult> GetFolderFiles(int folderId)
        {
            var files = await _dbFileService.GetFolderFilesAsync(folderId);
            return Ok(files);
        }

        [HttpGet("DownloadFile")]
        public async Task<IActionResult> DownloadFile(int fileId)
        {

            var file = await _dbFileService.GetFileByIdAsync(fileId);

            if (file == null)
            {
                return NotFound();
            }

            if (file.FileData == null || string.IsNullOrEmpty(file.Name) || string.IsNullOrEmpty(file.FileType))
            {
                return BadRequest(new
                {
                    Message = "Invalid file data."
                });
            }

            return File(file.FileData, file.FileType, file.Name);
        }

        [HttpPost("MoveFile")]
        public async Task<IActionResult> MoveFile(int fileId,int folderId)
        {
            try
            {
                var response = await _dbFileService.MoveFileAsync(fileId, folderId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("Search/{fileName}")]
        public async Task<IActionResult> SearchFileByName(string fileName)
        {
            var files = await _dbFileService.GetAllFilesAsync();

            return Ok(files.Where(f => f.Name.StartsWith(fileName, StringComparison.OrdinalIgnoreCase))
                   .OrderBy(f => f.Name)
                   .ToList());
        }
    }
}
