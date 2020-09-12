using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UserWebApi.Models
{
    public class ApplicationUserModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9._-]*@[a-z]*\.[a-z]*$")]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string FullName { get; set; }
    }
}
