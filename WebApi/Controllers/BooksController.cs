using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Xml.XPath;
using Entities.Models;
using Repositories.Contracts;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IRepositoryManager _repositoryManager;

        public BooksController(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }

        [HttpGet]
        public IActionResult GetAllBooks()
        {
            try
            {
                var books = _repositoryManager.Book.GetAllBooks(false);
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
                var book = _repositoryManager.Book.GetOneBookById(id, false);

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

                _repositoryManager.Book.CreateOneBook(book);
                _repositoryManager.Save();

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

                var bookToUpdate = _repositoryManager.Book.GetOneBookById(id, true);

                if (bookToUpdate is null)
                    return NotFound(); // 404

                _repositoryManager.Book.UpdateOneBook(book);
                _repositoryManager.Save();

                return Ok(bookToUpdate); // 200
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
                var bookToDelete = _repositoryManager.Book.GetOneBookById(id, false);

                if (bookToDelete is null)
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = $"Book with id {id} not found"
                    }); // 404

                _repositoryManager.Book.DeleteOneBook(bookToDelete);
                _repositoryManager.Save();

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
                var bookToUpdate = _repositoryManager.Book.GetOneBookById(id, true);

                if (bookToUpdate is null)
                    return NotFound(); // 404

                bookPatch.ApplyTo(bookToUpdate);
                _repositoryManager.Book.UpdateOneBook(bookToUpdate);
                _repositoryManager.Save();
                return NoContent(); // 204
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }
    }
}
