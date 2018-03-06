using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Knockout.Validation.Attributes
{
    /// <summary>
    /// Email or Phone valiation attribute
    /// </summary>
    public class EmailOrRFPhoneAttribute : RegularExpressionAttribute
    {
        #region Properties
        public string EmailErrorMessage { get; private set; }
        public string PhoneErroreMessage { get { return ErrorMessage; } }

        public string PhonePattern { get { return base.Pattern; } }
        public string EmailPattern { get { return @"^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$"; } }
        #endregion

        #region Constructors

        public EmailOrRFPhoneAttribute(string emailErrorMessage, string phoneErroreMessage)
            : base(@"^((8|\+7)-?)?\(?\d{3}\)?-?\d{1}-?\d{1}-?\d{1}-?\d{1}-?\d{1}-?\d{1}-?\d{1}?$")
        {
            EmailErrorMessage = emailErrorMessage;
            ErrorMessage = phoneErroreMessage;
        }
        #endregion

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value ==null || String.IsNullOrEmpty(value.ToString()))
            {
                return ValidationResult.Success;
            }

            bool isEmailValidation = value.ToString().Contains("@");

            return !isEmailValidation
                ? base.IsValid(value, validationContext)
                : (new Regex(this.EmailPattern).Match(value.ToString().ToLower())).Success ? ValidationResult.Success : new ValidationResult(EmailErrorMessage);
        }
    }
}