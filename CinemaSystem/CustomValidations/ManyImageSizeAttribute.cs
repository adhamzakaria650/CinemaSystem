using System.ComponentModel.DataAnnotations;

namespace CinemaSystem.CustomValidations
{
    public class ManyImageSizeAttribute:  ValidationAttribute 
    {
        public override string FormatErrorMessage(string name)
        {
            return $"Your {name} Sizes must be between 0MB and 2MB";
        }
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is List<IFormFile> images)
            {
                if (images.Count > 0)
                {
                    foreach (var image in images)
                    {
                        if (image.Length <= 0 || image.Length >= 2 * 1024 * 1024)
                        {
                            return new ValidationResult(ErrorMessage??"Your images sizes must be between 0MB and 2MB");
                        }
                    }
                    return ValidationResult.Success;
                }
                return new ValidationResult( ErrorMessage??"Image list cannot be empty");
            }
            return ValidationResult.Success;
        }
    }
}
