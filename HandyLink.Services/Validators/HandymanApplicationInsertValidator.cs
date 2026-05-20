using FluentValidation;
using HandyLink.Model.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Services.Validators
{
    public class HandymanApplicationInsertValidator : AbstractValidator<HandymanApplicationInsertRequest>
    {
        public HandymanApplicationInsertValidator() 
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required.");
            RuleFor(x => x.ExperienceYears).NotEmpty().WithMessage("ExperienceYears is required.")
                .GreaterThanOrEqualTo(0).WithMessage("ExperienceYears cannot be less that 0.");
            RuleFor(x => x.WorkDescription).NotEmpty().WithMessage("WorkDescription is required.")
                .MaximumLength(1000).WithMessage("WorkDescription cannot exceed 1000 characters.");
            RuleFor(x => x.ServiceCategoryIds).NotEmpty().WithMessage("ServiceCategoryIds are required.");
            RuleForEach(x => x.ServiceCategoryIds).GreaterThan(0).WithMessage("ServiceCategoryId must be valid.");
        }
    }
}
