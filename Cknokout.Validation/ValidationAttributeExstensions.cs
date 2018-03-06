using System.ComponentModel.DataAnnotations;

namespace Knockout.Validation
{
    public static class ValidationAttributeExstensions
    {
        /// <summary> 
        /// Returns localizible error descriptition from resources
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static string LocalizableError(this ValidationAttribute attribute)
        {
            if (attribute.ErrorMessageResourceType != null)
            {
                return ResourceManagerHelper.GetDynamicResource(attribute.ErrorMessageResourceName, attribute.ErrorMessageResourceType);
            }
            return attribute.ErrorMessage;
        }
    }
}