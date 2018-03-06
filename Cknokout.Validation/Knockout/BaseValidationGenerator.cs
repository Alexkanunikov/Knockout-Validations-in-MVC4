using System.ComponentModel.DataAnnotations;

namespace Knockout.Validation.Knockout
{
    public abstract class BaseValidationGenerator<T> : IClientRuleGeneratorInterface where T : ValidationAttribute
    {
        /// <summary>
        /// Generates validation rule as string based on validation attribute
        /// </summary>
        /// <param name="validationAttribute"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public string GenerateClientValidationRule(ValidationAttribute validationAttribute, string propertyName)
        {
            return GenerateClientValidationRule(validationAttribute as T, propertyName);
        }
        protected abstract string GenerateClientValidationRule(T validationAttribute, string propertyName);
    }
}