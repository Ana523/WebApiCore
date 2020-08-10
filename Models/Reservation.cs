using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace UserWebApi.Models
{
    public class Reservation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(150)")]
        public string FirstName { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(150)")]
        public string LastName { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateTime DateFrom { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateTime DateTo { get; set; }

        [Required]
        [Column(TypeName = "int")]
        public int NumOfPeople { get; set; }

        [Required]
        [Column(TypeName = "int")]
        public int NumOfRooms { get; set; }
    }
}
