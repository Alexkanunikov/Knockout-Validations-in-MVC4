using System;
using System.Resources;

namespace Knockout.Validation
{
    /// <summary>
    /// Handles resouces
    /// </summary>
    public static class ResourceManagerHelper
    {
        /// <summary>
        /// Returns resource by 'Key' and 'resourceType'
        /// </summary>
        /// <param name="key"></param>
        /// <param name="resourceType"></param>
        /// <returns></returns>
        public static string GetDynamicResource(string key, Type resourceType)
        {
            var rm = new ResourceManager(resourceType);

            return rm.GetString(key);
        }
    }
}