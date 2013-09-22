using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;
using TimeTracking2.Filters;
using TimeTracking2.Models;

namespace TimeTracking2.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class AccountController : Controller
    {
        private EFDbContext context = new EFDbContext();

        /// <summary>
        /// Возвращает форму входа
        /// </summary>
        /// <param name="returnUrl">Локальный url, переадресация на который будет осуществлена после входа</param>
        /// <returns></returns>
        [AllowAnonymous]
        public ViewResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        /// <summary>
        /// Производит попытку аутентификации пользователя
        /// </summary>
        /// <param name="model"></param>
        /// <param name="returnUrl">Локальный url, переадресация на который будет осуществлена после входа</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
            {
                return RedirectToLocal(returnUrl);
            }
            ModelState.AddModelError("", "Имя пользователя или пароль указаны неверно");
            return View(model);
        }


        /// <summary>
        /// Выполняет выход пользователя из системы
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            WebSecurity.Logout();
            return RedirectToAction("Index", "Home");
        }


        /// <summary>
        /// Возвращает форму регистрации нового пользователя
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ViewResult Register()
        {
            return View();
        }

        /// <summary>
        /// Производит попытку создания новой учетной записи пользователя
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    WebSecurity.CreateUserAndAccount(
                        model.UserProfile.UserName,
                        model.Password,
                        new
                        {
                            FirstName = model.UserProfile.FirstName,
                            LastName = model.UserProfile.LastName,
                            MiddleName = model.UserProfile.MiddleName
                        });
                    WebSecurity.Login(model.UserProfile.UserName, model.Password);

                    return RedirectToAction("Index", "Home");
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                }
            }
            return View(model);
        }


        /// <summary>
        /// Возвращает форму изменения пароля пользователя
        /// </summary>
        /// <returns></returns>
        public ViewResult ChangePassword()
        {
            return View();
        }


        /// <summary>
        /// Изменяет пароль пользователя на новый
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(PasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                // В ряде случаев при сбое ChangePassword породит исключение, а не вернет false.
                bool changePasswordSucceeded;
                try
                {
                    changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (!changePasswordSucceeded)
                {
                    ModelState.AddModelError("", "Неправильный текущий пароль или недопустимый новый пароль.");
                    return View(model);
                }
            }
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Возвращает список всех пользователей
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            if (!User.IsInRole("Administrator"))
            {
                return new HttpNotFoundResult();
            }

            var users = context.UserProfiles.
                OrderBy(u => u.LastName).
                ThenBy(u => u.FirstName);

            return View(users);
        }

        /// <summary>
        /// Удаляет профиль пользователя из БД
        /// </summary>
        /// <param name="userId">Id удаляемого пользователя</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int userId)
        {
            if (!User.IsInRole("Administrator"))
            {
                return new HttpNotFoundResult();
            }

            var user = context.UserProfiles.Find(userId);
            if (user == null)
            {
                return new HttpNotFoundResult();
            }

            if (User.Identity.Name == user.UserName)
            {
                return new HttpNotFoundResult();
            }
            
            context.UserProfiles.Remove(user);
            context.SaveChanges();

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Возвращает форму редактирования профиля пользователя
        /// </summary>
        /// <param name="userName">Логин пользователя</param>
        /// <returns></returns>
        public ActionResult Edit(string userName)
        {
            if (!User.IsInRole("Administrator"))
            {
                return new HttpNotFoundResult();
            }

            int userId = WebSecurity.GetUserId(userName);
            if (userId == -1)
            {
                return new HttpNotFoundResult();
            }

            var user = context.UserProfiles.Find(userId);

            return View(user);
        }

        /// <summary>
        /// Обновляет профиль пользователя в БД так, чтобы он соответствовал переданной модели  
        /// </summary>
        /// <param name="model">Профиль пользователя, подлежащий изменению</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UserProfile model)
        {
            if (!User.IsInRole("Administrator"))
            {
                return new HttpNotFoundResult();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = context.UserProfiles.Find(model.UserId);

            if (user == null)
            {
                return new HttpNotFoundResult();
            }

            user.FirstName = model.FirstName;
            user.MiddleName = model.MiddleName;
            user.LastName = model.LastName;
            user.Appointment = model.Appointment;
            user.CurrentHourlyRate = model.CurrentHourlyRate;

            context.SaveChanges();

            return RedirectToAction("Index");
        }

        #region Вспомогательные методы
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // Полный список кодов состояния см. по адресу http://go.microsoft.com/fwlink/?LinkID=177550
            //
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "Имя пользователя уже существует; введите другое имя пользователя";

                case MembershipCreateStatus.DuplicateEmail:
                    return "Имя пользователя для данного адреса электронной почты уже существует; введите другой адрес электронной почты";

                case MembershipCreateStatus.InvalidPassword:
                    return "Указан недопустимый пароль; введите допустимое значение пароля";

                case MembershipCreateStatus.InvalidEmail:
                    return "Указан недопустимый адрес электронной почты; проверьте значение и повторите попытку";

                case MembershipCreateStatus.InvalidAnswer:
                    return "Указан недопустимый ответ на вопрос для восстановления пароля; проверьте значение и повторите попытку";

                case MembershipCreateStatus.InvalidQuestion:
                    return "Указан недопустимый вопрос для восстановления пароля; проверьте значение и повторите попытку";

                case MembershipCreateStatus.InvalidUserName:
                    return "Указано недопустимое имя пользователя; проверьте значение и повторите попытку";

                case MembershipCreateStatus.ProviderError:
                    return "Поставщик проверки подлинности вернул ошибку; проверьте введенное значение и повторите попытку. Если проблему устранить не удастся, обратитесь к системному администратору";

                case MembershipCreateStatus.UserRejected:
                    return "Запрос создания пользователя был отменен; проверьте введенное значение и повторите попытку. Если проблему устранить не удастся, обратитесь к системному администратору";

                default:
                    return "Произошла неизвестная ошибка; проверьте введенное значение и повторите попытку. Если проблему устранить не удастся, обратитесь к системному администратору";
            }
        }
        #endregion
    }
}
