using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Xml.XPath;
using Entities.Models;
using Repositories.EFCore;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly RepositoryContext repositoryContext;

        public BooksController(RepositoryContext context)
        {
            repositoryContext = context;
        }
        [HttpGet]
        public IActionResult GetAllBooks()
        {
            try
            {
                var books = repositoryContext.Books.ToList();
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
                var book = repositoryContext.Books.Where(b => b.Id.Equals(id)).SingleOrDefault();

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

                repositoryContext.Books.Add(book);
                repositoryContext.SaveChanges();
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

                var bookToUpdate = repositoryContext.Books.Where(b => b.Id.Equals(id)).SingleOrDefault();

                if (bookToUpdate is null)
                    return NotFound(); // 404

                bookToUpdate.Title = book.Title;
                bookToUpdate.Price = book.Price;
                repositoryContext.SaveChanges();

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
                var bookToDelete = repositoryContext.Books.Where(b => b.Id.Equals(id)).SingleOrDefault();

                if (bookToDelete is null)
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = $"Book with id {id} not found"
                    }); // 404

                repositoryContext.Books.Remove(bookToDelete);
                repositoryContext.SaveChanges();
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
                var bookToUpdate = repositoryContext.Books.Where(b => b.Id.Equals(id)).SingleOrDefault();

                if (bookToUpdate is null)
                    return NotFound(); // 404

                bookPatch.ApplyTo(bookToUpdate);
                repositoryContext.SaveChanges();
                return NoContent(); // 204
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }
    }
}
