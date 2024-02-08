using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetAPI.Services;
using System.Globalization;

namespace PetAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FilesController : ControllerBase
{
    private readonly FileService _fileService;

    public FilesController(FileService fileService)
    {
        _fileService = fileService;
    }

    [HttpGet]
    public async Task<IActionResult> ListAllImages()
    {
        var result = await _fileService.ListAsync();
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        var result = await _fileService.UploadImageAsync(file);
        return Ok();
    }

    [HttpGet]
    [Route("filename")]
    public async Task<IActionResult> DownloadImage(string filename)
    {
        var result =  await _fileService.DownloadImageAsync(filename);
        return File(result.Content, result.ContentType, result.Name);
    }

    [HttpDelete]
    [Route("filename")]
    public async Task<IActionResult> DeleteImage(string filename)
    {
        var result = await _fileService.DeleteImageAsync(filename);
        return Ok(result);
    }
}

