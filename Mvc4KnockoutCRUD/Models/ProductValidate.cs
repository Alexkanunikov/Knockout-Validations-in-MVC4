using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Knockout.Validation.Attributes;

namespace Mvc4KnockoutCRUD.Models
{
    public class ProductValidate
    {
        #region properties
        [Required(ErrorMessage = "Name is required")]
        [Display(Name="Key1", ResourceType = typeof(ResourceFile))]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [MinLength(4, ErrorMessage = "min length is 4")]
        [MaxLength(10, ErrorMessage = "max length is 10")]
        [Display(Name = "описание ееее")]
        public string Description { get; set; }

        [EmailAddress(ErrorMessage = "wrong email address")]
        public string Email { get; set; }

        //[EmailAddress(ErrorMessage = "неправильный адрес")]
        [RegularExpression(@"^(0|[1-9][0-9]*)$", ErrorMessage = "Error")]
        public string Number10 { get; set; }
        [Required(ErrorMessage = "Поле является обязательным")]
        public string ServerValidationField { get; set; }

        public int SelectedItem { get; set; }
        [Display(Name = "описание 11111")]
        [ReguiredCondition("SelectedItem", 1, ErrorMessage = "Поле является обязательным")]
        public string Desc1 { get; set; }

        public List<KeyValuePair<string, int>> Options { get; set; } 
        #endregion
    }
}