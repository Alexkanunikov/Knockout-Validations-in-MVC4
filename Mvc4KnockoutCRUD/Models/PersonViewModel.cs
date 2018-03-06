using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using Knockout.Validation.Attributes;

namespace Mvc4KnockoutCRUD.Models
{
    public class PersonViewModel
    {
        #region properties
        [Required(ErrorMessage = "First name is required")]
        [Display(Name = "Key1",ResourceType = typeof(ResourceFile))]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Second name is required")]  
        public string SecondName { get; set; }

        public string ServerValidationField { get; set; }

        public bool RequireDescription { get; set; }

        [ReguiredCondition("RequireDescription", true, ErrorMessage = "Поле является обязательным")]
        public string Description { get; set; }

        public List<PhoneViewModel> Phones { get; set; }

        public PhoneViewModel NewPhoneViewModel { get; set; }


        #endregion
    }
}