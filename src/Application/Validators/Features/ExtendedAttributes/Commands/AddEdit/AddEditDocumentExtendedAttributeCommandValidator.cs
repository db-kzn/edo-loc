using EDO_FOMS.Domain.Entities.ExtendedAttributes;
using EDO_FOMS.Domain.Entities.Doc;
using Microsoft.Extensions.Localization;

namespace EDO_FOMS.Application.Validators.Features.ExtendedAttributes.Commands.AddEdit
{
    public class AddEditDocumentExtendedAttributeCommandValidator : AddEditExtendedAttributeCommandValidator<int, int, Document, DocumentExtendedAttribute>
    {
        public AddEditDocumentExtendedAttributeCommandValidator(IStringLocalizer<AddEditExtendedAttributeCommandValidatorLocalization> localizer) : base(localizer)
        {
            // you can override the validation rules here
        }
    }
}