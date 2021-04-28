using System;
using System.Collections.Generic;
using System.Linq;

namespace Money.Api.Exceptions
{
    public class ValidationException : Exception
    {
        public IDictionary<string, string[]> Errors { get; }
        public string ErrorMessageId { get; }
        public string Error
        {
            get
            {
                var errors = Errors.Values.SelectMany(kvp => kvp).Distinct().ToList();
                return string.Join(Environment.NewLine, errors);
            }
        }

        public ValidationException()
            : base("One or more validation failures have occurred.")
        {
            Errors = new Dictionary<string, string[]>();
            ErrorMessageId = string.Empty;
        }

        public ValidationException(IDictionary<string, string[]> errors)
            : this()
        {
            Errors = errors;
        }

        public ValidationException(IDictionary<string, string[]> errors, string errorMessageId)
            : this()
        {
            Errors = errors;
            ErrorMessageId = errorMessageId;
        }

        private ValidationException(string message) : base(message)
        {
        }

        private ValidationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
