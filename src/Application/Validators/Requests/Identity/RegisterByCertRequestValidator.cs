using EDO_FOMS.Application.Requests.Identity;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace EDO_FOMS.Application.Validators.Requests.Identity
{
    public class RegisterByCertRequestValidator : AbstractValidator<RegisterByCertRequest>
    {
        public RegisterByCertRequestValidator(IStringLocalizer<RegisterByCertRequestValidator> localizer)
        {
            RuleFor(request => request.Title)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Title is required"]);

            RuleFor(request => request.Org)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Org Name is required"]);

            //RuleFor(request => request.InnLe)
            //    .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["INN is required"])
            //    //.MinimumLength(10).WithMessage(localizer["INN must be 10 digits long"])
            //    .MaximumLength(10).WithMessage(localizer["INN must be 10 digits long"]);

            RuleFor(request => request.Snils)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["SNILS is required"]);

            RuleFor(request => request.Surname)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Surname is required"]);

            RuleFor(request => request.GivenName)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Given Name is required"]);

            RuleFor(request => request.Thumbprint)
               .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Thumbprint is required"]);

            RuleFor(request => request.FromDate)
               .Must(x => !string.IsNullOrWhiteSpace(x.ToString())).WithMessage(x => localizer["From Date is required"]);

            RuleFor(request => request.TillDate)
               .Must(x => !string.IsNullOrWhiteSpace(x.ToString())).WithMessage(x => localizer["Till Date is required"]);


            //RuleFor(request => request.Email)
            //    .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Email is required"])
            //    .EmailAddress().WithMessage(x => localizer["Email is not correct"]);
            //RuleFor(request => request.Org)
            //    .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["UserName is required"])
            //    .MinimumLength(6).WithMessage(localizer["UserName must be at least of length 6"]);
            //RuleFor(request => request.Password)
            //    .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Password is required!"])
            //    .MinimumLength(8).WithMessage(localizer["Password must be at least of length 8"])
            //    .Matches(@"[A-Z]").WithMessage(localizer["Password must contain at least one capital letter"])
            //    .Matches(@"[a-z]").WithMessage(localizer["Password must contain at least one lowercase letter"])
            //    .Matches(@"[0-9]").WithMessage(localizer["Password must contain at least one digit"]);
            //RuleFor(request => request.ConfirmPassword)
            //    .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Password Confirmation is required!"])
            //    .Equal(request => request.Password).WithMessage(x => localizer["Passwords don't match"]);
        }
    }
}
