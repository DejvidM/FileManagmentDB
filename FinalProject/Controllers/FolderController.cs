using FinalProject.DTO;
using Microsoft.AspNetCore.Mvc;
using ServiceL.DTO;
using ServiceL.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FinalProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FolderController : ControllerBase
    {
        private readonly IFolderService _folderService;

        public FolderController(IFolderService folderService)
        {
            _folderService = folderService;
        }

        // GET: api/<FolderController>
        [HttpGet]
        public async Task<IActionResult> GetFolders()
        {   
            var folders = await _folderService.GetAllFoldersAsync();

            return Ok(folders);
        }

        // GET api/<FolderController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFolder(int id)
        {
            var folder = await _folderService.GetFolderByIdAsync(id);

            if (folder == null)
            {
                return NotFound();
            }

            return Ok(folder);
        }

        // POST api/<FolderController>
        [HttpPost]
        public async Task<IActionResult> CreateFolder([FromBody] FolderDTO folderDTO)
        {
            try
            {
                var addedFolder = await _folderService.AddFolderAsync(folderDTO);

                return Ok(addedFolder);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        // DELETE api/<FolderController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _folderService.DeleteFoldersAsync(id);
                return Ok(response);
            }
            catch (Exception ex)
            {   
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Rename")]
        public async Task<IActionResult> RenameFolder([FromBody] RenameFolderDTO renameFolderDTO)
        {

            var folder = await _folderService.RenameFolderAsync(renameFolderDTO);

            if (folder == null)
            {
                return NotFound();
            }

            return Ok(folder);
        }

    }
}
