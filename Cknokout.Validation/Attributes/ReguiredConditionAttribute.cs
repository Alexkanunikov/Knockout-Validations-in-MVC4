using System.ComponentModel.DataAnnotations;

namespace Knockout.Validation.Attributes
{
    /// <summary>
    /// REquired by condition  validation attribute()
    /// </summary>
    public class ReguiredConditionAttribute: RequiredAttribute
    {
        #region Properties
        public string PropertyName { get; set; }
        public object PropertyValue { get; set; }
        #endregion

        /// <summary>
        /// makes field as required if an other files has predefined value
        /// </summary>
        /// <param name="propertyName">Property to be checked</param>
        /// <param name="propertyValue">Value to be compared</param>
        public ReguiredConditionAttribute(string propertyName, object propertyValue)
        {
            PropertyName = propertyName;
            PropertyValue =propertyValue;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            //if property value with name as PropertyName are equal to propertyValue  field  with ReguiredConditionAttribute will be required
            var property = validationContext.ObjectType.GetProperty(PropertyName);

            var propertyValue = property.GetValue(validationContext.ObjectInstance, null);

            return  propertyValue.Equals(PropertyValue)
                ? base.IsValid(value, validationContext)
                : ValidationResult.Success;
        }
    }
}