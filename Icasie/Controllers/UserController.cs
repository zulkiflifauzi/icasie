using Icasie.Helper;
using Icasie.Models;
using Icasie.Repositories;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Icasie.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        [Authorize(Roles = "Administrator")]
        public ActionResult Index(string id)
        {
            ViewData["Role"] = id;
            List<ViewModelUser> results = new List<ViewModelUser>();
            using (IcasieEntities entity = new IcasieEntities())
            {
                var users = entity.Users.Where(c => c.Role == id).ToList();
                foreach (var item in users)
                {
                    ViewModelUser result = new ViewModelUser();
                    result.UserId = item.UserId;
                    result.Email = item.Email;
                    result.Password = item.Password;
                    result.Salt = item.Salt;
                    result.Role = item.Role;
                    result.FirstName = item.FirstName;
                    result.LastName = item.LastName;
                    result.Gender = item.Gender;
                    result.StreetAddress = item.StreetAddress;
                    result.City = item.City;
                    result.State = item.State;
                    result.Country = item.Country;
                    result.Institution = item.Institution;
                    result.PhoneNumber = item.PhoneNumber;

                    results.Add(result);
                }
            }
            return View(results);
        }

        public ActionResult AddReviewer()
        {
            return View();
        }

        public ActionResult AddProofReader()
        {
            return View();
        }

        public ActionResult AddFormatChecker()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddFormatChecker(ViewModelUser model)
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
                user.Role = Constant.Role.FormatChecker;
                user.Salt = salt;
                user.State = model.State;
                user.StreetAddress = model.StreetAddress;
                user.Gender = model.Gender;
                user.Institution = model.Institution;

                entity.Users.Add(user);
                entity.SaveChanges();

                System.Threading.Tasks.Task.Run(() => EmailHelper.SendEmailNewUser(model.Email, model.FirstName + " " + model.LastName, randomPassword, ConfigurationManager.AppSettings["SiteName"]));

            }

            return RedirectToAction("Index", new { id = Constant.Role.FormatChecker });
        }

        [HttpPost]
        public ActionResult AddReviewer(ViewModelUser model)
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
                user.Role = Constant.Role.Reviewer;
                user.Salt = salt;
                user.State = model.State;
                user.StreetAddress = model.StreetAddress;
                user.Gender = model.Gender;
                user.Institution = model.Institution;

                entity.Users.Add(user);
                entity.SaveChanges();

                System.Threading.Tasks.Task.Run(() => EmailHelper.SendEmailNewUser(model.Email, model.FirstName + " " + model.LastName, randomPassword, ConfigurationManager.AppSettings["SiteName"]));
            }

            return RedirectToAction("Index", new { id = Constant.Role.Reviewer });
        }

        [HttpPost]
        public ActionResult AddProofReader(ViewModelUser model)
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
                user.Role = Constant.Role.ProofReader;
                user.Salt = salt;
                user.State = model.State;
                user.StreetAddress = model.StreetAddress;
                user.Gender = model.Gender;
                user.Institution = model.Institution;

                entity.Users.Add(user);
                entity.SaveChanges();

                System.Threading.Tasks.Task.Run(() => EmailHelper.SendEmailNewUser(model.Email, model.FirstName + " " + model.LastName, randomPassword, ConfigurationManager.AppSettings["SiteName"]));

            }

            return RedirectToAction("Index", new { id = Constant.Role.Reviewer });
        }

        public ActionResult Profile(int id)
        {
            ViewModelUser result = new ViewModelUser();
            using (IcasieEntities entity = new IcasieEntities())
            {
                var user = entity.Users.SingleOrDefault(c => c.UserId == id);
                result.UserId = user.UserId;
                result.Email = user.Email;
                result.Password = user.Password;
                result.Salt = user.Salt;
                result.Role = user.Role;
                result.FirstName = user.FirstName;
                result.LastName = user.LastName;
                result.Gender = user.Gender;
                result.StreetAddress = user.StreetAddress;
                result.City = user.City;
                result.State = user.State;
                result.Country = user.Country;
                result.Institution = user.Institution;
                result.PhoneNumber = user.PhoneNumber;
            }
            return View(result);
        }
    }
}