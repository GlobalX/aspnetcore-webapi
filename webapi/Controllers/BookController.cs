using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public IEnumerable<Book> GetAllBooks() => _bookRepository.GetAll();

        [HttpGet]
        [Route("{bookId}")]
        public Book GetBookById(Guid bookId) => _bookRepository.GetById(bookId);

        [HttpPost]
        [Route("")]
        [AllowAnonymous]
        public void AddBook([FromBody] Book book)
        {
            _bookRepository.Insert(book);
        }

        [HttpDelete]
        [Route("{bookId}")]
        [AllowAnonymous]
        public void DeleteBook(Guid bookId) => _bookRepository.Delete(bookId);
    }
}