using AutoMapper;
using Entities.DataTransferObjects;
using Entities.Exceptions;
using Entities.Models;
using Entities.RequestFeatures;
using Repositories.Contracts;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Dynamic;
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
        private readonly IDataShaper<BookDto> _shaper;

        public BookManager(IRepositoryManager repositoryManager, ILoggerService logger, IMapper mapper, IDataShaper<BookDto> shaper)
        {
            _repositoryManager = repositoryManager;
            _logger = logger;
            _mapper = mapper;
            _shaper = shaper;
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
            var book = await GetOneBookByIdAndCheckExistsAsync(id, trackChanges);
            _repositoryManager.Book.DeleteOneBook(book);
            await _repositoryManager.SaveAsync();
        }

        public async Task UpdateOneBookAsync(int id, BookDtoForUpdate bookDto, bool trackChanges)
        {
            var bookEntity = await GetOneBookByIdAndCheckExistsAsync(id, trackChanges);
            //Mapping
            bookEntity = _mapper.Map<Book>(bookDto);
            _repositoryManager.Book.Update(bookEntity);
            await _repositoryManager.SaveAsync();
        }

        public async Task<(IEnumerable<ExpandoObject> books, MetaData metaData)> GetAllBooksAsync(BookParamaters bookParamaters, bool trackChanges)
        {
            if (!bookParamaters.ValidPriceRange)
                throw new PriceOutofRangeBadRequestException();

            var booksWithMetaData = await _repositoryManager.Book.GetAllBooksAsync(bookParamaters, trackChanges);
            var booksDto = _mapper.Map<IEnumerable<BookDto>>(booksWithMetaData);

            var shapedData = _shaper.ShapeData(booksDto, bookParamaters.Field);
            return (books: shapedData, metaData : booksWithMetaData.MetaData);
        }

        public async Task<BookDto> GetOneBookByIdAsync(int id, bool trackChanges)
        {
            var book = await GetOneBookByIdAndCheckExistsAsync(id, trackChanges);
            return _mapper.Map<BookDto>(book);
        }

        public async Task<(BookDtoForUpdate bookDtoForUpdate, Book book)> GetOneBookForPatchAsync(int id, bool trackChanges)
        {
            var book = await GetOneBookByIdAndCheckExistsAsync(id, trackChanges);
            var bookDtoForUpdate = _mapper.Map<BookDtoForUpdate>(book);
            return (bookDtoForUpdate, book);
        }

        public async Task SaveChangesForPatchAsync(BookDtoForUpdate bookDtoForUpdate, Book book)
        {
            _mapper.Map(bookDtoForUpdate, book);
            _repositoryManager.SaveAsync();
        }

        private async Task<Book> GetOneBookByIdAndCheckExistsAsync(int id, bool trackChanges)
        {
            var book = await _repositoryManager.Book.GetOneBookByIdAsync(id, trackChanges);
            if (book is null)
                throw new BookNotFoundException(id);
            return book;
        }
    }
}
