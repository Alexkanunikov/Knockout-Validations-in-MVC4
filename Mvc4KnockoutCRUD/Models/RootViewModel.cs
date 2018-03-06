using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mvc4KnockoutCRUD.Models
{
    public class RootViewModel
    {
        #region Properties
        public ProductValidate ProductValidateViewModel { get; set; }
        public PersonViewModel PersonViewModel { get; set; }
        #endregion
    }
}