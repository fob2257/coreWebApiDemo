using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace coreWebApiDemo.Models.DAL.Entities
{
    public class Book
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        //[Required]
        //public int AuthorId { get; set; }

        //public Author Author { get; set; }
        public List<AuthorBook> AuthorsBooks { get; set; }
        [NotMapped]
        public List<Author> Authors { get { return (this.AuthorsBooks != null) ? this.AuthorsBooks.Select(o => o.Author).ToList() : new List<Author>(); } }
    }
}
