using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace coreWebApiDemo.Models.DTO
{
    public class BookDTO_POST
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public int AuthorId { get; set; }
    }
}
