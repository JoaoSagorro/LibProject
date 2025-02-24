using Microsoft.AspNetCore.Mvc;
using ADOLib;
using ADOLib.DTOs;


namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestBookController : ControllerBase
    {
        private readonly RequestBookService _requestBookService;

        public RequestBookController()
        {
            _requestBookService = new RequestBookService();
        }

        [HttpPost]
        public async Task<IActionResult> RequestBook([FromBody] RequestBookDto request)
        {
            if (request == null)
                return BadRequest("Invalid request data.");

            try
            {
                bool success = await _requestBookService.RequestBook(
                    request.UserId, request.BookId, request.LibraryId, request.NumberOfCopies
                );

                if (success)
                    return Ok(new { message = "Book requested successfully." });

                return BadRequest("Book request failed.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
