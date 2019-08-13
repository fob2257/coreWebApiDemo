using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace coreWebApiDemo.Models.DTO
{
    public class BookDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        //public int AuthorId { get; set; }

        //public AuthorDTO Author { get; set; }

        public List<AuthorDTO> Authors { get; set; }
    }
}