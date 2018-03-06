using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;

namespace Mvc4KnockoutCRUD.ValidationAttributes
{
    public class ReguiredConditionAttribute: RequiredAttribute
    {
        #region Properties
        public string PropertyName { get; set; }
        public object PropertyValue { get; set; }
        #endregion

        public ReguiredConditionAttribute(string propertyName, object propertyValue)
        {
            PropertyName = propertyName;
            PropertyValue =propertyValue;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var property = validationContext.ObjectType.GetProperty(PropertyName);

            var propertyValue = property.GetValue(validationContext.ObjectInstance, null);

            return  propertyValue.Equals(PropertyValue)
                ? base.IsValid(value, validationContext)
                : ValidationResult.Success;
        }
    }
}