using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Xml.XPath;
using Entities.Models;
using Repositories.Contracts;
using Services.Contracts;
using Entities.Exceptions;
using Entities.DataTransferObjects;

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
            var books = _serviceManager.BookService.GetAllBooks(false);
            return Ok(books);
        }
        [HttpGet("{id:int}")]
        public IActionResult GetOneBooks([FromRoute(Name = "id")] int id)
        {
            var book = _serviceManager.BookService.GetOneBookById(id, false);

            return Ok(book); // 200
        }
        [HttpPost]
        public IActionResult CreateOneBook([FromBody] Book book)
        {
            if (book is null)
                return BadRequest(); // 400

            _serviceManager.BookService.CreateOneBook(book);

            return StatusCode(201, book); // 201
        }
        [HttpPut("{id:int}")]
        public IActionResult UpdateOneBook([FromRoute(Name = "id")] int id, [FromBody] BookDtoForUpdate book)
        {
            if (book is null)
                return BadRequest(); // 400

            _serviceManager.BookService.UpdateOneBook(id, book, true);

            return NoContent(); // 204
        }
        [HttpDelete("{id:int}")]
        public IActionResult DeleteOneBook([FromRoute(Name = "id")] int id)
        {
            
            _serviceManager.BookService.DeleteOneBook(id, false);
            return NoContent(); // 204
        }
        [HttpPatch("{id:int}")]
        public IActionResult PatchOneBook([FromRoute(Name = "id")] int id, [FromBody] JsonPatchDocument<Book> bookPatch)
        {
            var bookToUpdate = _serviceManager.BookService.GetOneBookById(id, true);

            bookPatch.ApplyTo(bookToUpdate);
            _serviceManager.BookService.UpdateOneBook(id, 
                new BookDtoForUpdate(bookToUpdate.Id, bookToUpdate.Title, bookToUpdate.Price),
                true);
            return NoContent(); // 204
        }
    }
}
