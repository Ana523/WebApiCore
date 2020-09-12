using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace UserWebApi.Models
{
    // Default validation => IdentityUser
    public class ApplicationUser : IdentityUser
    {
        // Insert new column full name into the table
        [Column(TypeName = "nvarchar(150)")]
        public string FullName { get; set; }
    }
}
