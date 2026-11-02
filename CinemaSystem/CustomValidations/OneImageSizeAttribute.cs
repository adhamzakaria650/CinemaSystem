using System.ComponentModel.DataAnnotations;

namespace CinemaSystem.CustomValidations
{
    public class OneImageSizeAttribute : ValidationAttribute
    {
        public override string FormatErrorMessage(string name)
        {
            return $"the {name} Size Must Be Between 0MB and 2MB";
        }
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is IFormFile image)
            {
                if (image.Length > 0 && image.Length < 2 * 1024 * 1024)
                {
                    return ValidationResult.Success;
                }
                return new ValidationResult(ErrorMessage ?? "The image size must be between 0MB and 2MB");
            }
            return ValidationResult.Success;
        }
    }
}
