﻿using Entities.Models;
using Entities.RequestFeatures;
using Microsoft.EntityFrameworkCore;
using Repositories.Contracts;
using Repositories.EFCore.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.EFCore
{
    internal sealed class BookRepository : RepositoryBase<Book>, IBookRepository
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

        public async Task<PagedList<Book>> GetAllBooksAsync(BookParameters bookParamaters, bool trackChanges)
        {
            var books =await FindAll(trackChanges).FilterBooks(bookParamaters.MinPrice, bookParamaters.MaxPrice)
                .Search(bookParamaters.SearchTerm).Sort(bookParamaters.OrderBy).ToListAsync();
            return PagedList<Book>.ToPagedList(books, bookParamaters.PageNumber, bookParamaters.PageSize);
        }

        public async Task<Book> GetOneBookByIdAsync(int id, bool trackChanges) 
            => await FindByConditons(b => b.Id.Equals(id), trackChanges).SingleOrDefaultAsync();

        public async Task<List<Book>> GetAllBooksAsync(bool trackChanges) => 
            await FindAll(trackChanges).OrderBy(b => b.Id).ToListAsync();
    }
}
