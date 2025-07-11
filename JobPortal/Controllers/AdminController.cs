using JobPortal.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JobPortal.Controllers
{
    public class AdminController : Controller
    {
        JobApplicationPortalEntities dbObj = new JobApplicationPortalEntities();
        
      
        public ActionResult Index()
        {
            return View();
        }
    }
}