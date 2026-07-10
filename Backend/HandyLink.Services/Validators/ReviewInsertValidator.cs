using FluentValidation;
using HandyLink.Model.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Services.Validators
{
    public class ReviewInsertValidator : AbstractValidator<ReviewInsertRequest>
    {
        public ReviewInsertValidator() 
        {
            RuleFor(x => x.ClientProfileId).GreaterThan(0).WithMessage("ClientProfileId must be greater than 0.");
            RuleFor(x => x.Rating).GreaterThan(0).LessThan(6).WithMessage("Rating must be from 1 to 5.");
            RuleFor(x => x.Comment).MaximumLength(1000).WithMessage("Comment cannot exceed 1000 characters.");
        }
    }
}
