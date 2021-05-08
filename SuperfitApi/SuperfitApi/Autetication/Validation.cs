using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SuperfitApi.Autetication
{
    public class Validation:ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var sesion = HttpContext.Current.Session["sesion"];

            if (sesion == null)
            {
                filterContext.Result = new RedirectResult("~/LoginWeb/LoginWeb");
            }
            base.OnActionExecuting(filterContext);
        }        
    }
}