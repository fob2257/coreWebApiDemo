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
using coreWebApiDemo.Business.Helpers;
using coreWebApiDemo.Business.Services;

[assembly: ApiConventionType(typeof(DefaultApiConventions))]

namespace coreWebApiDemo.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IDIService diService;
        private readonly IMapper mapper;

        public AuthorsController(ApplicationDbContext context, IDIService diService, IMapper mapper)
        {
            this.context = context;
            this.diService = diService;
            this.mapper = mapper;
        }

        // GET: api/caching
        [HttpGet("/api/caching")]
        [ResponseCache(Duration = 15)]
        public ActionResult<DateTime> GetTime() => DateTime.UtcNow;

        // GET: api/dependency-injection
        [HttpGet("/api/dependency-injection")]
        public ActionResult<string> GetDI() => diService.DoSomethingMethod("ayyylmao");

        // GET: api/Authors
        // GET: api/Authors/list
        [HttpGet]
        [HttpGet("list")]
        [ServiceFilter(typeof(MyActionFilter))]
        public async Task<ActionResult<IEnumerable<AuthorDTO>>> Get(int page = 1, int totalRows = 25)
        {
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

        // GET: api/Authors/first
        // GET: first
        [HttpGet("first")]
        [HttpGet("/first")]
        public ActionResult<Author> GetFirst()
        {
            return context.Authors.FirstOrDefault();
        }

        // GET: api/Authors/5
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

        // POST: api/Authors
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

        // PUT: api/Authors/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] AuthorDTO_PUT authorPut)
        {
            var author = mapper.Map<Author>(authorPut);
            author.Id = id;

            context.Entry(author).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return NoContent();
        }

        // PATCH: api/Authors/5
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

        // DELETE: api/ApiWithActions/5
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
