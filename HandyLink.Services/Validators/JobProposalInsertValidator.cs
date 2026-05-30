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
    public class JobProposalInsertValidator : AbstractValidator<JobProposalInsertRequest>
    {
        public JobProposalInsertValidator() {
            RuleFor(x => x.ProposedByUserId).NotEmpty().WithMessage("ProposedByUserId is required.").GreaterThan(0).WithMessage("ProposedByUserId must be greater than 0.");
            RuleFor(x => x.ProposedPrice).GreaterThan(0).When(x => x.ProposedPrice.HasValue).WithMessage("ProposedPrice must be greater than 0.");
            RuleFor(x => x.ProposedScheduledAtUtc).NotEmpty().WithMessage("ProposedScheduledAtUtc is required.").Must(x => x > DateTime.UtcNow).WithMessage("ProposedScheduledAtUtc must be in the future.");
            RuleFor(x => x.Note).MaximumLength(2000).When(x=>!string.IsNullOrWhiteSpace(x.Note)).WithMessage("Note cannot exceed 2000 characters.");
        }
    }
}
