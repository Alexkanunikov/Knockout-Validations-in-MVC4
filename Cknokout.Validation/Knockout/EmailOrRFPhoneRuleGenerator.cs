using Knockout.Validation.Attributes;

namespace Knockout.Validation.Knockout
{
    public class EmailOrRFPhoneRuleGenerator : BaseValidationGenerator<EmailOrRFPhoneAttribute> 
    {
        protected override string GenerateClientValidationRule(EmailOrRFPhoneAttribute validationAttribute, string propertyName)
        {
            return string.Format("model['{0}'].extend( {{" +
                                 "validation: {{validator: function ( val, someOtherVal ) {{	" +
                                 "var result = false;if (val == null) return true;" +
                                 "if (val.indexOf('@') > -1) {{var emailPattern = /{1}/i;" +
                                 "result = emailPattern.test( val );" +
                                 "}} else {{" +
                                 "var phonePattern = /{2}/;" +
                                 "result = phonePattern.test( val );" +
                                 "}}return result;}}," +
                                 "message: function () {{" +
                                 " return model['{0}']() != null && model['{0}']().indexOf('@') > -1 ? '{3}' : '{4}'; }}" +
                                 ",params: true}} }});", propertyName, validationAttribute.EmailPattern, validationAttribute.PhonePattern, validationAttribute.EmailErrorMessage,validationAttribute.PhoneErroreMessage);
        }
    }
}