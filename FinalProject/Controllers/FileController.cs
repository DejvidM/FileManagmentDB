using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Serialization;
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


        // GET: api/<FileController>
        [HttpGet]
        public async Task<IActionResult> GetFiles()
        {
            var files = await _dbFileService.GetAllFilesAsync();
            return Ok(files);
        }

        // GET api/<FileController>/5
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


        // POST api/<FileController>
        [HttpPost]
        public async Task<IActionResult> AddFile()
        {
            var filePath = @"C:\Users\hp\Downloads\practice_file.txt";

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
                    UserId = 1,
                    FileSize = fileBytes.Length,
                    FileType = mimeType,
                    FileData = fileBytes,
                    FolderId = 15
                }
                );
                return Ok(response);
            }
            catch (Exception ex) 
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE api/<FileController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFileById(int id)
        {
            var response = await _dbFileService.RemoveFileAsync(id);
            if (response)
            {
                return Ok("File deleted succesfully");
            }

            return NotFound();
        }

        [HttpGet("GetFolderFiles/{id}")]
        public async Task<IActionResult> GetFolderFiles(int id)
        {
            var files = await _dbFileService.GetFolderFiles(id);
            return Ok(files);
        }
    }
}
