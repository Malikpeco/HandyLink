using FluentValidation;
using HandyLink.Model.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Services.Validators
{
    public class UserUpdateValidator : AbstractValidator<UserUpdateRequest>
    {
        public UserUpdateValidator() 
        {
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("FirstName is required.")
                .MaximumLength(50).WithMessage("FirstName cannot exceed 50 characters.")
                .MinimumLength(2).WithMessage("FirstName must have atleast 2 characters.");

            RuleFor(x => x.LastName).NotEmpty().WithMessage("LastName is required.")
                .MaximumLength(50).WithMessage("LastName cannot exceed 50 characters.")
                .MinimumLength(2).WithMessage("LastName must have atleast 2 characters.");

            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required.")
                .MaximumLength(200).WithMessage("Email cannot exceed 200 characters")
                .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]{2,}$").WithMessage("Email format is invalid.");

            RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("PhoneNumber is required.")
                .Matches(@"^(?=(?:.*\d){7,15})\+?[0-9\s\/\-\(\)]{7,20}$").WithMessage("PhoneNumber format is invalid.");

            RuleFor(x => x.CityId).NotEmpty().WithMessage("CityId is required.");
        }
    }
}
