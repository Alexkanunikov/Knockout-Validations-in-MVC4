using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Knockout.Validation.Knockout
{
    /// <summary>
    /// Registers custom validation attribute and generates ko rules based on custom validation attribute
    /// </summary> 
    public static class CustomAttrValidationRegistrator
    {
        private static Dictionary<Type, IClientRuleGeneratorInterface> _container;

        static CustomAttrValidationRegistrator()
        {
            _container = new Dictionary<Type, IClientRuleGeneratorInterface>();
        }

        /// <summary>
        /// Registers rule generator
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="validationGenerator">Ko Rule generator by ValidationAttribute</param>
        public static void Register<T>(BaseValidationGenerator<T> validationGenerator) where T : ValidationAttribute
        {
            var validationAttributeType =typeof(T) ;
            _container.Add(validationAttributeType, validationGenerator);
        }

        public static bool IsRegistered(ValidationAttribute validationAttribute)
        {
            return _container.ContainsKey(validationAttribute.GetType());
        }

        /// <summary>
        /// Generated ko validation rule based on ValidationAttribute
        /// </summary>
        /// <param name="validationAttribute"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static string GenerateRuleIfRegistered(ValidationAttribute validationAttribute, string propertyName)
        {
            if (_container.ContainsKey(validationAttribute.GetType()))
            {
               return (_container[validationAttribute.GetType()]).GenerateClientValidationRule(validationAttribute, propertyName);
            }

            return string.Empty;
        }
    }
}