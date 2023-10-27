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
        public async Task<IActionResult> GetAllBooksAsync()
        {
            var books = await _serviceManager.BookService.GetAllBooksAsync(false);
            return Ok(books);
        }
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetOneBooksAsync([FromRoute(Name = "id")] int id)
        {
            var book = await _serviceManager.BookService.GetOneBookByIdAsync(id, false);

            return Ok(book); // 200
        }
        [HttpPost]
        public async Task<IActionResult> CreateOneBookAsync([FromBody] BookDtoForInsertion bookDto)
        {
            if (bookDto is null)
                return BadRequest(); // 400

            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState); // 422

            var book = await _serviceManager.BookService.CreateOneBookAsync(bookDto);

            return StatusCode(201, book); // 201
        }
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateOneBookAsync([FromRoute(Name = "id")] int id, [FromBody] BookDtoForUpdate book)
        {
            if (book is null)
                return BadRequest(); // 400

            if(!ModelState.IsValid)
                return UnprocessableEntity(ModelState); // 422

            await _serviceManager.BookService.UpdateOneBookAsync(id, book, false);

            return NoContent(); // 204
        }
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteOneBookAsync([FromRoute(Name = "id")] int id)
        {
            
            await _serviceManager.BookService.DeleteOneBookAsync(id, false);
            return NoContent(); // 204
        }
        [HttpPatch("{id:int}")]
        public async Task<IActionResult> PatchOneBookAsync([FromRoute(Name = "id")] int id, 
            [FromBody] JsonPatchDocument<BookDtoForUpdate> bookPatch)
        {
            if(bookPatch is null)
                return BadRequest(); // 400

            var result = await _serviceManager.BookService.GetOneBookForPatchAsync(id, false);

            TryValidateModel(result.bookDtoForUpdate);

            if(!ModelState.IsValid)
                return UnprocessableEntity(ModelState); // 422

            await _serviceManager.BookService.SaveChangesForPatchAsync(result.bookDtoForUpdate, result.book);
            

            return NoContent(); // 204
        }
    }
}
