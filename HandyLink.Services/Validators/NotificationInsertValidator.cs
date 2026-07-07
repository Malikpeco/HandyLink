using FluentValidation;
using HandyLink.Model.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Services.Validators
{
    public class NotificationInsertValidator : AbstractValidator<NotificationInsertRequest>
    {
        public NotificationInsertValidator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(100).WithMessage("Title must have 1-100 characters.");
            RuleFor(x => x.Content).NotEmpty().MaximumLength(200).WithMessage("Content must have 1-200 characters.");
            RuleFor(x => x.UserId).NotEmpty().GreaterThan(0).WithMessage("UserId must be greater than 0.");
        }
    }
}
