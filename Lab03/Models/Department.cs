using System.Collections.Generic;

namespace Lab03.Models
{
    public class Department
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Location { get; set; }

        public string PhoneNumber { get; set; }

        public string Manager { get; set; }

        public ICollection<Student> Students { get; set; }
    }
}
