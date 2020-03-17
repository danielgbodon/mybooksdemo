using System.ComponentModel.DataAnnotations;

namespace MyBooks.Core.Validations
{
    public class UserProfileAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            return (AppEnvironment.UserProfiles.Contains(value.ToString().ToLower())) ? ValidationResult.Success : new ValidationResult(ErrorMessage);
        }
    }
}
