using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Lab03.Models.Validators;

namespace Lab03.Models
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
