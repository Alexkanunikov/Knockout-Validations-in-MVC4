using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Mvc4KnockoutCRUD.Models
{
    public class PhoneViewModel
    {
        [Required(ErrorMessage = "Phone is required")]
        public string Phone { get; set; }
        public string Description { get; set; }
    }
}