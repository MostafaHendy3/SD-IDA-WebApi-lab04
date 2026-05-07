using System.ComponentModel.DataAnnotations;
using Lab03.Models;

namespace Lab06_MVC_01.Models
{
    public class PastDate : ValidationAttribute
    {
        protected override ValidationResult IsValid(
            object value,
            ValidationContext validationContext
        )
        {
            //compare age with DOB in ValidationContext
            var student = validationContext.ObjectInstance as Student;
            var dob = value as DateTime?;
            if (dob == null)
            {
                return new ValidationResult("DOB is required");
            }
            if (dob > DateTime.Now)
            {
                return new ValidationResult("DOB is in the future");
            }
            if (student.Age < 18 || student.Age > 20)
            {
                return new ValidationResult("Age is not in the range of 18 to 20");
            }
            if (student.Age != DateTime.Now.Year - dob.Value.Year)
            {
                return new ValidationResult("Age is not equal to DOB");
            }

            return ValidationResult.Success;
        }
    }
}
