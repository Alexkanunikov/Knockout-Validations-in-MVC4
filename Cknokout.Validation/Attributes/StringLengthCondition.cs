using System;
using System.ComponentModel.DataAnnotations;

namespace Knockout.Validation.Attributes
{
    [System.AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class StringLengthCondition : StringLengthAttribute
    {
        #region Properties
        public string PropertyName { get; set; }
        public object PropertyValue { get; set; }
        #endregion

        #region Constructors


        public StringLengthCondition(int maximumLength, string propertyName, object propertyValue): base(maximumLength)
        {
            PropertyName = propertyName;
            PropertyValue = propertyValue;
        }
        #endregion

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var property = validationContext.ObjectType.GetProperty(PropertyName);

            var propertyValue = property.GetValue(validationContext.ObjectInstance, null);

            return propertyValue.Equals(PropertyValue)
                ? base.IsValid(value, validationContext)
                : ValidationResult.Success;
        }
    }
}