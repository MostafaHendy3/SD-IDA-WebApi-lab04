using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab01.Models
{
    [Table("Students")]
    [PrimaryKey("SSN")]
    public class Student
    {
        //SSN
        [Required]
        public int SSN { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        //age
        [Required]
        public int Age { get; set; }
        //address
        public string Address { get; set; }
        //image
        public string ImageUrl { get; set; }
        //level
        public string Level { get; set; }
    }
}
