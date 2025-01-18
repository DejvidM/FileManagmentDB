using DomainL.Entities;
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

        [HttpGet]
        public async Task<IActionResult> GetFolders()
        {   
            var folders = await _folderService.GetAllFoldersAsync();

            return Ok(folders);
        }

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

        [HttpPost("MoveFolder")]
        public async Task<IActionResult> MoveFolder(int originFolderId, int destinationFolderId)
        {
            try
            {
                var response = await _folderService.MoveFolderAsync(originFolderId, destinationFolderId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("Search/{folderName}")]
        public async Task<IActionResult> SearchForFolder(string folderName)
        {
            var folders = await _folderService.GetAllFoldersAsync();
            var matchingFolders = folders
                .Where(f => f.Name.StartsWith(folderName, StringComparison.OrdinalIgnoreCase))
                .OrderBy(f => f.Name)
                .ToList();

            if ( matchingFolders.Count() == 0)
            {
                return NotFound();
            }

            var newList = new List<Folder>();
            
            foreach( FolderDTO folder in matchingFolders)
            {
                newList.Add(await _folderService.GetFolderByIdAsync(folder.Id));
            }

            return Ok(newList);
        }

        [HttpPost("UploadFolder")]
        public async Task<IActionResult> UploadFolder(FolderDTO folderDTO)
        {
            try
            {
                if (folderDTO == null)
                {
                    return BadRequest(new { Message = "Folder cannot be empty" });
                }

                if (string.IsNullOrEmpty(folderDTO.Name))
                {
                    return BadRequest(new { Message = "Folder name is required" });
                }

                var response = await _folderService.UploadFolderAsync(folderDTO);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Errors = ex.Message});
            }
        }

    }
}
