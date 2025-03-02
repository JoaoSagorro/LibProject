using Microsoft.AspNetCore.Mvc;
using ADOLib;
using ADOLib.DTOs;
using ADOLib.ModelView;


namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly Books _booksService;
        private readonly RequestBookService _requestBookService;
        private readonly ReturnBookService _returnBookService;

        public BooksController()
        {
            _booksService = new Books();
            _requestBookService = new RequestBookService();
            _returnBookService = new ReturnBookService();
        }

        // GET: api/books
        [HttpGet]
        public IActionResult GetAllBooks()
        {
            try
            {
                var books = _booksService.GetAllBooks();
                return Ok(books);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // GET: api/books/{id}
        [HttpGet("{bookId}")]
        public IActionResult GetBookById(int bookId)
        {
            try
            {
                var book = _booksService.GetBookById(bookId);
                if (book == null)
                    return NotFound(new { message = "Book not found." });

                return Ok(book);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("requestBook")]
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


        // POST: api/books/return/{orderId}
        [HttpPost("return/{orderId}")]
        public IActionResult ReturnBookByOrderId(int orderId)
        {
            try
            {
                _returnBookService.ReturnBookByOrderId(orderId);
                return Ok(new { message = "Book returned successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }


        // GET: api/books-search
        [HttpGet("getAll")]
        public IActionResult GetBooksForSearch()
        {
            try
            {
                var books = _booksService.GetBooksForSearch();
                return Ok(books);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
