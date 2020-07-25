using CW.Soloist.CompositionService;
using CW.Soloist.WebApplication.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CW.Soloist.WebApplication.Validations
{
    public class PitchRangeValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            CompositionParamsViewModel formData =
                validationContext.ObjectInstance as CompositionParamsViewModel;

            if (formData == null)
                return ValidationResult.Success;

            int minPitch = (int)formData.MinPitch;
            int maxPitch = (int)formData.MaxPitch;

            if (Composition.isPitchRangeValid(minPitch, maxPitch, out string errorMessage))
                return ValidationResult.Success;
            else return new ValidationResult(errorMessage);
        }
    }
}