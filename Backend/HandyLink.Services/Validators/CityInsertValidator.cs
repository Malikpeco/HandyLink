using FluentValidation;
using HandyLink.Model.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Services.Validators
{
    public class CityInsertValidator : AbstractValidator<CityInsertRequest>
    {
        public CityInsertValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.")
                .MinimumLength(3).WithMessage("Name must have atleast 3 characters.");

            RuleFor(x => x.CountryId)
                .GreaterThan(0).WithMessage("CountryId is required and must be greater than 0.");
        }
    }
}
