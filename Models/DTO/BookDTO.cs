using System.ComponentModel.DataAnnotations;

namespace coreWebApiDemo.Models.DTO
{
    public class BookDTO
    {
        public string Title { get; set; }
        public int AuthorId { get; set; }

        public AuthorDTO Author { get; set; }
    }
}