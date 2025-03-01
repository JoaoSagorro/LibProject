namespace WebAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using ADOLib;
using ADOLib.DTOs;
using ADOLib.ModelView;
using LibLibrary.Services;

[ApiController]
[Route("api/[controller]")]
public class LibrariesController : ControllerBase
{
    private readonly Libraries _libraryService;

    public LibrariesController(Libraries libraries)
    {
        _libraryService = libraries;
    }

    [HttpGet("Books/{bookId}")]
    public IActionResult GetLibrariesByNumberOfCopies(int bookId)
    {
        try
        {
            var libraries = _libraryService.GetLibrariesByNumberOfCopies(bookId);
            return Ok(libraries);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}
