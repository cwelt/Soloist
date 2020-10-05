using CW.Soloist.CompositionService;
using CW.Soloist.WebApplication.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace CW.Soloist.WebApplication.Validations
{
    /// <summary>
    /// Custom validation class for validating pitch range in composition form.
    /// </summary>
    public class PitchRangeValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            CompositionViewModel formData = validationContext.ObjectInstance as CompositionViewModel;

            if (formData == null)
                return ValidationResult.Success;

            int minPitch = (int)formData.MinPitch;
            int maxPitch = (int)formData.MaxPitch;

            if (CompositionContext.IsPitchRangeValid(minPitch, maxPitch, out string errorMessage))
                return ValidationResult.Success;
            else return new ValidationResult(errorMessage);
        }
    }
}