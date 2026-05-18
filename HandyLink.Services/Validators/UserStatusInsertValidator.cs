using FluentValidation;
using HandyLink.Model.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Services.Validators
{
    public class UserStatusInsertValidator : AbstractValidator<UserStatusInsertRequest>
    {
        public UserStatusInsertValidator() 
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.")
                .MaximumLength(50).WithMessage("Name cannot exceed 50 characters.")
                .MinimumLength(3).WithMessage("Name must have atleast 3 characters");
            RuleFor(x => x.Code).NotEmpty().WithMessage("Code is required.")
                .MaximumLength(50).WithMessage("Code cannot exceed 50 characters.")
                .MinimumLength(3).WithMessage("Code must have atleast 3 characters");
            RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required.")
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.")
                .MinimumLength(5).WithMessage("Description must have atleast 50 characters");

        }
    }
}
