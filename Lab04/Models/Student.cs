using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Lab04.Models.Validators;
using Lab06_MVC_01.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab04.Models
{
    [Table("Students")]
    [PrimaryKey("SSN")]
    public class Student
    {
        //SSN
        [Required]
        public int SSN { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(12)]
        public string Name { get; set; } = string.Empty;

        //age
        [Required]
        //(18,20)
        [Range(18, 20)]
        public int Age { get; set; }

        [PastDate]
        public DateTime DOB { get; set; }

        [EmailValidation]
        public string Email { get; set; } = string.Empty;

        //address
        public string Address { get; set; }

        //image
        public string ImageUrl { get; set; }

        //level
        public string Level { get; set; }
    }
}
