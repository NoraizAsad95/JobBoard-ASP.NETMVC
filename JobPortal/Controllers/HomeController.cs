using JobPortal.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;

namespace JobPortal.Controllers
{
    public class HomeController : Controller
    {
        JobApplicationPortalEntities dbObj = new JobApplicationPortalEntities();

        public ActionResult SignUp()
        {
            return View();
        }

        [HttpPost]

        public ActionResult AddSignUp(User Model, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                var exist = dbObj.Users.FirstOrDefault(u => u.user_email == Model.user_email);
                if(exist != null)
                {
                    ViewBag.ErrorMessage = "Email already exists.";
                    return View("SignUp", Model);
                }
                bool isEmployer = form["IsEmployer"] == "true";
                User obj = new User
                {
                    user_fullname = Model.user_fullname,
                    user_email = Model.user_email,
                    user_password = Model.user_password,
                    IsEmployer = isEmployer,
                    CreatedAt = DateTime.Now,
                };

                dbObj.Users.Add(obj);
                dbObj.SaveChanges();      
            }

            return View("SignUp", Model);
        
        }

        public ActionResult SignIn()
        {
            return View();
        }

        [HttpPost]

        public ActionResult AddSignIn(User Model)
        {
            User obj = dbObj.Users.Where(x => x.user_email == Model.user_email && x.user_password == Model.user_password).SingleOrDefault();
            if (obj != null)
            {
                Session["UserId"] = obj.UserId.ToString();
                Session["FullName"] = obj.user_fullname;
                Session["IsEmployer"] = obj.IsEmployer; // Important for role-based checks

                TempData["SuccessMessage"] = "Login successful.";

                // Redirect based on role
                if ((bool)obj.IsEmployer)
                {
                    return RedirectToAction("EmployerDashboard", "Job");
                }
                else
                {
                    return RedirectToAction("BrowseJobs", "Job");
                }
            }
            else
            {
                ViewBag.Error = "Invalid email or password.";
            }
        

            return View("SignIn");
        }

      
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}