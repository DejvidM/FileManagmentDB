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

        //[HttpGet]
        //public async Task<IActionResult> GetFolders()
        //{
        //        var folders = await _folderService.GetAllFoldersAsync();
        //        return Ok(folders);
        //}

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetFolder(int id)
        {
            var folder = await _folderService.GetFolderByIdAsync(id);
            if (folder == null)
            {
                return NotFound($"Folder with ID {id} was not found");
            }

            return Ok(folder);
        }

        [HttpPost]
        public async Task<IActionResult> CreateFolder([FromBody] CreatingFolderDTO creatingFolderDTO)
        {
            try
            {
                var addedFolder = await _folderService.AddFolderAsync(creatingFolderDTO);

                return Ok(addedFolder);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] int[] folderIds)
        {
            try
            {
                var response = await _folderService.DeleteFoldersAsync(folderIds);
                return Ok("Folders were deleted");
            }
            catch (Exception ex)
            {   
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Rename/{userId}")]
        public async Task<IActionResult> RenameFolder([FromBody] RenameFolderDTO renameFolderDTO, int userId)
        {
            try
            {
                var folder = await _folderService.RenameFolderAsync(renameFolderDTO, userId);

                if (folder == null)
                {
                    return NotFound("Folder was not found!");
                }

                return Ok(folder);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
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

        [HttpGet("Search/{folderName}/{userId}")]
        public async Task<IActionResult> SearchForFolder(string folderName, int userId)
        {
            var result = await _folderService.SearchForFolderAsync(folderName, userId);

            if (result.Count() == 0)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPost("UploadFolder")]
        public async Task<IActionResult> UploadFolder(FolderDTO folderDTO)
        {
            try
            {
                if (folderDTO == null)
                {
                    return BadRequest(new { Message = "You can not upload nothing." });
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
                return BadRequest(new { Errors = ex.Message });
            }
        }

    }
}
