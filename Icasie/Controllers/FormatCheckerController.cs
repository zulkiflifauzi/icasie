using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Icasie.Controllers
{
    [Authorize(Roles = "Format Checker, Administrator")]
    public class FormatCheckerController : Controller
    {
        //
        // GET: /FormatChecker/
        public ActionResult Index()
        {
            return View();
        }
	}
}