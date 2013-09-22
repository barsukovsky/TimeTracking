using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Data.Entity;
using WebMatrix.WebData;
using TimeTracking2.Filters;
using TimeTracking2.Models;

namespace TimeTracking2.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class ReportController : Controller
    {
        private EFDbContext context = new EFDbContext();

        public ActionResult Index()
        {
            if (!User.IsInRole("Administrator"))
            {
                return RedirectToAction("FetchByUser", new
                {
                    username = User.Identity.Name
                });
            }

            var reports = context.Reports.
                Include(x => x.UserProfile).
                OrderByDescending(x => x.Year).
                ThenByDescending(x => x.Month);

            return View(reports);
        }

        public ActionResult FetchByUser(string userName)
        {
            if (User.Identity.Name != userName && !User.IsInRole("Administrator"))
            {
                return new HttpNotFoundResult();
            }

            int userId = WebSecurity.GetUserId(userName);
            if (userId == -1)
            {
                return new HttpNotFoundResult();
            }

            return View(new ReportsOfUserViewModel
            {
                Reports = context.Reports.
                    Where(x => x.UserId == userId).
                    OrderByDescending(x => x.Year).
                    ThenByDescending(x => x.Month),
                UserProfile = context.UserProfiles.Find(userId)
            });
        }

        public ActionResult FetchByYear(int year)
        {
            if (!User.IsInRole("Administrator"))
            {
                return RedirectToAction("FetchByYearAndUser", new
                {
                    username = User.Identity.Name,
                    year = year
                });
            }

            var reports = context.Reports.
                Where(x => x.Year == year).
                Include(x => x.UserProfile).
                OrderByDescending(x => x.Month);

            return View("Index", reports);
        }

        public ActionResult FetchByMonth(int month)
        {
            if (!User.IsInRole("Administrator"))
            {
                return RedirectToAction("FetchByMonthAndUser", new
                {
                    username = User.Identity.Name,
                    month = month
                });
            }

            var reports = context.Reports.
                Where(x => x.Month == month).
                Include(x => x.UserProfile).
                OrderByDescending(x => x.Year);

            return View("Index", reports);
        }

        public ActionResult FetchByYearAndUser(int year, string userName)
        {
            if (User.Identity.Name != userName && !User.IsInRole("Administrator"))
            {
                return new HttpNotFoundResult();
            }

            int userId = WebSecurity.GetUserId(userName);
            if (userId == -1)
            {
                return new HttpNotFoundResult();
            }

            return View("FetchByUser", new ReportsOfUserViewModel
            {
                Reports = context.Reports.
                    Where(x => x.Year == year && x.UserId == userId).
                    OrderByDescending(x => x.Month),
                UserProfile = context.UserProfiles.Find(userId)
            });
        }

        public ActionResult FetchByMonthAndUser(int month, string userName)
        {
            if (User.Identity.Name != userName && !User.IsInRole("Administrator"))
            {
                return new HttpNotFoundResult();
            }

            int userId = WebSecurity.GetUserId(userName);
            if (userId == -1)
            {
                return new HttpNotFoundResult();
            }

            return View("FetchByUser", new ReportsOfUserViewModel
            {
                Reports = context.Reports.
                    Where(x => x.Month == month && x.UserId == userId).
                    OrderByDescending(x => x.Year),
                UserProfile = context.UserProfiles.Find(userId)
            });
        }

        public ActionResult FetchByYearAndMonth(int year, int month)
        {
            if (!User.IsInRole("Administrator"))
            {
                return RedirectToAction("Edit", new {
                    username = User.Identity.Name,
                    year = year,
                    month = month
                });
            }

            var reports = context.Reports.
                Where(x => x.Year == year && x.Month == month).
                Include(x => x.UserProfile);

            return View("Index", reports);
        }

        public ActionResult Create()
        {
            return RedirectToAction("CreateForUser", new
            {
                username = User.Identity.Name
            });
        }

        public ActionResult CreateForUser(string userName)
        {
            if (User.Identity.Name != userName && !User.IsInRole("Administrator"))
            {
                return new HttpNotFoundResult();
            }
            
            int userId = WebSecurity.GetUserId(userName);
            if (userId == -1)
            {
                return new HttpNotFoundResult();
            }
            var user = context.UserProfiles.Find(userId);

            return View("Edit", new Report
            {
                Year = DateTime.Now.Year,
                Month = DateTime.Now.Month,
                UserId = userId,
                HourlyRate = (user.CurrentHourlyRate != null) ? user.CurrentHourlyRate.Value : 0,
                UserProfile = user
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateForUser(Report model)
        {
            if (!ModelState.IsValid)
            {
                return View("Edit", model);
            }

            model.UserProfile = context.UserProfiles.Find(model.UserId);

            if (model.UserProfile == null)
            {
                return new HttpNotFoundResult();
            }

            if (User.Identity.Name != model.UserProfile.UserName && !User.IsInRole("Administrator"))
            {
                return new HttpNotFoundResult();
            }

            if (model.UserProfile.CurrentHourlyRate == null || model.UserProfile.CurrentHourlyRate.Value == 0)
            {
                ModelState.AddModelError("", "Сотрудник сейчас не нанят; свяжитесь с администратором");
                return View("Edit", model);
            }

            var report = context.Reports.Find(model.UserId, model.Year, model.Month);
            if (report != null)
            {
                ModelState.AddModelError("", "Отчет за " +
                    new DateTime(model.Year, model.Month, 1).ToString("MMMM").ToLower() + " " +
                    model.Year.ToString() + " года уже существует");
                return View("Edit", model);
            }

            context.Reports.Add(report = new Report
            {
                Year = model.Year,
                Month = model.Month,
                UserId = model.UserId,
                Hours = model.Hours,
                HourlyRate = model.UserProfile.CurrentHourlyRate.Value
            });
            context.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult Edit(int year, int month, string userName)
        {
            if (User.Identity.Name != userName && !User.IsInRole("Administrator"))
            {
                return new HttpNotFoundResult();
            }

            int userId = WebSecurity.GetUserId(userName);
            if (userId == -1)
            {
                return new HttpNotFoundResult();
            }

            var report = context.Reports.
                Where(x => x.UserId == userId && x.Year == year && x.Month == month).
                Include(x => x.UserProfile).
                FirstOrDefault();

            if (report == null)
            {
                return new HttpNotFoundResult();
            }

            return View(report);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Report model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (WebSecurity.GetUserId(User.Identity.Name) != model.UserId && !User.IsInRole("Administrator"))
            {
                return new HttpNotFoundResult();
            }

            var report = context.Reports.Find(model.UserId, model.Year, model.Month);
            
            if (report == null)
            {
                return new HttpNotFoundResult();
            }

            report.Hours = model.Hours;
            context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Report model)
        {
            if (!ModelState.IsValid)
            {
                return new HttpNotFoundResult();
            }
            
            if (WebSecurity.GetUserId(User.Identity.Name) != model.UserId && !User.IsInRole("Administrator"))
            {
                return new HttpNotFoundResult();
            }

            var report = context.Reports.Find(model.UserId, model.Year, model.Month);
            
            if (report == null)
            {
                return new HttpNotFoundResult();
            }
            
            context.Reports.Remove(report);
            context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
