using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

using coreWebApiDemo.Models.DAL;
using coreWebApiDemo.Models.DTO;
using coreWebApiDemo.Models.DAL.Entities;
using coreWebApiDemo.Services;
using coreWebApiDemo.Helpers;

namespace coreWebApiDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IClassService classService;
        private readonly IMapper mapper;
        public AuthorsController(ApplicationDbContext context, IClassService classService, IMapper mapper)
        {
            this.context = context;
            this.classService = classService;
            this.mapper = mapper;
        }

        [HttpGet("list")]
        [HttpGet]
        [ServiceFilter(typeof(MyActionFilter))]
        public async Task<ActionResult<IEnumerable<AuthorDTO>>> Get()
        {
            classService.DoSomething("ayyyylmao");
            var authors = await context.Authors
                .Include(a => a.Books)
                .ToListAsync();
            var authorsDto = mapper.Map<List<AuthorDTO>>(authors);

            return authorsDto;
        }

        [HttpGet("first")]
        [HttpGet("/first")]
        public ActionResult<Author> GetFirst()
        {
            return context.Authors.FirstOrDefault();
        }

        [HttpGet("{id}", Name = "GetAuthor")]
        public async Task<ActionResult<AuthorDTO>> GetById(int id)
        {
            var author = await context.Authors.FirstOrDefaultAsync(a => a.Id == id);

            if (author == null)
            {
                return NotFound();
            }

            var authorDto = mapper.Map<AuthorDTO>(author);

            return authorDto;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] AuthorDTO_POST authorPost)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            var author = mapper.Map<Author>(authorPost);

            context.Authors.Add(author);
            await context.SaveChangesAsync();

            var authorDto = mapper.Map<AuthorDTO>(author);

            return new CreatedAtRouteResult("GetAuthor", new { id = author.Id }, authorDto);
        }

        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] Author author)
        {
            if (id != author.Id)
            {
                return BadRequest();
            }

            context.Entry(author).State = EntityState.Modified;
            context.SaveChanges();
            return Ok();
        }

        [HttpDelete("{id}")]
        public ActionResult<Author> Delete(int id)
        {
            var author = context.Authors.FirstOrDefault(a => a.Id == id);

            if (author == null)
            {
                return NotFound();
            }

            context.Authors.Remove(author);
            context.SaveChanges();
            return author;
        }
    }
}
