using Icasie.Helper;
using Icasie.Models;
using Icasie.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Icasie.Controllers
{
    [Authorize(Roles = "Format Checker, Administrator")]
    public class FormatCheckerController : Controller
    {
        public ActionResult Index(int id)
        {
            List<ViewModelConference> confList = new List<ViewModelConference>();
            //get user credential
            string currentUserEmail = HttpContext.User.Identity.Name;

            using (IcasieEntities entity = new IcasieEntities())
            {
                var conference = entity.Conferences.Where(c => c.ConferenceId == id).ToList();

                var subThemes = entity.SubThemes.ToList();

                var formatChecker = entity.Users.SingleOrDefault(c => c.Email == currentUserEmail);

                foreach (var item in conference)
                {
                    ViewModelConference newConf = new ViewModelConference();
                    newConf.PaperSubActive = false;

                    if (item.PaperSubStartDate <= DateTime.Now && item.PaperSubEndDate >= DateTime.Now)
                    {
                        newConf.PaperSubActive = true;
                    }

                    //fill the properties
                    newConf.ConferenceId = item.ConferenceId;
                    newConf.Name = item.Name;
                    newConf.StartDate = item.StartDate;
                    newConf.EndDate = item.EndDate;
                    newConf.PaperSubStartDate = item.PaperSubStartDate;
                    newConf.PaperSubEndDate = item.PaperSubEndDate;
                    newConf.Submissions = new List<ViewModelSubmission>();

                    var submissions = entity.Submissions.Where(c => c.ConferenceId == item.ConferenceId && c.FormatCheckerId == formatChecker.UserId && (c.FullPaperStatus == Constant.FullPaperStatus.PaymentVerified || c.FullPaperStatus == Constant.FullPaperStatus.FormatRevised));
                    foreach (var subItem in submissions)
                    {
                        ViewModelSubmission newSubmission = new ViewModelSubmission();
                        newSubmission.UserId = subItem.UserId;

                        var user = entity.Users.SingleOrDefault(c => c.UserId == newSubmission.UserId);

                        newSubmission.AuthorName = user.FirstName + " " + user.LastName;
                        newSubmission.ConferenceId = subItem.ConferenceId;
                        newSubmission.FullPaperStatus = subItem.FullPaperStatus;
                        newSubmission.FullPaper = subItem.FullPaper;
                        newSubmission.FullPaperDate = subItem.FullPaperDate;
                        newSubmission.FullPaperFileName = subItem.FullPaperFileName;
                        newSubmission.Title = subItem.Title;
                        newSubmission.SubmissionId = subItem.SubmissionId;
                        newSubmission.Number = subItem.PaperNumber;
                        newSubmission.SubThemesId = subItem.SubThemesId;
                        newSubmission.SubThemes = subThemes.FirstOrDefault(c => c.SubThemesId == subItem.SubThemesId).Title;


                        if (subItem.FormatCheckerId != null)
                        {
                            var formatCheckerId = entity.Users.SingleOrDefault(c => c.UserId == subItem.FormatCheckerId);
                            newSubmission.FormatCheckerName = formatCheckerId.FirstName + " " + formatCheckerId.LastName;
                            newSubmission.FormatCheckingResultFileName = subItem.FormatCheckingResultFileName;
                        }


                        newSubmission.FormatCheckingDateString = subItem.FormatCheckingDate.HasValue ? subItem.FormatCheckingDate.Value.ToString(Constant.DateFormat) : string.Empty;

                        newConf.Submissions.Add(newSubmission);
                    }

                    confList.Add(newConf);

                }


            }
            return View(confList);
        }

        public ActionResult Edit(int id) {
            ViewModelSubmission sub = new ViewModelSubmission();
            using (IcasieEntities entity = new IcasieEntities())
            {
                var oldSub = entity.Submissions.SingleOrDefault(c => c.SubmissionId == id);
                sub.SubmissionId = oldSub.SubmissionId;
                sub.ConferenceId = oldSub.ConferenceId;
                sub.Title = oldSub.Title;
            }
            return View(sub);
        }

        [HttpPost]
        public ActionResult Edit(int id, string fullPaperStatus, string comment, HttpPostedFileBase file)
        {
            int conferenceId = 0;
            if (fullPaperStatus == Constant.FullPaperStatus.FormatRevised && file == null)
            {
                ModelState.AddModelError("file", "File cannot be empty");
            }

            if (fullPaperStatus == Constant.FullPaperStatus.FormatRevised)
            {
                if (comment == null || comment == string.Empty)
                {
                    ModelState.AddModelError("comment", "Comment is required");
                }
            }

            if (!ModelState.IsValid)
            {
                ViewModelSubmission sub = new ViewModelSubmission();
                using (IcasieEntities entity = new IcasieEntities())
                {
                    var oldSub = entity.Submissions.SingleOrDefault(c => c.SubmissionId == id);
                    sub.SubmissionId = oldSub.SubmissionId;
                    sub.Title = oldSub.Title;
                    sub.FullPaperStatus = fullPaperStatus;
                    sub.Comment = comment;
                }

                return View(sub);
            }

            try
            {
                using (IcasieEntities entity = new IcasieEntities())
                {
                    var sub = entity.Submissions.SingleOrDefault(c => c.SubmissionId == id);
                    sub.Comment = comment;
                    sub.FullPaperStatus = fullPaperStatus;
                    conferenceId = sub.ConferenceId;

                    sub.FormatCheckingDate = DateTime.Now;

                    string currentUserEmail = HttpContext.User.Identity.Name;

                    int userId = entity.Users.SingleOrDefault(c => c.Email == currentUserEmail).UserId;

                    sub.FormatCheckerId = userId;

                    if (fullPaperStatus == Constant.FullPaperStatus.FormatChecked)
                    {
                        sub.FormatCheckingResult = null;
                        sub.FormatCheckingResultFileName = null;

                        var conferenceName = entity.Conferences.SingleOrDefault(c => c.ConferenceId == sub.ConferenceId).Name;
                        var title = sub.Title;
                        List<string> reviewerEmail = new List<string>();

                        reviewerEmail.Add(entity.Users.SingleOrDefault(c => c.UserId == sub.ReviewedBy).Email);
                        Task.Run(() => Helper.EmailHelper.SendEmailNotification(reviewerEmail, Constant.NotificationMode.AssignReviewer, conferenceName, title));
                    }
                    else
                    {

                        if (file != null)
                        {
                            using (Stream inputStream = file.InputStream)
                            {
                                MemoryStream memoryStream = inputStream as MemoryStream;
                                if (memoryStream == null)
                                {
                                    memoryStream = new MemoryStream();
                                    inputStream.CopyTo(memoryStream);
                                }
                                sub.FormatCheckingResult = memoryStream.ToArray();
                                sub.FormatCheckingResultFileName = file.FileName;
                            }
                        }


                    }


                    entity.SaveChanges();
                }
            }
            catch (DbEntityValidationException ex)
            {
            }


            return RedirectToAction("Index", new { id = conferenceId });
        }
	}
}