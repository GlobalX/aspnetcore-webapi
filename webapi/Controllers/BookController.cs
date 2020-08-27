using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using webapi.Models;
using webapi.Repositories;
using System.Threading.Tasks;
using System.Linq;

namespace webapi.Controllers
{
    [ApiController]
    [Route("books")]
    public class BookController : ControllerBase
    {
        private readonly IRepository<Book> _bookRepository;
        private readonly IRepository<Tenant> _tenantRepository;

        public BookController(IRepository<Book> bookRepository, IRepository<Tenant> tenantRepository)
        {
            _bookRepository = bookRepository;
            _tenantRepository = tenantRepository;
        }

        [HttpGet]
        [Route("")]
        public async Task<IEnumerable<Book>> GetAllBooks() => await _bookRepository.GetAllAsync();


        [HttpGet]
        [Route("{bookId}")]
        public async Task<Book> GetBookById(Guid bookId) => await _bookRepository.GetByIdAsync(bookId);

        [HttpPost]
        [Route("")]
        [AllowAnonymous]
        public async Task<Book> AddBook([FromBody] Book book) => await _bookRepository.InsertAsync(book);
        

        [HttpDelete]
        [Route("{bookId}")]
        [AllowAnonymous]
        public async Task DeleteBook(Guid bookId) => await _bookRepository.DeleteAsync(bookId);
    }
}