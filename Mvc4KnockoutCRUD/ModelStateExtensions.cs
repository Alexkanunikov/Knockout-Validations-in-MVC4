using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Mvc4KnockoutCRUD
{
    /// <summary>
    /// Extension methods for ModelStateDictionary type
    /// </summary>
    public static class ModelStateExtensions
    {
        /// <summary>
        /// Returns model errors
        /// </summary>
        /// <param name="modelState"></param>
        /// <returns></returns>
        public static IDictionary<string, string> AllErrors(this ModelStateDictionary modelState)
        {
            return
                (from ms in modelState
                 where ms.Value.Errors.Any()
                 let key = ms.Key
                 let errors = ms.Value.Errors
                 from error in errors
                 select new KeyValuePair<string, string>(key, error.ErrorMessage)).ToDictionary(x => x.Key, x => x.Value);
        }
    }
}