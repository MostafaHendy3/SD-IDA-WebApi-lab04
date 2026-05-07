using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Lab04.Models.Validators;

namespace Lab04.Models
{
    public class Department
    {
        [Key]
        public int Id { get; set; }

        [Unique]
        public string Name { get; set; }
        public string Location { get; set; }

        public string PhoneNumber { get; set; }

        public string Manager { get; set; }

        public ICollection<Student> Students { get; set; }
    }
}
