using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Xml.XPath;
using Entities.Models;
using Repositories.Contracts;
using Services.Contracts;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public BooksController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpGet]
        public IActionResult GetAllBooks()
        {
            try
            {
                var books = _serviceManager.BookService.GetAllBooks(false);
                return Ok(books);

            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }
        [HttpGet("{id:int}")]
        public IActionResult GetOneBooks([FromRoute(Name = "id")] int id)
        {
            try
            {
                var book = _serviceManager.BookService.GetOneBookById(id, false);

                if (book is null)
                    return NotFound(); // 404

                return Ok(book); // 200
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }

        }
        [HttpPost]
        public IActionResult CreateOneBook([FromBody] Book book)
        {
            try
            {
                if (book is null)
                    return BadRequest(); // 400

                _serviceManager.BookService.CreateOneBook(book);

                return StatusCode(201, book); // 201
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }
        [HttpPut("{id:int}")]
        public IActionResult UpdateOneBook([FromRoute(Name = "id")] int id, [FromBody] Book book)
        {
            try
            {
                if (book is null)
                    return BadRequest(); // 400

                _serviceManager.BookService.UpdateOneBook(id, book, true);

                return NoContent(); // 204
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }
        [HttpDelete("{id:int}")]
        public IActionResult DeleteOneBook([FromRoute(Name = "id")] int id)
        {
            try
            {
                _serviceManager.BookService.DeleteOneBook(id, false);
                return NoContent(); // 204
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }
        [HttpPatch("{id:int}")]
        public IActionResult PatchOneBook([FromRoute(Name = "id")] int id, [FromBody] JsonPatchDocument<Book> bookPatch)
        {
            try
            {
                var bookToUpdate = _serviceManager.BookService.GetOneBookById(id, true);

                if (bookToUpdate is null)
                    return NotFound(); // 404

                bookPatch.ApplyTo(bookToUpdate);
                _serviceManager.BookService.UpdateOneBook(id, bookToUpdate, true);
                return NoContent(); // 204
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }
    }
}
