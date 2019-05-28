using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using coreWebApiDemo.Models.DAL;
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
        public AuthorsController(ApplicationDbContext context, IClassService classService)
        {
            this.context = context;
            this.classService = classService;
        }

        [HttpGet("list")]
        [HttpGet]
        [ServiceFilter(typeof(MyActionFilter))]
        public ActionResult<IEnumerable<Author>> Get()
        {
            classService.DoSomething("ayyyylmao");
            return context.Authors.ToList();
        }

        [HttpGet("first")]
        [HttpGet("/first")]
        public ActionResult<Author> GetFirst()
        {
            return context.Authors.FirstOrDefault();
        }

        [HttpGet("{id}", Name = "GetAuthor")]
        public async Task<ActionResult<Author>> GetById(int id)
        {
            var author = await context.Authors.FirstOrDefaultAsync(a => a.Id == id);

            if (author == null)
            {
                return NotFound();
            }

            return author;
        }

        [HttpPost]
        public ActionResult Post([FromBody] Author author)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            context.Authors.Add(new Author { Name = author.Name });
            context.SaveChanges();

            return new CreatedAtRouteResult("GetAuthor", new { id = author.Id }, author);
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
