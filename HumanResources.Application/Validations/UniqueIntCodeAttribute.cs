using HumanResources.Infrastructure.DbContext;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanResources.Application.Validations
{
    public class UniqueIntCode : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success; // Skip validation if null
            }

            if (value is not int code)
            {
                return new ValidationResult("الكود يجب أن يكون رقمًا صحيحًا."); // Return error if not an integer
            }

            var context = (ApplicationDbContext)validationContext.GetService(typeof(ApplicationDbContext));

            // Check if the code already exists in the database
            var exists = context.EmployeeTbl.Any(e => e.Code == code);

            if (exists)
            {
                return new ValidationResult("الكود يجب أن يكون فريدًا."); // Custom error message
            }

            return ValidationResult.Success; // Validation passed
        }
    }
}
