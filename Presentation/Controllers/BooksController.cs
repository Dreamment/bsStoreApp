﻿using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Xml.XPath;
using Entities.Models;
using Repositories.Contracts;
using Services.Contracts;
using Entities.Exceptions;
using Entities.DataTransferObjects;
using Presentation.ActionFilters;
using Entities.RequestFeatures;
using System.Text.Json;
using Entities.LinkModels;

namespace Presentation.Controllers
{
    //[ApiVersion("1.0")]
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/books")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public BooksController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpHead]
        [HttpGet(Name = "GetAllBooksAsync")]
        [ServiceFilter(typeof(ValidatorMediaTypeAttribute))]
        [ResponseCache(Duration = 60)]
        public async Task<IActionResult> GetAllBooksAsync([FromQuery] BookParameters bookParamaters)
        {
            var linkParamaters = new LinkParameters()
            {
                BookParameters = bookParamaters,
                HttpContext = HttpContext
            };

            var result = await _serviceManager.BookService.GetAllBooksAsync(linkParamaters, false);
            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(result.metaData));
            return result.linkResponse.HasLinks ? Ok(result.linkResponse.LinkedEntities) : Ok(result.linkResponse.ShapedEntities);
        }
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetOneBooksAsync([FromRoute(Name = "id")] int id)
        {
            var book = await _serviceManager.BookService.GetOneBookByIdAsync(id, false);

            return Ok(book); // 200
        }
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [HttpPost(Name = "CreateOneBookAsync")]
        public async Task<IActionResult> CreateOneBookAsync([FromBody] BookDtoForInsertion bookDto)
        {
            var book = await _serviceManager.BookService.CreateOneBookAsync(bookDto);
            return StatusCode(201, book); // 201
        }
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateOneBookAsync([FromRoute(Name = "id")] int id, [FromBody] BookDtoForUpdate book)
        {
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
            if (bookPatch is null)
                return BadRequest(); // 400

            var result = await _serviceManager.BookService.GetOneBookForPatchAsync(id, false);

            TryValidateModel(result.bookDtoForUpdate);

            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState); // 422

            await _serviceManager.BookService.SaveChangesForPatchAsync(result.bookDtoForUpdate, result.book);


            return NoContent(); // 204
        }
        [HttpOptions]
        public IActionResult GetBooksOptions()
        {
            Response.Headers.Add("Allow", "GET, PUT, POST, PATCH, DELETE, HEAD, OPTIONS");
            return Ok();
        }
    }
}
