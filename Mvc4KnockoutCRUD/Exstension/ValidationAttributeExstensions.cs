using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Mvc4KnockoutCRUD.Helpers;

namespace Mvc4KnockoutCRUD.Exstension
{
    public static class ValidationAttributeExstensions
    {
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