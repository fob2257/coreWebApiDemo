using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;

using coreWebApiDemo.Models.DAL;
using coreWebApiDemo.Models.DTO;
using coreWebApiDemo.Models.DAL.Entities;
using coreWebApiDemo.Services;
using coreWebApiDemo.Helpers;

[assembly: ApiConventionType(typeof(DefaultApiConventions))]

namespace coreWebApiDemo.Controllers.v1
{
    [Route("api/v1/[controller]")]
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

        [HttpGet]
        [HttpGet("list")]
        [ServiceFilter(typeof(MyActionFilter))]
        public async Task<ActionResult<IEnumerable<AuthorDTO>>> Get(int page = 1, int totalRows = 25)
        {
            classService.DoSomething("ayyyylmao");

            var query = context.Authors.AsQueryable();

            var total = query.Count();

            var authors = await query
                .Skip(totalRows * (page - 1))
                .Take(totalRows)
                .Include(a => a.Books)
                .ToListAsync();
            var authorsDto = mapper.Map<List<AuthorDTO>>(authors);

            Response.Headers["X-Total-Rows"] = total.ToString();
            Response.Headers["X-Total-Pages"] =
                ((int)Math.Ceiling((double)total / totalRows)).ToString();

            return authorsDto;
        }

        [HttpGet("first")]
        [HttpGet("/first")]
        public ActionResult<Author> GetFirst()
        {
            return context.Authors.FirstOrDefault();
        }

        [HttpGet("{id}", Name = "GetAuthorV1")]
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

            return new CreatedAtRouteResult("GetAuthorV1", new { id = author.Id }, authorDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] AuthorDTO_PUT authorPut)
        {
            var author = mapper.Map<Author>(authorPut);
            author.Id = id;

            context.Entry(author).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<Author> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            var author = await context.Authors.FirstOrDefaultAsync(a => a.Id == id);

            if (author == null)
            {
                return NotFound();
            }

            patchDocument.ApplyTo(author, ModelState);


            var isValid = TryValidateModel(author);

            if (!isValid)
            {
                return BadRequest(ModelState);
            }

            await context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Deletes a specific Author.
        /// </summary>
        /// <param name="id">Id of the author to delete</param>
        [HttpDelete("{id}")]
        public async Task<ActionResult<Author>> Delete(int id)
        {
            var authorId = await context.Authors
                .Select(a => a.Id)
                .FirstOrDefaultAsync(Id => Id == id);

            if (authorId == default(int))
            {
                return NotFound();
            }

            context.Authors.Remove(new Author { Id = authorId });
            context.SaveChanges();
            return NoContent();
        }
    }
}
