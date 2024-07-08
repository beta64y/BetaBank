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
                return new ValidationResult("Doğum tarihi gereklidir.");
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
                    return new ValidationResult($"Yaşınız en az {_minAge} olmalıdır.");
                }
                return ValidationResult.Success;
            }
            return new ValidationResult("Geçersiz doğum tarihi.");
        }
    }
}
