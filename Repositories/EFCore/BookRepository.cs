using Entities.Models;
using Entities.RequestFeatures;
using Microsoft.EntityFrameworkCore;
using Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.EFCore
{
    internal class BookRepository : RepositoryBase<Book>, IBookRepository
    {
        public BookRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }

        public void CreateOneBook(Book book) 
            => Create(book);
        public void DeleteOneBook(Book book) 
            => Delete(book);
        public void UpdateOneBook(Book book) 
            => Update(book);

        public async Task<IEnumerable<Book>> GetAllBooksAsync(BookParamaters bookParamaters, bool trackChanges) 
            => await FindAll(trackChanges).OrderBy(b => b.Id).Skip((bookParamaters.PageNumber-1)*bookParamaters.PageSize)
            .Take(bookParamaters.PageSize).ToListAsync();

        public async Task<Book> GetOneBookByIdAsync(int id, bool trackChanges) 
            => await FindByConditons(b => b.Id.Equals(id), trackChanges).SingleOrDefaultAsync();

    }
}
