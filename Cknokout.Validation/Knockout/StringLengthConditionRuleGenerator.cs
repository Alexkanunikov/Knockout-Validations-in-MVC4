using System.Text;
using Knockout.Validation.Attributes;

namespace Knockout.Validation.Knockout
{
    public class StringLengthConditionRuleGenerator :  BaseValidationGenerator<StringLengthCondition> 
    {
        protected override string GenerateClientValidationRule(StringLengthCondition validationAttribute, string propertyName)
        {
            var result = new StringBuilder();

            result.AppendFormat("ko.validation.addAnonymousRule(model['{0}'], {{" +
                                "validator: function (val, someOtherVal) {{" +
                                "if (model['{1}']() == {2}) {{" +
                                "return model['{0}']() != null && model['{0}']().length == {3};}}" +
                                "return true;}}," +
                                "message: '{4}'," +
                                "params: true}});", propertyName, validationAttribute.PropertyName, validationAttribute.PropertyValue.ToString().ToLower(), validationAttribute.MinimumLength, validationAttribute.LocalizableError());

            return result.ToString();
        }
    }
}