using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voyara.Domain.ValueObjects
{

  

    public sealed class ValidationResult
    {
        private readonly List<string> _errors = [];

        public bool IsValid => _errors.Count == 0;

        public IReadOnlyCollection<string> Errors => _errors.AsReadOnly();

        private ValidationResult()
        {
        }

        private ValidationResult(IEnumerable<string> errors)
        {
            _errors.AddRange(errors);
        }

        public static ValidationResult Success()
        {
            return new ValidationResult();
        }

        public static ValidationResult Failure(IEnumerable<string> errors)
        {
            ArgumentNullException.ThrowIfNull(errors);

            var materializedErrors = errors
                .Where(error => !string.IsNullOrWhiteSpace(error))
                .ToList();

            if (materializedErrors.Count == 0)
            {
                throw new ArgumentException(
                    "A failed validation must contain at least one error.",
                    nameof(errors));
            }

            return new ValidationResult(materializedErrors);
        }

        public static ValidationResult Failure(string error)
        {
            return Failure([error]);
        }
    }

}
