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

        /// <summary>
        /// Возвращает список отчетов для всех сотрудников, если пользователь входит в группу Administrator,
        /// иначе - список отчетов только для него самого
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Выборка отчетов определенного сотрудника
        /// </summary>
        /// <param name="userName">Логин сотрудника</param>
        /// <returns></returns>
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

        /// <summary>
        /// Выборка за определенный год
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Выборка за конкретный месяц
        /// </summary>
        /// <param name="month"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Выборка отчетов сотрудника за определенный год
        /// </summary>
        /// <param name="year"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Выборка отчетов сотрудника только за определенный месяц
        /// </summary>
        /// <param name="month"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Выборка отчетов за определенные год и месяц
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Возвращает форму добавления нового отчета для аутентифицированного пользователя
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            return RedirectToAction("CreateForUser", new
            {
                username = User.Identity.Name
            });
        }

        /// <summary>
        /// Возвращает форму добавления нового отчета для выбранного сотрудника
        /// </summary>
        /// <param name="userName">Логин сотрудника</param>
        /// <returns></returns>
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

            return View("Create", new Report
            {
                Year = DateTime.Now.Year,
                Month = DateTime.Now.Month,
                UserId = userId,
                HourlyRate = (user.CurrentHourlyRate != null) ? user.CurrentHourlyRate.Value : 0,
                UserProfile = user
            });
        }

        /// <summary>
        /// Создает новый отчет в БД на основе данных модели
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateForUser(Report model)
        {
            if (!ModelState.IsValid)
            {
                return View("Create", model);
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
                return View("Create", model);
            }

            var report = context.Reports.Find(model.UserId, model.Year, model.Month);
            if (report != null)
            {
                ModelState.AddModelError("", "Отчет за " +
                    new DateTime(model.Year, model.Month, 1).ToString("MMMM").ToLower() + " " +
                    model.Year.ToString() + " года уже существует");
                return View("Create", model);
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


        /// <summary>
        /// Возвращает форму редактирования отчета
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="userName">Логин сотрудника</param>
        /// <returns></returns>
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

        /// <summary>
        /// Обновляет отчет в БД
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Удаляет отчет
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
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
