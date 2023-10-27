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

        public BookDto CreateOneBook(BookDtoForInsertion bookDto)
        {
            var entityBook = _mapper.Map<Book>(bookDto);
            _repositoryManager.Book.CreateOneBook(entityBook);
            _repositoryManager.Save();
            return _mapper.Map<BookDto>(entityBook);
        }

        public void DeleteOneBook(int id, bool trackChanges)
        {
            var book = _repositoryManager.Book.GetOneBookById(id, trackChanges);

            if (book == null)
                throw new BookNotFoundException(id);


            _repositoryManager.Book.DeleteOneBook(book);
            _repositoryManager.Save();
        }

        public void UpdateOneBook(int id, BookDtoForUpdate bookDto, bool trackChanges)
        {
            var bookEntity = _repositoryManager.Book.GetOneBookById(id, trackChanges);

            if (bookEntity is null)
                throw new BookNotFoundException(id);

            //Mapping
            bookEntity = _mapper.Map<Book>(bookDto);

            _repositoryManager.Book.Update(bookEntity);
            _repositoryManager.Save();
        }

        public IEnumerable<BookDto> GetAllBooks(bool trackChanges)
        {
            var books = _repositoryManager.Book.GetAllBooks(trackChanges);
            return _mapper.Map<IEnumerable<BookDto>>(books);
        }

        public BookDto GetOneBookById(int id, bool trackChanges)
        {
            var book = _repositoryManager.Book.GetOneBookById(id, trackChanges);
            if (book is null)
                throw new BookNotFoundException(id);
            return _mapper.Map<BookDto>(book);
        }

        public (BookDtoForUpdate bookDtoForUpdate, Book book) GetOneBookForPatch(int id, bool trackChanges)
        {
            var book = _repositoryManager.Book.GetOneBookById(id, trackChanges);
            if (book is null)
                throw new BookNotFoundException(id);
            var bookDtoForUpdate = _mapper.Map<BookDtoForUpdate>(book);
            return (bookDtoForUpdate, book);
        }

        public void SaveChangesForPatch(BookDtoForUpdate bookDtoForUpdate, Book book)
        {
            _mapper.Map(bookDtoForUpdate, book);
            _repositoryManager.Save();
        }
    }
}
