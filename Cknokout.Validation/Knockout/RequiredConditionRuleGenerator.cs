using Knockout.Validation.Attributes;

namespace Knockout.Validation.Knockout
{
    public class RequiredConditionRuleGenerator : BaseValidationGenerator<ReguiredConditionAttribute>
    {

        protected override string GenerateClientValidationRule(ReguiredConditionAttribute validationAttribute, string propertyName)
        {
            return
                    string.Format(
                    @"model['{0}'].extend({{ required: {{
                                                                 message: '{1}',
                                                                 onlyIf: function() {{
                                                                                        return model['{2}']() === {3};
                                                                                    }} }} }});",
                    propertyName,
                    validationAttribute.LocalizableError(),
                    validationAttribute.PropertyName,
                    validationAttribute.PropertyValue.ToString().ToLower());
        }
    }
}