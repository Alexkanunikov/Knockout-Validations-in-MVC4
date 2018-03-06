using System.Resources;
using Mvc4KnockoutCRUD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mvc4KnockoutCRUD.Controllers
{
    public class ProductController : Controller
    {

        public ActionResult Product()
        {

            return View(new ProductValidate(){Name = "nameeee", Description = "descr", SelectedItem = 0, Options = new List<KeyValuePair<string, int>>()
            {
                new KeyValuePair<string, int>("option0",0),
                new KeyValuePair<string, int>("option1",1),
                new KeyValuePair<string, int>("option2",2)
            }});
        }

        [HttpPost]
        public JsonResult Product(ProductValidate model)
        {

            if (model.ServerValidationField == "qwerty")
            {
                ModelState.AddModelError("ServerValidationField", @"Невалидный номер телефона");
                return Json(new JsonResponse(JsonResponseState.Error, ModelState.AllErrors()));
            }

            return Json(new JsonResponse { State = JsonResponseState.Ok, Data = null });
        }


        #region several models validation
        public ActionResult MultiValidate()
        {
            return View(new RootViewModel()
            {
                PersonViewModel = new PersonViewModel(){FirstName = "First name", SecondName = "Second name", ServerValidationField = "bla",Phones = new List<PhoneViewModel>()
                {
                    new PhoneViewModel(){Phone = "1234234", Description = "Main"}
                },
                NewPhoneViewModel = new PhoneViewModel(){Phone = "123333", Description = "desc"}},
                ProductValidateViewModel = new ProductValidate() { Name = "nameeee 1", Description = "descr 1" }
                
            });
        }

        [HttpPost]
        public JsonResult PersonSave(PersonViewModel model)
        {
            if (model.ServerValidationField == "123")
            {
                ModelState.AddModelError("ServerValidationField", @"Такое значение уже используется");
            }

            if (model.Description == "123")
            {
                ModelState.AddModelError("Description", @"Такое Description значение уже используется");
            }

            if (!ModelState.IsValid)
            {
                return Json(new JsonResponse(JsonResponseState.Error, ModelState.AllErrors()));
            }

            return Json(new JsonResponse { State = JsonResponseState.Ok, Data = null });
        } 
        #endregion

    }
}
