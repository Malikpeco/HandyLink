using FluentValidation;
using HandyLink.Model.Database.Enums;
using HandyLink.Model.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Services.Validators
{
    public class JobInsertValidator : AbstractValidator<JobInsertRequest>
    {
        public JobInsertValidator() {
            RuleFor(x => x.ClientProfileId).NotEmpty().WithMessage("ClientProfileId is required.").GreaterThan(0).WithMessage("ClientProfileId must be greater than 0.");
            RuleFor(x => x.HandymanProfileId).GreaterThan(0).When(x=>x.HandymanProfileId.HasValue).WithMessage("HandymanProfileId must be greater than 0.");
            RuleFor(x => x.ServiceCategoryId).NotEmpty().WithMessage("ServiceCategoryId is required.").GreaterThan(0).WithMessage("ServiceCategoryId must be greater than 0.");
            RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required.").MaximumLength(150).WithMessage("Title cannot exceed 150 characters.");
            RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required.").MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters.");
            RuleFor(x => x.CityId).NotEmpty().WithMessage("CityId is required.").GreaterThan(0).WithMessage("CityId must be greater than 0.");
            RuleFor(x => x.Address).MaximumLength(200).When(x => !string.IsNullOrWhiteSpace(x.Address)).WithMessage("Address cannot exceed 200 characters.");
            RuleFor(x => x.JobCreationType).IsInEnum().NotEqual(JobCreationType.Unknown).WithMessage("JobCreationType is invalid.");
            RuleFor(x => x.InitialPrice).GreaterThan(0).When(x => x.InitialPrice.HasValue).WithMessage("InitialValue must be greater than 0.");
            RuleFor(x => x.InitialScheduledAtUtc).NotEmpty().WithMessage("InitialScheduledAt is required.").Must(x => x > DateTime.UtcNow).WithMessage("InitialScheduledAt must be in the future.");

        }
    }
}
