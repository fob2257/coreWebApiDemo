using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace coreWebApiDemo.Models.DTO
{
    public class AuthorDTO_POST
    {
        [Required]
        public string Name { get; set; }
        public int Age { get; set; }
        public string Url { get; set; }
    }
}
