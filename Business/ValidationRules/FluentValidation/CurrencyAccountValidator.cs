using Entities.Concrete;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.ValidationRules.FluentValidation
{
    public class CurrencyAccountValidator:AbstractValidator<CurrencyAccount>
    {
        public CurrencyAccountValidator()
        {
            RuleFor(p => p.Name).NotEmpty().WithMessage("Cari ad boş olamaz");
            RuleFor(p => p.Name).MinimumLength(4).WithMessage("Cari ad en az 4 karakter olmalıdır");
            RuleFor(p => p.Adress).NotEmpty().WithMessage("Cari adres boş olamaz");
            RuleFor(p => p.Adress).MinimumLength(4).WithMessage("Cari adres en az 4 karakter olmalıdır");
        }
    }
}
