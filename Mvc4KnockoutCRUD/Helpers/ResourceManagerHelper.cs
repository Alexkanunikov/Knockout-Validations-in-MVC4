using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Web;

namespace Mvc4KnockoutCRUD.Helpers
{
    public static class ResourceManagerHelper
    {
        public static string GetDynamicResource(string key, Type resourceType)
        {
            ResourceManager rm = new ResourceManager(resourceType);

            return rm.GetString(key);
        }
    }
}