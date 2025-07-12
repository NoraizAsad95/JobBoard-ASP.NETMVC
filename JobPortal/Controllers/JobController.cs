using JobPortal.Context;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JobPortal.Controllers
{
    public class JobController : Controller
    {
        JobApplicationPortalEntities dbObj = new JobApplicationPortalEntities();
        private bool? isReported;

        public ActionResult BrowseJobs(int? id, int page = 1, int pageSize = 5)
        {
            var jobsQuery = dbObj.JobPosts
                .OrderByDescending(j => j.CreatedAt);

            int totalJobs = jobsQuery.Count();
            var paginatedJobs = jobsQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            JobPost selectedJob = null;
            if (id != null)
            {
                selectedJob = dbObj.JobPosts.Include("User").FirstOrDefault(j => j.JobId == id);
            }

            ViewBag.SelectedJob = selectedJob;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalJobs / pageSize);
            ViewBag.CurrentPage = page;

            return View(paginatedJobs);
        }


        public ActionResult EmployerDashboard()
        {
            if (Session["UserId"] == null || !(bool)Session["IsEmployer"])
            {
                return RedirectToAction("SignIn", "Home");
            }

            int currentEmployerId = int.Parse(Session["UserId"].ToString());

            var jobs = dbObj.JobPosts
                            .Where(j => j.PostedByUserId == currentEmployerId)
                            .ToList();

            return View(jobs);
        }

        public ActionResult PostJob(int? jobId)
        {
            if (Session["UserId"] == null || !(bool)Session["IsEmployer"])
            {
                return RedirectToAction("SignIn", "User");
            }

            if (jobId.HasValue)
            {
                var job = dbObj.JobPosts.Find(jobId.Value);
                if (job != null && job.PostedByUserId == int.Parse(Session["UserId"].ToString()))
                {
                    return View(job); // Return loaded job
                }
                else
                {
                    return RedirectToAction("EmployerDashboard");
                }
            }

            return View(new JobPost()); // Empty for new post
        }

        [HttpPost]
        public ActionResult CreatePostJob(JobPost Model, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                if (Model.JobId == 0)
                {
                    // Creating a new job post
                    JobPost obj = new JobPost
                    {
                        Title = Model.Title,
                        Job_description = Model.Job_description,
                        Requirements = Model.Requirements,
                        PostedByUserId = int.Parse(Session["UserId"].ToString()),
                        IsReported = false,
                        CreatedAt = DateTime.Now
                    };

                    dbObj.JobPosts.Add(obj);
                    dbObj.SaveChanges();
                }
                else
                {
                    // Editing an existing job post
                    var existing = dbObj.JobPosts.Find(Model.JobId);
                    if (existing != null && existing.PostedByUserId == int.Parse(Session["UserId"].ToString()))
                    {
                        existing.Title = Model.Title;
                        existing.Job_description = Model.Job_description;
                        existing.Requirements = Model.Requirements;
                        // You can update CreatedAt only if you want to reset the date (optional)
                        dbObj.SaveChanges();
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Job not found or unauthorized.";
                        return RedirectToAction("EmployerDashboard");
                    }
                }

                TempData["SuccessMessage"] = "Job posted successfully!";
                return RedirectToAction("EmployerDashboard", "Job");
            }

            return View("PostJob", Model);
        }

        public ActionResult DeleteJob(int id)
        {
            var res = dbObj.JobPosts.Where(data => data.JobId == id).First();
            dbObj.JobPosts.Remove(res);
            dbObj.SaveChanges();
            var list = dbObj.JobPosts.ToList();

            return View("EmployerDashboard", list);
        }

        public ActionResult NoOfApplicants(int id)
        {
            var applications = dbObj.Applications
                .Where(a => a.JobId == id)
                .OrderByDescending(a => a.AppliedAt)
                .ToList();

            return View(applications);
        }

        public ActionResult Applied(int jobId)
        {
            if(Session["UserId"] == null)
        {
                return RedirectToAction("SignIn", "Home");
            }

            ViewBag.JobId = jobId;
            return View(); 
        }
        [HttpPost]

        public ActionResult AppliedFor(Application model, HttpPostedFileBase ResumeFile)
        {
            
            if (ModelState.IsValid)
            {
                // Save file to /Uploads/
                string filePath = "";
                if (ResumeFile != null && ResumeFile.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(ResumeFile.FileName);
                    string path = Path.Combine(Server.MapPath("~/Uploads"), fileName);
                    ResumeFile.SaveAs(path);
                    filePath = "/Uploads/" + fileName;
                }

                model.ResumePath = filePath;
                model.AppliedAt = DateTime.Now;
                model.UserId = int.Parse(Session["UserId"].ToString());

                dbObj.Applications.Add(model);
                dbObj.SaveChanges();

                TempData["Success"] = "Application submitted successfully!";
                return RedirectToAction("BrowseJobs", "Job");
            }

            ViewBag.JobId = model.JobId;
            return View(model);
        }
        public ActionResult Signout()
        {
            
            Session.RemoveAll();
            Session.Abandon();
            return RedirectToAction("SignIn", "Home");
        }

        public ActionResult SaveJob(int jobId)
        {
            if (Session["UserId"] == null )
            {
                return RedirectToAction("SignIn", "Home");
            }
            int userId = int.Parse(Session["UserId"].ToString ()) ;
            bool alreadySaved = dbObj.SavedJobs.Any(data => data.UserId == userId && data.JobId == jobId);
            if(!alreadySaved)
            {
                var saved = new SavedJob
                {
                    UserId = userId,
                    JobId = jobId,
                    SavedAt = DateTime.Now,
                };
                dbObj.SavedJobs.Add(saved);
                dbObj.SaveChanges();
                TempData["SuccessMessage"] = "Job saved successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "You have already saved this job.";
            }
            return RedirectToAction("BrowseJobs");
        }

        public ActionResult MySavedJobs()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("SignIn", "Home");

            int userId = int.Parse(Session["UserId"].ToString());

            var savedJobs = dbObj.SavedJobs
                            .Where(s => s.UserId == userId)
                            .Select(s => s.JobPost) // Assuming you have navigation property
                            .ToList();

            return View(savedJobs);
        }
        public ActionResult SearchJobs(string keyword)
        {
            var jobs = dbObj.JobPosts.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                jobs = jobs.Where(j =>
                    j.Title.Contains(keyword) ||
                    j.Job_description.Contains(keyword) ||
                    j.Requirements.Contains(keyword));
            }

            ViewBag.SearchTerm = keyword;
            return View("BrowseJobs", jobs.ToList()); // reuse your BrowseJobs view
        }

        public ActionResult Index()
        {
            return View();
        }
    }
}