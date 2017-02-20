using Icasie.Helper;
using Icasie.Models;
using Icasie.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Icasie.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult PaperList(int id)
        {
            List<ViewModelSubmission> results = new List<ViewModelSubmission>();
            using (IcasieEntities entity = new IcasieEntities())
            {

                var submissions = entity.Submissions.Where(c => c.ConferenceId == id).OrderBy(c => c.FullPaperDate);
                foreach (var subItem in submissions)
                {

                    ViewModelSubmission newSubmission = new ViewModelSubmission();

                    var user = entity.Users.SingleOrDefault(c => c.UserId == subItem.UserId);

                    newSubmission.AuthorName = user.FirstName + " " + user.LastName;
                    newSubmission.UserId = subItem.UserId;
                    newSubmission.ConferenceId = subItem.ConferenceId;
                    newSubmission.FullPaperStatus = subItem.FullPaperStatus;
                    newSubmission.FullPaperDate = subItem.FullPaperDate;
                    newSubmission.PaymentDate = subItem.PaymentDate;
                    newSubmission.Comment = subItem.Comment;
                    newSubmission.Title = subItem.Title;
                    newSubmission.SubmissionId = subItem.SubmissionId;
                    newSubmission.Institution = user.Institution;
                    newSubmission.Number = subItem.PaperNumber;
                    results.Add(newSubmission);
                }
            }
            return View(results);
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