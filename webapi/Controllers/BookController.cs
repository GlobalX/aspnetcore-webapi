using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using webapi.Models;
using webapi.Repositories;

namespace webapi.Controllers
{
    [ApiController]
    [Route("books")]
    public class BookController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;
        public BookController(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        [HttpGet]
        [Route("")]
        public IEnumerable<Book> GetAllBooks()
        {
            var results = _bookRepository.GetAll();

            return new List<Book>() { results.First() };
        }

        [HttpGet]
        [Route("{bookId}")]
        public Book GetBookById(Guid bookId) => _bookRepository.GetById(bookId);

        [HttpPost]
        [Route("")]
        [AllowAnonymous]
        public object AddBook([FromBody] Book book)
        {
            book.Id = Guid.NewGuid();
            var newBookId = _bookRepository.Insert(book);

            return new {Id = newBookId };
        }

        [HttpDelete]
        [Route("{bookId}")]
        [AllowAnonymous]
        public void DeleteBook(Guid bookId) => _bookRepository.Delete(bookId);
    }
}