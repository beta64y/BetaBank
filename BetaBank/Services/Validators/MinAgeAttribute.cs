using System.ComponentModel.DataAnnotations;

namespace BetaBank.Services.Validators
{
    public class MinAgeAttribute : ValidationAttribute
    {
        private readonly int _minAge;

        public MinAgeAttribute(int minAge)
        {
            _minAge = minAge;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("Date of birth is required.");
            }

            if (value is DateTime date)
            {
                var age = DateTime.Today.Year - date.Year;
                if (date > DateTime.Today.AddYears(-age))
                {
                    age--;
                }

                if (age < _minAge)
                {
                    return new ValidationResult($"Your age must be at least {_minAge}.");
                }
                return ValidationResult.Success;
            }
            return new ValidationResult("Invalid date of birth.");
        }
    }
}
