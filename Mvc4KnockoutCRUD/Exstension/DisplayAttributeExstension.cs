using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Mvc4KnockoutCRUD.Helpers;

namespace Mvc4KnockoutCRUD.Exstension
{
    public static class DisplayAttributeExstension
    {
        public static string LocalizableName(this DisplayAttribute attribute)
        {
            if (attribute.ResourceType != null)
            {
                return ResourceManagerHelper.GetDynamicResource(attribute.Name, attribute.ResourceType);
            }
            return attribute.Name;
        }
    }
}