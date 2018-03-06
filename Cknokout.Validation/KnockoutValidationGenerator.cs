using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using Knockout.Validation.Exstensions;
using Knockout.Validation.Knockout;

namespace Knockout.Validation
{
    public class KnockoutValidationGenerator
    {
        private const string ModelRootFormatString = @"{5}$(function (section) {{section.{0} = function () {{var model = this;
                                                                                                         model.InitValidation = function() {{
                                                                                                                                            {1}
                                                                                                                                            }};{3};
                                                                                                                                                model.Errors = ko.validation.group(model);
                                                                                                                                                model.InitValidation();
                                                                                                                                                {4}
                                                                                                                                              }};
                                                                                                                                            }}({2}));";

        public string GenerateValidationModel(Type type, string modelName, string namespaceName, KnockoutValidationOptions options = null, bool genateChildModels = false, string idPrefixs = "")
        {
            return string.Format(ModelRootFormatString,
                                     modelName,
                                     BulidValidationBody(type, idPrefixs, options, genateChildModels),
                                     namespaceName, SetUpDisplayPropertiesNames(type), CreateValidatablePropertiesArray(type),
                                     GenerateChildModels(type, options, namespaceName, genateChildModels));
        }


        private string GenerateChildModels(Type type, KnockoutValidationOptions options, string namespaceName, bool genateChildModels)
        {
            var result = new StringBuilder();

            if (genateChildModels)
            {
                foreach (var prop in type.GetProperties())
                {
                    if (IsCollection(prop.PropertyType)) continue;

                    if (!IsPrimitiveType(prop.PropertyType))
                    {
                        result.Append(new HtmlString(string.Format(ModelRootFormatString, prop.Name + "Base",
                            BulidValidationBody(prop.PropertyType, prop.Name, options, true),
                            namespaceName, SetUpDisplayPropertiesNames(prop.PropertyType), CreateValidatablePropertiesArray(prop.PropertyType),
                            GenerateChildModels(prop.PropertyType, options, namespaceName, true))));
                    }
                }

                result = FormatString(result);

                return result.ToString();
            }

            return string.Empty;
        }

        #region Helpers

        private string BulidValidationBody(Type type, string idPrefixs, KnockoutValidationOptions options, bool generateChildModels)
        {
            var result = new StringBuilder();

            result.Append(string.Format("ko.validation.init({{errorElementClass: 'error'," +
                                        " decorateElement: {3}," +
                                        " decorateInputElement: true," +
                                        " registerExtenders: {0}," +
                                        " insertMessages: {2} }});", options.RegisterExtenders.ToString().ToLower(), options.MessagesOnModified.ToString().ToLower(), options.InsertMessages.ToString().ToLower(), options.DecarateInput.ToString().ToLower()));

            result.Append("model.ValidatingChildren=[];");

            result.Append("model.SetUpAllServerErrors = function(errors) {" +
                          "for (var name in errors) {" +
                          "if (name.indexOf('.') != -1) {continue;}" +
                          "var value = errors[name];" +
                          "model[name].SetUpServerError(errors[name], name);" +
                          "model.Errors.showAllMessages();}				" +
                          "var childProperties = splitPropertyAndChildModelsKeys(errors);if (childProperties.length >0) {" +
                          "for (var i = 0; i < childProperties.length; i++) {" +
                          "for (var name1 in childProperties[i]) {" +
                          "	if (name1 != undefined) {" +
                          "model[childProperties[i][name1].Key].SetUpAllServerErrors(childProperties[i][name1].Data);" +
                          "}" +
                          "}}}};");

            foreach (var prop in type.GetProperties())
            {
                if (!generateChildModels && (!IsPrimitiveType(prop.PropertyType) || IsCollection(prop.PropertyType)))
                {
                    continue;
                }

                if (generateChildModels && !IsPrimitiveType(prop.PropertyType) && !IsCollection(prop.PropertyType))
                {
                    result.AppendFormat("section.{1}.apply(model['{0}']);", prop.Name, prop.Name + "Base");
                    result.AppendFormat("model.ValidatingChildren.push(model['{0}']);", prop.Name);

                    continue;
                }

                result.Append(BuildAttributeBasedValidation(prop.GetCustomAttributes(typeof(ValidationAttribute), true).Cast<ValidationAttribute>().ToList(), prop.Name));
                result.Append(ExstendForServerValidation(prop.Name));
                result.Append(SetUpDisplayPropertyName(prop, prop.Name));
                result.Append(SetUpElementIdAndName(prop, prop.Name, idPrefixs));
            }

            result.Append("model.GoToField = function (data) {" +
                          "var elementId = '#' + data.Id;" +
                          "var navbar = $('.navbar');var navbarheight = 10;" +
                          "if (navbar.length > 0)navbarheight += navbar.height();" +
                          "$('html, body').animate({scrollTop: $(elementId).offset().top - navbarheight}, 500);" +
                          "setTimeout(function () { $('#' + data.Id).focus(); }, 500);};");

            result.Append("model.IsValid = function () {var result = true;" +
                          "for (var name in model) {" +
                          "if (model[name]['__valid__'] != undefined) {" +
                          "model[name].notifySubscribers('');" +
                          "if (!model[name].isValid()) {" +
                          "result = false;}" +
                          "}" +
                          "if (model.ValidatingChildren != undefined && model.ValidatingChildren.length > 0) {for (var i = 0; i < model.ValidatingChildren.length; i++) {" +
                          "     var child = model.ValidatingChildren[i];" +
                          "     if (!child.IsValid()) {	result = false;}}}" +
                          "}" +
                          "return result;};");

            BuildChildPropertiesValidationKeysSet(result);

            result = FormatString(result);

            return result.ToString();
        }

        private void BuildChildPropertiesValidationKeysSet(StringBuilder builder)
        {
            builder.Append("var splitPropertyAndChildModelsKeys = function (keys) {" +
                           "var result = [];var arr = [];" +
                           "for (var key in keys) {" +
                           "if (keys.hasOwnProperty(key)) {arr.push(key);" +
                           "}};var childPropertiesKeys = arr.filter(" +
                           "function (el) {" +
                           "return el.indexOf('.') > -1;});var keyDictionary = [];for (var i = 0; i < childPropertiesKeys.length; i++) {" +
                           "var splittedKeys = childPropertiesKeys[i].split('.');" +
                           "var childPropertyKey = splittedKeys.shift();" +
                           "var path = splittedKeys.join('.');" +
                           "var kv = {};if (keyDictionary[childPropertyKey] == undefined) {" +
                           "keyDictionary[childPropertyKey] = { Key: childPropertyKey, Data: {} };" +
                           "}" +
                           "keyDictionary[childPropertyKey].Data[path] = keys[childPropertyKey + '.' + path];" +
                           "}" +
                           "result.push(keyDictionary);return result;};");

        }

        private  string CreateValidatablePropertiesArray(Type type)
        {
            var result = new StringBuilder();
            result.Append("model.ValidatedProperties = ko.observableArray();");

            foreach (var prop in type.GetProperties())
            {
                if (!IsPrimitiveType(prop.PropertyType) || IsCollection(prop.PropertyType))
                {
                    continue;
                }

                if (prop.GetCustomAttributes(typeof(ValidationAttribute), true).Any())
                {
                    result.AppendFormat("model.ValidatedProperties.push(model['{0}']);", prop.Name);
                }
            }

            result.Append("model.ShowSummaryHints=ko.observable(false);");

            result.Append("model.ControlsWithErrors = ko.computed(function () {var filtered = ko.utils.arrayFilter(model.ValidatedProperties(), function (item) {" +
                          "return item.isValid() != true;});" +
                          "var result= ko.utils.arrayMap(filtered, function (item) {	return { DisplayName: item.DisplayName, Id: item.ElementId };});" +
                          "if (model.ValidatingChildren.length > 0) {" +
                          "for (var i = 0; i < model.ValidatingChildren.length; i++) {" +
                          "var child = model.ValidatingChildren[i];" +
                          "ko.utils.arrayPushAll(result,child.ControlsWithErrors());	}	}" +
                          "return result;}, model);"
                );

            result = FormatString(result);

            return result.ToString();
        }

        private  string SetUpDisplayPropertiesNames(Type type)
        {
            var result = new StringBuilder();
            foreach (var prop in type.GetProperties())
            {
                if (!IsPrimitiveType(prop.PropertyType) || IsCollection(prop.PropertyType))
                {
                    continue;
                }
                result.Append(SetUpDisplayPropertyName(prop, prop.Name));
            }
            result = FormatString(result);

            return result.ToString();
        }

        private  string SetUpDisplayPropertyName(PropertyInfo property, string propertyName)
        {
            var result = new StringBuilder();
            var displayNameAttribute = property.GetCustomAttributes(typeof(DisplayAttribute), false).Cast<DisplayAttribute>().FirstOrDefault();

            result.AppendFormat("model['{0}'].DisplayName=ko.observable('{1}');", propertyName, displayNameAttribute != null ? displayNameAttribute.LocalizableName() : propertyName);

            result = FormatString(result);

            return result.ToString();
        }

        private  string SetUpElementIdAndName(PropertyInfo property, string propertyName, string idPrefixs)
        {
            var result = new StringBuilder();

            result.AppendFormat("model['{0}'].ElementName='{1}';", propertyName, string.IsNullOrEmpty(idPrefixs) ? propertyName : string.Format("{0}.{1}", idPrefixs, propertyName));
            result.AppendFormat("model['{0}'].ElementId='{1}';", propertyName, string.IsNullOrEmpty(idPrefixs) ? propertyName : string.Format("{0}_{1}", idPrefixs, propertyName));

            result = FormatString(result);

            return result.ToString();
        }

        private  string ExstendForServerValidation(string propertyName)
        {
            var result = new StringBuilder();

            result.Append(string.Format("model['{0}'].HasSercverError = false;", propertyName));
            result.Append(string.Format("model['{0}'].ServerMessage = '';", propertyName));

            result.Append("model['" + propertyName +
                           "'].SetUpServerError = function(errorMsq, key) " +
                           "{var ruleName = model[key].rules()[model[key].rules().length - 1];" +
                           "ruleName.message = errorMsq;" +
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

            result = FormatString(result);

            return result.ToString();
        }

        private  string BuildAttributeBasedValidation(List<ValidationAttribute> attributes, string propertyName)
        {
            var result = new StringBuilder();

            foreach (var attribute in attributes)
            {
                #region Custom attributes(will be removed from here,  custom attributes will be applied with registration on app start.)
                //generation for custom registered validation attributes
                //TODO(Alex) : serviceLocator to be used here
                if (CustomAttrValidationRegistrator.IsRegistered(attribute))
                {
                    result.Append(CustomAttrValidationRegistrator.GenerateRuleIfRegistered(attribute, propertyName));

                    continue;
                }

                #endregion

                if (attribute is StringLengthAttribute)
                {
                    result.Append(BuildValidationWithLengthAttribute(attribute as StringLengthAttribute, propertyName));
                    continue;
                }

                if (attribute is PhoneAttribute)
                {
                    //result.Append(BuildValidationWithPhoneAttribute(attribute as PhoneAttribute, propertyName));
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
            result = FormatString(result);

            return result.ToString();
        }

        private string BuildValidationWithRequiredAttribute(RequiredAttribute requiredAttribute, string propertyName)
        {
            return
                string.Format(
                    @"model['{0}'].extend({{ required: {{ message: '{1}' }} }});", propertyName, requiredAttribute.LocalizableError());
        }

        private string BuildValidationWithPhoneAttribute(PhoneAttribute phoneAttribute, string propertyName)
        {

            Type type = typeof(PhoneAttribute);
            FieldInfo info = type.GetField("_regex", BindingFlags.NonPublic | BindingFlags.Static);
            var value = info.GetValue(null);

            return
                string.Format(
                    @"model['{0}'].extend({{
                         pattern: {{
                                    message: '{2}',
                                    params: /{1}/
                                    }} }});", propertyName, @"^((8|\+7)-?)?\(?\d{3}\)?-?\d{1}-?\d{1}-?\d{1}-?\d{1}-?\d{1}-?\d{1}-?\d{1}?$", phoneAttribute.LocalizableError());
        }

        private string BuildValidationWithMinLengthAttribute(MinLengthAttribute minAttribute, string propertyName)
        {
            return
                string.Format(
                    @"model['{0}'].extend({{ minLength: {{ params: {1},
                                                           message: '{2}' }} }});", propertyName, minAttribute.Length, minAttribute.LocalizableError());
        }

        private string BuildValidationWithLengthAttribute(StringLengthAttribute lengthAttribute, string propertyName)
        {
            var result = new StringBuilder();

            result.AppendFormat(
                    @"model['{0}'].extend({{ maxLength: {{ params: {1},
                                                           message: '{2}' }} }});", propertyName, lengthAttribute.MaximumLength, lengthAttribute.LocalizableError());

            result.AppendFormat(
                   @"model['{0}'].extend({{ minLength: {{ params: {1},
                                                           message: '{2}' }} }});", propertyName, lengthAttribute.MinimumLength, lengthAttribute.LocalizableError());

            return result.ToString();
        }

        private string BuildValidationWithMaxLengthAttribute(MaxLengthAttribute maxAttribute, string propertyName)
        {
            return
                string.Format(
                    @"model['{0}'].extend({{ maxLength: {{ params: {1},
                                                           message: '{2}' }} }});", propertyName, maxAttribute.Length, maxAttribute.ErrorMessage);
        }

        private string BuildValidationWithEmailLengthAttribute(EmailAddressAttribute emailValidationAttribute, string propertyName)
        {
            //(?i)
            Type type = typeof(EmailAddressAttribute);
            FieldInfo info = type.GetField("_regex", BindingFlags.NonPublic | BindingFlags.Static);
            var value = info.GetValue(null).ToString();
           // value = value+"i"

            return
                string.Format(
                    @"model['{0}'].extend({{
                         pattern: {{
                                    message: '{2}',
                                    params: /{1}/i
                                    }} }});", propertyName, value, emailValidationAttribute.LocalizableError());
        }

        private string BuildValidationWithMaxLengthAttribute(RegularExpressionAttribute regexpAtribute, string propertyName)
        {
            return
                string.Format(
                    @"model['{0}'].extend({{
                         pattern: {{
                                    message: '{2}',
                                    params: /{1}/
                                    }} }});", propertyName, regexpAtribute.Pattern, regexpAtribute.LocalizableError());
        }

        private StringBuilder FormatString(StringBuilder builder)
        {
            return builder.Replace(";", string.Format(";{0}", Environment.NewLine));
        }

        private bool IsCollection(Type fieldType)
        {
            return fieldType.IsArray
                || fieldType.GetInterfaces().Any(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IList<>));
        }

        private  bool IsPrimitiveType(Type fieldType)
        {
            return (fieldType.IsPrimitive || fieldType.Namespace.Equals("System"));
        }

        #endregion Helpers
    }
}