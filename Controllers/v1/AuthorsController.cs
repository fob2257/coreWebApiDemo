using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

using coreWebApiDemo.Models.DAL;
using coreWebApiDemo.Models.DTO;
using coreWebApiDemo.Models.DAL.Entities;
using coreWebApiDemo.Business.Helpers;

[assembly: ApiConventionType(typeof(DefaultApiConventions))]

namespace coreWebApiDemo.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AuthorsController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public AuthorsController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

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
                .Include(a => a.AuthorsBooks)
                    .ThenInclude(a => a.Book)
                .ToListAsync();
            var authorsDTO = mapper.Map<List<AuthorDTO>>(authors);

            Response.Headers["X-Total-Rows"] = total.ToString();
            Response.Headers["X-Total-Pages"] =
                ((int)Math.Ceiling((double)total / totalRows)).ToString();

            return authorsDTO;
        }

        // GET: api/Authors/first
        [HttpGet("first")]
        public async Task<ActionResult<AuthorDTO>> GetFirst() => mapper.Map<AuthorDTO>(await context.Authors
            .Include(a => a.AuthorsBooks)
                    .ThenInclude(a => a.Book)
            .FirstOrDefaultAsync());

        // GET: api/Authors/5
        [HttpGet("{id}", Name = "GetAuthorV1")]
        public async Task<ActionResult<AuthorDTO>> GetById(int id)
        {
            var author = await context.Authors
                .Include(a => a.AuthorsBooks)
                    .ThenInclude(a => a.Book)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (author == null)
            {
                return NotFound();
            }

            var authorDTO = mapper.Map<AuthorDTO>(author);

            return authorDTO;
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

            var authorDTO = mapper.Map<AuthorDTO>(author);

            return new CreatedAtRouteResult("GetAuthorV1", new { id = author.Id }, authorDTO);
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
        public async Task<ActionResult> Delete(int id)
        {
            var author = await context.Authors.FirstOrDefaultAsync(a => a.Id == id);

            if (author == null)
            {
                return NotFound();
            }

            context.Authors.Remove(author);
            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}
