using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Mvc4KnockoutCRUD.Exstension;
using Mvc4KnockoutCRUD.ValidationAttributes;

namespace Mvc4KnockoutCRUD
{
    public static class HtmlHelperExstension
    {
        #region Const
        private const string ModelRootFormatString = @"$(function (section) {{section.{0} = function () {{var model = this;
                                                                                                         model.InitValidation = function() {{
                                                                                                                                            {1}
                                                                                                                                            }};{3};
                                                                                                                                                model.Errors = ko.validation.group(model);
                                                                                                                                                model.InitValidation();
                                                                                                                                                {4}
                                                                                                                                              }};
                                                                                                                                            }}({2}));";  
        #endregion

        public static HtmlString BuidKoBaseModel<TModelMain, TModel>(this HtmlHelper<TModelMain> htmlHelper, Func<TModelMain, TModel> modelGenerateTo, string modelName, string namespaceName, bool registerExtenders = true, bool messagesOnModified = true, bool insertMessages = true, bool decarateInput = true)
        {
            TModel model = modelGenerateTo(htmlHelper.ViewData.Model);

            return new HtmlString(string.Format(ModelRootFormatString, modelName, BulidValidationBody(model.GetType(), registerExtenders, messagesOnModified, insertMessages, decarateInput), namespaceName, SetUpDisplayPropertiesNames(model.GetType()), CreateValidatablePropertiesArray(model.GetType())));
        }

        public static HtmlString BuidKoBaseModelByType<TModelMain>(this HtmlHelper<TModelMain> htmlHelper, Type type, string modelName, string namespaceName, bool registerExtenders = true, bool messagesOnModified = true, bool insertMessages = true, bool decarateInput = true)
        {
            return new HtmlString(string.Format(ModelRootFormatString, modelName, BulidValidationBody(type, registerExtenders, messagesOnModified, insertMessages, decarateInput), namespaceName, SetUpDisplayPropertiesNames(type), CreateValidatablePropertiesArray(type)));
        }

        #region Helpers

        private static string BulidValidationBody(Type type, bool registerExtenders, bool messagesOnModified, bool insertMessages, bool decarateInput)
        {
            var result =new StringBuilder();

            result.Append(string.Format("ko.validation.init({{errorElementClass: 'validation-error'," +
                                        " decorateElement: {3}," +
                                        " registerExtenders: {0}," +
                                        " insertMessages: {2} }});", registerExtenders.ToString().ToLower(), messagesOnModified.ToString().ToLower(), insertMessages.ToString().ToLower(), decarateInput.ToString().ToLower()));


            
			result.Append("model.SetUpAllServerErrors = function(errors) {for (var name in errors) {" +
			              "var value = errors[name];" +
			              "model[name].SetUpServerError(errors[name], name);}};");

            foreach (var prop in type.GetProperties())
            {
                if (!IsPrimitiveType(prop.PropertyType) || prop.PropertyType.IsArray)
                {
                    continue;
                }

                result.Append(BuildAttributeBasedValidation(prop.GetCustomAttributes(typeof(ValidationAttribute), true).Cast<ValidationAttribute>().ToList(),prop.Name));
                result.Append(ExstendForServerValidation(prop.Name));
                result.Append(SetUpDisplayPropertyName(prop, prop.Name));
            }

            return result.ToString();
        }

        private static string CreateValidatablePropertiesArray(Type type)
        {

            var result = new StringBuilder();
            result.Append("model.ValidatedProperties = ko.observableArray();");

            foreach (var prop in type.GetProperties())
            {
                if (!IsPrimitiveType(prop.PropertyType) || prop.PropertyType.IsArray)
                {
                    continue;
                }

                if (prop.GetCustomAttributes(typeof (ValidationAttribute), true).Any())
                {
                    result.AppendFormat("model.ValidatedProperties.push(model['{0}']);", prop.Name);
                }
            }

            result.Append("model.ShowSummaryHints=ko.observable(false);");

            result.Append(
                "model.ControlsWithErrors = ko.computed(function () {var filtered = ko.utils.arrayFilter(model.ValidatedProperties(), function (item) {" +
                "return item.isValid() != true;});" +
                "return ko.utils.arrayMap(filtered, function (item) {return item.DisplayName;});}, model);");

            return result.ToString();
        }

        private static string SetUpDisplayPropertiesNames(Type type)
        {
            var result = new StringBuilder();
            foreach (var prop in type.GetProperties())
            {
                if (!IsPrimitiveType(prop.PropertyType) || prop.PropertyType.IsArray)
                {
                    continue;
                }
                result.Append(SetUpDisplayPropertyName(prop, prop.Name));
            }

            return result.ToString();
        }

        private static string SetUpDisplayPropertyName(PropertyInfo property, string propertyName)
        {
            var result =new StringBuilder();
            var displayNameAttribute = property.GetCustomAttributes(typeof(DisplayAttribute), false).Cast<DisplayAttribute>().FirstOrDefault();

            result.AppendFormat("model['{0}'].DisplayName=ko.observable('{1}');", propertyName, displayNameAttribute != null ? displayNameAttribute.LocalizableName() : propertyName);

            return result.ToString();
        }

        private static string ExstendForServerValidation(string propertyName)
        {
            var result =new StringBuilder();

            result.Append(string.Format("model['{0}'].HasSercverError = false;", propertyName));
            result.Append(string.Format("model['{0}'].ServerMessage = '';", propertyName));

            result.Append("model['" + propertyName +
                           "'].SetUpServerError = function(errorMsq, key) " +
                           "{var ruleName = model[key].rules()[model[key].rules().length - 1];" +
                           "var rule = ko.validation.rules[ruleName['rule']]; " +
                           "rule.message = errorMsq;" +
                           "model[key].HasSercverError = true;" +
                           "ko.validation.validateObservable(model[key]);" +
                           "model[key].HasSercverError = false;};");

            result.AppendFormat(
                "ko.validation.addAnonymousRule(model['{0}'], {{" +
                "validator: function(val, someOtherVal) {{  " +
                "return !model['{0}'].HasSercverError;" +
                "}}," +
                "message: 'Something must be really wrong!'," +
                "params: true}});", propertyName);

            return result.ToString();
        }

        private static string BuildAttributeBasedValidation(List<ValidationAttribute> attributes, string propertyName)
        {
            var result = new StringBuilder();

            foreach (var attribute in attributes)
            {
                if (attribute is ReguiredConditionAttribute)
                {
                    result.Append(BuildValidationWithRequiredDependencyAttribute(attribute as ReguiredConditionAttribute, propertyName));
                    continue;
                }

                if (attribute is RequiredAttribute)
                {
                    result.Append(BuildValidationWithRequiredAttribute(attribute as RequiredAttribute, propertyName));
                    continue;
                }

                if (attribute is MinLengthAttribute)
                {
                    result.Append(BuildValidationWithMinLengthAttribute(attribute as MinLengthAttribute, propertyName));
                    continue;
                }

                if (attribute is MaxLengthAttribute)
                {
                    result.Append(BuildValidationWithMaxLengthAttribute(attribute as MaxLengthAttribute, propertyName));
                    continue;
                }

                if (attribute is EmailAddressAttribute)
                {
                    result.Append(BuildValidationWithEmailLengthAttribute(attribute as EmailAddressAttribute, propertyName));
                    continue;
                }

                if (attribute is RegularExpressionAttribute)
                {
                    result.Append(BuildValidationWithMaxLengthAttribute(attribute as RegularExpressionAttribute, propertyName));
                }
            }

            return result.ToString();
        }

        private static string BuildValidationWithRequiredAttribute(RequiredAttribute requiredAttribute, string propertyName)
        {
            return
                string.Format(
                    @"model['{0}'].extend({{ required: {{ message: '{1}' }} }});", propertyName, requiredAttribute.LocalizableError());
        }

        private static string BuildValidationWithRequiredDependencyAttribute(ReguiredConditionAttribute requiredAttribute, string propertyName)
        {
            return
                string.Format(
                    @"model['{0}'].extend({{ required: {{
                                             message: '{1}',  
                                             onlyIf: function() {{
                                                                    return model['{2}']() === {3}; 
                                                                }} }} }});", 
                    propertyName,
                    requiredAttribute.LocalizableError(), 
                    requiredAttribute.PropertyName,
                    requiredAttribute.PropertyValue.ToString().ToLower());
        }

        private static string BuildValidationWithMinLengthAttribute(MinLengthAttribute minAttribute, string propertyName)
        {
            return
                string.Format(
                    @"model['{0}'].extend({{ minLength: {{ params: {1}, 
                                                           message: '{2}' }} }});", propertyName, minAttribute.Length, minAttribute.LocalizableError());
        }

        private static string BuildValidationWithMaxLengthAttribute(MaxLengthAttribute maxAttribute, string propertyName)
        {
            return
                string.Format(
                    @"model['{0}'].extend({{ maxLength: {{ params: {1}, 
                                                           message: '{2}' }} }});", propertyName, maxAttribute.Length, maxAttribute.ErrorMessage);
        }

        private static string BuildValidationWithEmailLengthAttribute(EmailAddressAttribute emailValidationAttribute, string propertyName)
        {
            Type type = typeof(EmailAddressAttribute);
            FieldInfo info = type.GetField("_regex", BindingFlags.NonPublic | BindingFlags.Static);
            var value = info.GetValue(null);

            return
                string.Format(
                    @"model['{0}'].extend({{
                         pattern: {{
                                    message: '{2}',
                                    params: /{1}/
                                    }} }});", propertyName, value, emailValidationAttribute.LocalizableError());
        }

        private static string BuildValidationWithMaxLengthAttribute(RegularExpressionAttribute regexpAtribute, string propertyName)
        {
            return
                string.Format(
                    @"model['{0}'].extend({{
                         pattern: {{
                                    message: '{2}',
                                    params: /{1}/
                                    }} }});", propertyName, regexpAtribute.Pattern, regexpAtribute.LocalizableError());
        }

        public static bool IsPrimitiveType(Type fieldType)
        {
            return fieldType.IsPrimitive || fieldType.Namespace.Equals("System");
        }
        #endregion        
    }
}