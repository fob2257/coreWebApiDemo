using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

using coreWebApiDemo.Business.Helpers;

namespace coreWebApiDemo.Models.DAL.Entities
{
    public class Author : IValidatableObject
    {
        public int Id { get; set; }
        [Required]
        // attribute validation
        [FirstLetter]
        [StringLength(10, ErrorMessage = "Name must have {1} character(s) or less")]
        public string Name { get; set; }
        [Range(18, 100)]
        public int Age { get; set; }
        [Url]
        public string Url { get; set; }

        //public List<Book> Books { get; set; }
        public List<AuthorBook> AuthorsBooks { get; set; }
        [NotMapped]
        public List<Book> Books { get { return (this.AuthorsBooks != null) ? this.AuthorsBooks.Select(o => o.Book).ToList() : new List<Book>(); } }

        // model validation
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrEmpty(Name))
            {
                var firstLetter = Name[0].ToString();

                if (firstLetter != firstLetter.ToUpper())
                {
                    yield return new ValidationResult("First letter must be capitalized", new string[] { nameof(Name) });
                }
            }
        }
    }
}
