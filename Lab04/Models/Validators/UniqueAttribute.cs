using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Lab04.Models.Validators
{
    internal class UniqueAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(
            object value,
            ValidationContext validationContext
        )
        {
            var db = (UniDbContext)validationContext.GetService(typeof(UniDbContext));
            var name = value as string;

            var exists = db.Departments.Any(d => d.Name == name);
            if (exists)
                return new ValidationResult("Department name must be unique");

            return ValidationResult.Success;
        }
    }
}
