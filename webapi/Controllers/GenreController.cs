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
    [Route("genres")]
    public class GenreController : ControllerBase
    {
        private IGenreRepository _genreRepository;

        public GenreController(IGenreRepository genreRepository)
            { _genreRepository = genreRepository; }

        [HttpGet("")]
        public IEnumerable<Genre> GetAllGenres() => _genreRepository.GetAll();

        [HttpGet("{id}")]
        public Genre GetGenreById(int id) => _genreRepository.GetById(id);

        [HttpGet("name/{name}")]
        public Task<Genre> GetGenreByName(string name) => _genreRepository.GetByName(name);

        [HttpPost("")]
        [AllowAnonymous]
        public void AddGenre([FromBody] Genre genre) => _genreRepository.Insert(genre);
    }
}