using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;

namespace MyBooks.Core.Validations
{
    public class UserProfileAttributeAdapter : AttributeAdapterBase<UserProfileAttribute>
    {
        public UserProfileAttributeAdapter(UserProfileAttribute attribute, IStringLocalizer stringLocalizer) : base(attribute, stringLocalizer) { }

        public override void AddValidation(ClientModelValidationContext context) { }

        public override string GetErrorMessage(ModelValidationContextBase validationContext)
        {
            return GetErrorMessage(validationContext.ModelMetadata, validationContext.ModelMetadata.GetDisplayName());
        }
    }
}
