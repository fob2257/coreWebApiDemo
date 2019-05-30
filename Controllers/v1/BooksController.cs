using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using coreWebApiDemo.Models.DAL;
using coreWebApiDemo.Models.DAL.Entities;

namespace coreWebApiDemo.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        public BooksController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Book>> Get()
        {
            return context.Books.Include(a => a.Author).ToList();
        }

        [HttpGet("{id}", Name = "GetBook")]
        public ActionResult<Book> GetById(int id)
        {
            var book = context.Books.FirstOrDefault(a => a.Id == id);

            if (book == null)
            {
                return NotFound();
            }

            return book;
        }

        [HttpPost]
        public ActionResult Post([FromBody] Book book)
        {
            context.Books.Add(new Book { Title = book.Title, AuthorId = book.AuthorId });
            context.SaveChanges();

            return new CreatedAtRouteResult("GetBook", new { id = book.Id }, book);
        }
    }
}
