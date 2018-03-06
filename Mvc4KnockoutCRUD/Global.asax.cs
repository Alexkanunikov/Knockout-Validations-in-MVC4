using System;
using System.Collections.Generic;

using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Knockout.Validation.Knockout;

namespace Mvc4KnockoutCRUD
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //register custom ko rule generators
            RegisterValidationRuleGenerator();
        }

        private void RegisterValidationRuleGenerator()
        {
            //TODO: make this type non static and Ioc with singelton lifetime will be used instead
            CustomAttrValidationRegistrator.Register(new RequiredConditionRuleGenerator());
            CustomAttrValidationRegistrator.Register(new StringLengthConditionRuleGenerator());
            CustomAttrValidationRegistrator.Register(new EmailOrRFPhoneRuleGenerator());
        }
    }
}