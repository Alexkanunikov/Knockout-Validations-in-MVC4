using System;
using System.Web;
using System.Web.Mvc;

namespace Knockout.Validation.Exstensions
{
    public static class HtmlHelperExstension
    {
        private static readonly KnockoutValidationOptions _defaultSettings;

        #region Static constructor

        static HtmlHelperExstension()
        {
            _defaultSettings =new KnockoutValidationOptions()
            {
                DecarateInput = true,
                DecorateInputElement = true,
                InsertMessages = true,
                ErrorElementClass = "error",
                MessagesOnModified = true,
                RegisterExtenders = true
            };
        }
        #endregion

        /// <summary>
        /// Builds base Ko view model with client side validation rules based on data-annotation attributes
        /// </summary>
        /// <typeparam name="TModelMain"></typeparam>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="modelGenerateTo"></param>
        /// <param name="modelName"></param>
        /// <param name="namespaceName"></param>
        /// <param name="geneteChildModels"></param>
        /// <param name="options"></param>
        /// <param name="idPrefixs"></param>
        /// <returns></returns>
        public static HtmlString BuidKoBaseModel<TModelMain, TModel>(this HtmlHelper<TModelMain> htmlHelper, Func<TModelMain, TModel> modelGenerateTo,
            string modelName, string namespaceName, bool geneteChildModels = false, KnockoutValidationOptions options = null, string idPrefixs = "")
        {            

            TModel model = modelGenerateTo(htmlHelper.ViewData.Model);
            options = options ?? _defaultSettings;

            return new HtmlString(new KnockoutValidationGenerator().GenerateValidationModel(model.GetType(), modelName, namespaceName, options, geneteChildModels, idPrefixs));
        }

        /// <summary>
        /// Builds base Ko view model by type with client side validation rules based on data-annotation attributes
        /// </summary>
        /// <typeparam name="TModelMain"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="type"></param>
        /// <param name="modelName"></param>
        /// <param name="namespaceName"></param>
        /// <param name="options"></param>
        /// <param name="geneteChildModels"></param>
        /// <param name="idPrefixs"></param>
        /// <returns></returns>
        public static HtmlString BuidKoBaseModelByType<TModelMain>(this HtmlHelper<TModelMain> htmlHelper,
            Type type, string modelName, string namespaceName, KnockoutValidationOptions options = null, bool geneteChildModels = false, string idPrefixs = "")
        {
            options = options ?? _defaultSettings;

            return new HtmlString(new KnockoutValidationGenerator().GenerateValidationModel(type, modelName, namespaceName, options, geneteChildModels, idPrefixs));
        }
    }
}