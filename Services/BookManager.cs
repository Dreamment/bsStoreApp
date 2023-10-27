using AutoMapper;
using Entities.DataTransferObjects;
using Entities.Exceptions;
using Entities.Models;
using Repositories.Contracts;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class BookManager : IBookService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly ILoggerService _logger;
        private readonly IMapper _mapper;

        public BookManager(IRepositoryManager repositoryManager, ILoggerService logger, IMapper mapper)
        {
            _repositoryManager = repositoryManager;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<BookDto> CreateOneBookAsync(BookDtoForInsertion bookDto)
        {
            var entityBook = _mapper.Map<Book>(bookDto);
            _repositoryManager.Book.CreateOneBook(entityBook);
            _repositoryManager.SaveAsync();
            return _mapper.Map<BookDto>(entityBook);
        }

        public async Task DeleteOneBookAsync(int id, bool trackChanges)
        {
            var book = await _repositoryManager.Book.GetOneBookByIdAsync(id, trackChanges);

            if (book == null)
                throw new BookNotFoundException(id);


            _repositoryManager.Book.DeleteOneBook(book);
            await _repositoryManager.SaveAsync();
        }

        public async Task UpdateOneBookAsync(int id, BookDtoForUpdate bookDto, bool trackChanges)
        {
            var bookEntity = await _repositoryManager.Book.GetOneBookByIdAsync(id, trackChanges);

            if (bookEntity is null)
                throw new BookNotFoundException(id);

            //Mapping
            bookEntity = _mapper.Map<Book>(bookDto);

            _repositoryManager.Book.Update(bookEntity);
            await _repositoryManager.SaveAsync();
        }

        public async Task<IEnumerable<BookDto>> GetAllBooksAsync(bool trackChanges)
        {
            var books = await _repositoryManager.Book.GetAllBooksAsync(trackChanges);
            return _mapper.Map<IEnumerable<BookDto>>(books);
        }

        public async Task<BookDto> GetOneBookByIdAsync(int id, bool trackChanges)
        {
            var book = await _repositoryManager.Book.GetOneBookByIdAsync(id, trackChanges);
            if (book is null)
                throw new BookNotFoundException(id);
            return _mapper.Map<BookDto>(book);
        }

        public async Task<(BookDtoForUpdate bookDtoForUpdate, Book book)> GetOneBookForPatchAsync(int id, bool trackChanges)
        {
            var book = await _repositoryManager.Book.GetOneBookByIdAsync(id, trackChanges);
            if (book is null)
                throw new BookNotFoundException(id);
            var bookDtoForUpdate = _mapper.Map<BookDtoForUpdate>(book);
            return (bookDtoForUpdate, book);
        }

        public async Task SaveChangesForPatchAsync(BookDtoForUpdate bookDtoForUpdate, Book book)
        {
            _mapper.Map(bookDtoForUpdate, book);
            _repositoryManager.SaveAsync();
        }
    }
}
