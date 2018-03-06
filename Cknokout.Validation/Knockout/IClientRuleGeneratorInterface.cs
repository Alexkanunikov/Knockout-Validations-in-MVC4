using System.ComponentModel.DataAnnotations;

namespace Knockout.Validation.Knockout
{
    public interface IClientRuleGeneratorInterface
    {
        string GenerateClientValidationRule(ValidationAttribute validationAttribute, string propertyName);
    }
}