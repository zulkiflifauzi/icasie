using Icasie.Helper;
using Icasie.Models;
using Icasie.Repositories;
using reCAPTCHA.MVC;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Icasie.Controllers
{
    public class LoginController : Controller
    {

        public ActionResult ChangePassword()      
        { 
            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(ViewModelChangePassword model)
        {
            string role = string.Empty;
            if(model.NewPassword != model.RetypePassword)
            {
                ModelState.AddModelError("PasswordDontMatch", "'ReType New Password' and 'New Password' do not match");
                return View(model);
            }

            using (IcasieEntities entity = new IcasieEntities())
            {
                string currentUserEmail = HttpContext.User.Identity.Name;
                var user = entity.Users.SingleOrDefault(c => c.Email == currentUserEmail);
                string salt = user.Salt;
                string password = Icasie.Helper.Helper.CreatePasswordHash(model.NewPassword, salt);

                user.Password = password;

                role = user.Role;

                entity.SaveChanges();

                System.Threading.Tasks.Task.Run(() => EmailHelper.SendEmailPasswordChange(user.Email, user.FirstName + " " + user.LastName, model.NewPassword));

                
            }

            ViewData["message"] =" Your password succesfully changed";

            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [CaptchaValidator]
        public ActionResult Register(ViewModelUser model, bool captchaValid)
        {
            if (!ModelState.IsValid)
                return View(model);

            string randomPassword = Icasie.Helper.Helper.GenerateRandomGuidPassword().Substring(0, 10);

            var random = new Random();
            string salt = Icasie.Helper.Helper.CreateSalt(random.Next(10, 100));
            string password = Icasie.Helper.Helper.CreatePasswordHash(randomPassword, salt);
            
            using (IcasieEntities entity = new IcasieEntities())
            {
                if (entity.Users.Any(c => c.Email == model.Email))
                {
                    ViewData["message"] = "User already exist";
                    return View(model);
                }
                Icasie.Repositories.User user = new Icasie.Repositories.User();
                user.Email = model.Email;
                user.City = model.City;
                user.Country = model.Country;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Password = password;
                user.PhoneNumber = model.PhoneNumber;
                user.Role = model.Role;
                user.Salt = salt;
                user.State = model.State;
                user.StreetAddress = model.StreetAddress;
                user.Gender = model.Gender;
                user.Institution = model.Institution;

                entity.Users.Add(user);
                entity.SaveChanges();

                System.Threading.Tasks.Task.Run(() => EmailHelper.SendEmailNewUser(model.Email, model.FirstName + " " + model.LastName, randomPassword, ConfigurationManager.AppSettings["SiteName"]));
                
            }
            
            return View("RegisterSuccess");
        }

        public ActionResult Logoff()
        {
            FormsAuthentication.SignOut();
            Session.Clear();  
            return RedirectToAction("Login");
        }
        
        //
        // GET: /Login/
        public ActionResult Login()
        {
            //get user credential
            string currentUserEmail = HttpContext.User.Identity.Name;

            if (currentUserEmail != "")
            {
                Icasie.Repositories.User user = new Repositories.User();

                using (IcasieEntities entity = new IcasieEntities())
                {
                    user = entity.Users.SingleOrDefault(c => c.Email == currentUserEmail);
                }

                switch (user.Role)
                {
                    case Constant.Role.Author:
                        return RedirectToAction("Index", "Submission");
                    case Constant.Role.Participant:
                        return RedirectToAction("Index", "Participant");
                    default:
                        return RedirectToAction("Index", "Conference");
                }
            }

            return View();
        }

        [HttpPost]
        [CaptchaValidator]
        public ActionResult Login(ViewModelLogin login, bool captchaValid)
        {
            if (!ModelState.IsValid)
            {
                return View(login);
            }

            Icasie.Repositories.User user = new Repositories.User();

            using (IcasieEntities entity = new IcasieEntities())
            {
                user = entity.Users.SingleOrDefault(c => c.Email == login.Email);
            }

            if (user != null && Helper.Helper.CreatePasswordHash(login.Password, user.Salt) == user.Password)
            {
                int timeout = Constant.CookieExpiration.Normal;
                var ticket = new FormsAuthenticationTicket(1, user.Email, DateTime.Now, DateTime.Now.AddMinutes(timeout), true, user.Role);
                var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(ticket));

                if (ticket.IsPersistent)
                {
                    cookie.Expires = ticket.Expiration;
                }

                Response.Cookies.Add(cookie);
            }
            else {
                TempData["Message"] = "User & Password combination cannot be found";

                return View(login);
            }

            switch (user.Role)
            { 
                case Constant.Role.Author:
                    return RedirectToAction("Index", "Submission");
                case Constant.Role.Participant:
                    return RedirectToAction("Index", "Participant");
                default:
                    return RedirectToAction("Index", "Conference");
            }
        }
	}
}