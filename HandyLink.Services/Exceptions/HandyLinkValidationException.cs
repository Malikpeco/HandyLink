using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Services.Exceptions
{
    /// <summary>
    /// Represents an error that occurs when the request data is invalid.
    /// </summary>
    public class HandyLinkValidationException : Exception
    {
        public IReadOnlyList<ValidationFailure> Errors { get; }

        public HandyLinkValidationException(string message) : base(message) 
        { 
            Errors = new List<ValidationFailure>();
        }

        public HandyLinkValidationException(IEnumerable<ValidationFailure> errors)
            :base("One or more validation errors occurred.")
        {
            Errors = errors.ToList();
        }
    }
}
