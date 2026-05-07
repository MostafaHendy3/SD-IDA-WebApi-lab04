using System.Collections.Generic;

namespace Lab03.Models.DTOs
{
    public class DepartmentDto
    {
        public string DepartmentName { get; set; }
        public List<StudentDto> Students { get; set; } = new List<StudentDto>();
    }
}
