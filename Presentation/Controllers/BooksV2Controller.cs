﻿using Microsoft.AspNetCore.Mvc;
using Services.Contracts;

namespace Presentation.Controllers
{
    [ApiVersion("2.0")]
    [ApiController]
    [Route("api/{v:apiversion}/books")]
    public class BooksV2Controller : ControllerBase
    {
        private readonly IServiceManager _manager;

        public BooksV2Controller(IServiceManager manager)
        {
            _manager = manager;
        }

        [HttpGet(Name = "GetAllBooksAsyncV2")]
        public async Task<IActionResult> GetAllBooksAsync()
        {
            var books = await _manager.BookService.GetAllBooksAsync(false);
            var booksV2 = books.Select(b => new
            {
                Id = b.Id,
                Title = b.Title
            });
            return Ok(booksV2);
        }
    }
}
