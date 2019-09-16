using RICONS.Logger;
using Nop.Admin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace RICONS.Web.Controllers
{
    public class DesignController : BaseController
    {
        #region Fields
        Log4Net _logger = new Log4Net(typeof(DesignController));
        #endregion

        public ActionResult Index()
        {
            if (!IsLogged())
                return BackToLogin();
            return View();
        }
	}
}