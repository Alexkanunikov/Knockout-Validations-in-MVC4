using System.ComponentModel.DataAnnotations;

namespace Knockout.Validation.Exstensions
{
    public static class DisplayAttributeExtension
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