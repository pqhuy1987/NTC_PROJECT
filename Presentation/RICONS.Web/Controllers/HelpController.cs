using Nop.Admin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RICONS.Web.Controllers
{
    public class HelpController : BaseController
    {
        //
        // GET: /Help/
        public ActionResult Index()
        {
            return View();
        }
	}
}