using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using webapi.Models;
using webapi.Repositories;

namespace webapi.Controllers
{
    [ApiController]
    [Route("author")]
    public class AuthorController : ControllerBase
    {
        private IAuthorRepository _authorRepository;

        public AuthorController(IAuthorRepository authorRepository)
            { _authorRepository = authorRepository; }

        [HttpGet("")]
        public async Task<IEnumerable<Author>> GetAllAuthors() => await _authorRepository.GetAllAsync();

        [HttpGet("{authorName}")]
        public async Task<Author> GetAuthorByName(String authorName) => await _authorRepository.GetByName(authorName);

        [HttpPost("")]
        [AllowAnonymous]
        public async Task AddAuthor([FromBody] Author author) => await _authorRepository.InsertAsync(author);
    }
}