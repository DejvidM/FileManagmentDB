using DataAccessL.DTOdata;
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
        public async Task<IActionResult> AddFile([FromBody] FileDTO fileDTO)
        {
            try
            {
                if (fileDTO.FileData == null || fileDTO.FileData.Length == 0)
                {
                    fileDTO.FileData = Array.Empty<byte>();
                }

                var response = await _dbFileService.AddFileAsync(fileDTO);

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
            if(id.Length == 0) 
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

        //[HttpGet("GetFolderFiles/{folderId}")]
        //public async Task<IActionResult> GetFolderFiles(int folderId)
        //{
        //    var files = await _dbFileService.GetFolderFilesAsync(folderId);
        //    return Ok(files);
        //}

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
                return Ok(new { message = "File moved successfully"});
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("Search/{fileName}/{userId}")]
        public async Task<IActionResult> SearchFileByName(string fileName, int userId)
        {
            var files = await _dbFileService.SearchFilesAsync(fileName, userId);

            return Ok(files);
        }

        [HttpPost("UpdateFile")]
        public async Task<IActionResult> UpdateFile(UpdateFileDTO ufd)
        {
            if (ufd == null)
                return BadRequest();

            if (String.IsNullOrEmpty(ufd.Name))
                return BadRequest("Name is required");

            var response = await _dbFileService.UpdateFileAsync(ufd);

            if(response == null)
                return NotFound("File does not exist!");

            return Ok(response);
        }

        [HttpGet("Rollback")]
        public async Task<IActionResult> RollbackToPreviousVersion(int fileId)
        {
            try
            {
                var response = await _dbFileService.RollbackToPreviousVersionAsync(fileId);
                return Ok("File reversed succesfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}
