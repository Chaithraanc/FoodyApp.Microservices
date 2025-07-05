using System.ComponentModel.DataAnnotations;

namespace Foody.Web.Utility
{
    public class AllowedMaxFileSizeAttribute : ValidationAttribute
    {
        private readonly int _maxFileSizeInBytes;
        public AllowedMaxFileSizeAttribute(int maxFileSize)
        {
            _maxFileSizeInBytes = maxFileSize;
        }
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            var file = value as IFormFile;

            if (file != null && file.Length == 0)
            {

                if (file.Length > (_maxFileSizeInBytes * 1024 * 1024))
                {
                    return new ValidationResult($"File size exceeds the maximum allowed size of {_maxFileSizeInBytes} MB.");
                }
            }
            else if (file == null)
            {
                return new ValidationResult("File is required.");
            }

            return ValidationResult.Success;
        }
    }
    
}
