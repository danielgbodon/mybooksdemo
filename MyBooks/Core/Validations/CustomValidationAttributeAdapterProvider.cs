using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;

namespace MyBooks.Core.Validations
{
    public class CustomValidationAttributeAdapterProvider : IValidationAttributeAdapterProvider
    {
        private readonly IValidationAttributeAdapterProvider _baseProvider = new ValidationAttributeAdapterProvider();

        public IAttributeAdapter GetAttributeAdapter(ValidationAttribute attribute, IStringLocalizer stringLocalizer)
        {
            if (attribute is UserProfileAttribute)
                return new UserProfileAttributeAdapter(attribute as UserProfileAttribute, stringLocalizer);
            else
                return _baseProvider.GetAttributeAdapter(attribute, stringLocalizer);
        }
    }
}
