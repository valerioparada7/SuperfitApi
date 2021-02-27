using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SuperfitApi.Controllers
{
    public class LoginWebController : Controller
    {
        public LoginWebController()
        {

        }
        // GET: LoginWeb
        public ActionResult LoginWeb()
        {
            return View();
        }
    }
}