using System.ComponentModel.DataAnnotations;

namespace coreWebApiDemo.Models.DTO
{
    public class BookDTO
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public int AuthorId { get; set; }

        public AuthorDTO Author { get; set; }
    }
}