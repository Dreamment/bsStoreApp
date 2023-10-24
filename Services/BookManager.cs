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

        public BookManager(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }

        public Book CreateOneBook(Book book)
        {
            if (book == null)
                throw new ArgumentNullException(nameof(book));
            _repositoryManager.Book.CreateOneBook(book);
            _repositoryManager.Save();
            return book;
        }

        public void DeleteOneBook(int id, bool trackChanges)
        {
            var book = _repositoryManager.Book.GetOneBookById(id, trackChanges);

            if (book != null)
                throw new Exception($"Book with id:{id} could not found.");

            _repositoryManager.Book.DeleteOneBook(book);
            _repositoryManager.Save();
        }

        public void UpdateOneBook(int id, Book book, bool trackChanges)
        {
            var bookEntity = _repositoryManager.Book.GetOneBookById(id, trackChanges);

            if (bookEntity is null)
                throw new Exception($"Book with id:{id} could not found.");

            bookEntity.Title = book.Title;
            bookEntity.Price = book.Price;

            _repositoryManager.Book.Update(bookEntity);
            _repositoryManager.Save();
        }

        public IEnumerable<Book> GetAllBooks(bool trackChanges)
        {
            return _repositoryManager.Book.GetAllBooks(trackChanges);
        }

        public Book GetOneBookById(int id, bool trackChanges)
        {
            return _repositoryManager.Book.GetOneBookById(id, trackChanges);
        }
    }
}
