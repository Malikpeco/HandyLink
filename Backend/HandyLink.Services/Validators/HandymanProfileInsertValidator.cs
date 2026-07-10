using FluentValidation;
using HandyLink.Model.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Services.Validators
{
    public class HandymanProfileInsertValidator : AbstractValidator<HandymanProfileInsertRequest>
    {
        public HandymanProfileInsertValidator()
        {
            RuleFor(x => x.Bio).NotEmpty().WithMessage("Bio is required.")
                .MaximumLength(2500);
        }
            
    }
}
