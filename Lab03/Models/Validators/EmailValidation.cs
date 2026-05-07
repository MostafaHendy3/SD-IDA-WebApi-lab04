using System.ComponentModel.DataAnnotations;

namespace Lab03.Models.Validators
{
    public class EmailValidation : ValidationAttribute
    {
        protected override ValidationResult? IsValid(
            object? value,
            ValidationContext validationContext
        )
        {
            var schoolContext = validationContext.GetService<UniDbContext>();
            var student = validationContext.ObjectInstance as Student;
            var myssn = student.SSN;
            if (schoolContext == null)
            {
                return new ValidationResult("Database context is not available");
            }

            if (value == null)
            {
                return new ValidationResult("Email is required");
            }
            else
            {
                if (value is string)
                {
                    string suppliedValue = (string)value;
                    var emailexists = schoolContext
                        .Students.Where(s => s.SSN != myssn)
                        .Any(s => s.Email == suppliedValue);

                    if (emailexists)
                    {
                        return new ValidationResult("Email already exists");
                    }
                }
                else
                {
                    return new ValidationResult("Invalid email format");
                }

                return ValidationResult.Success;
            }
        }
    }
}
