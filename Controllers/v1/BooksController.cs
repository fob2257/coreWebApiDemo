using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

using coreWebApiDemo.Models.DAL;
using coreWebApiDemo.Models.DAL.Entities;
using coreWebApiDemo.Models.DTO;

namespace coreWebApiDemo.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class BooksController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public BooksController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookDTO>>> Get(int page = 1, int totalRows = 25)
        {
            var query = context.Books.AsQueryable();

            var total = await query.CountAsync();

            var books = await query
                .Skip(totalRows * (page - 1))
                .Take(totalRows)
                .Include(b => b.AuthorsBooks)
                    .ThenInclude(a => a.Author)
                .ToListAsync();

            var booksDTO = mapper.Map<List<BookDTO>>(books);

            Response.Headers["X-Total-Rows"] = total.ToString();
            Response.Headers["X-Total-Pages"] = ((int)Math.Ceiling((double)total / totalRows)).ToString();

            return booksDTO;
        }

        [HttpGet("{id}", Name = "GetBookV1")]
        public async Task<ActionResult<BookDTO>> GetById(int id)
        {
            var book = await context.Books
                .Include(b => b.AuthorsBooks)
                    .ThenInclude(a => a.Author)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (book == null)
            {
                return NotFound();
            }

            var bookDTO = mapper.Map<BookDTO>(book);

            return bookDTO;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] BookDTO_POST body)
        {
            var book = mapper.Map<Book>(body);

            context.Books.Add(book);
            await context.SaveChangesAsync();

            var bookDTO = mapper.Map<BookDTO>(book);

            return new CreatedAtRouteResult("GetBookV1", new { id = book.Id }, bookDTO);
        }
    }
}
