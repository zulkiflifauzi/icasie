using Icasie.Helper;
using Icasie.Models;
using Icasie.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity.Validation;
using System.Threading.Tasks;

namespace Icasie.Controllers
{
    [Authorize(Roles = "Reviewer, Administrator")]
    public class ReviewerController : Controller
    {
        //
        // GET: /Reviewer/
        public ActionResult Index(int id)
        {
            List<ViewModelConference> confList = new List<ViewModelConference>();
            //get user credential
            string currentUserEmail = HttpContext.User.Identity.Name;

            using (IcasieEntities entity = new IcasieEntities())
            {
                var conference = entity.Conferences.Where(c => c.ConferenceId == id).ToList();

                var subThemes = entity.SubThemes.ToList();

                var reviewerId = entity.Users.SingleOrDefault(c => c.Email == currentUserEmail);

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

                    var submissions = entity.Submissions.Where(c => c.ConferenceId == item.ConferenceId && c.ReviewedBy == reviewerId.UserId && (c.FullPaperStatus == Constant.FullPaperStatus.FormatChecked || c.FullPaperStatus == Constant.FullPaperStatus.Revised));
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
                        newSubmission.Payment = subItem.Payment;
                        newSubmission.PaymentDate = subItem.PaymentDate;
                        newSubmission.PaymentFileName = subItem.PaymentFileName;
                        newSubmission.Comment = subItem.Comment;
                        newSubmission.Title = subItem.Title;
                        newSubmission.SubmissionId = subItem.SubmissionId;
                        newSubmission.FullPaperReview = subItem.FullPaperReview;
                        newSubmission.FullPaperReviewFileName = subItem.FullPaperReviewFileName;
                        newSubmission.Number = subItem.PaperNumber;
                        newSubmission.SubThemesId = subItem.SubThemesId;
                        newSubmission.SubThemes = subThemes.FirstOrDefault(c => c.SubThemesId == subItem.SubThemesId).Title;


                        if (subItem.ReviewedBy != null)
                        {
                            var reviewer = entity.Users.SingleOrDefault(c => c.UserId == subItem.ReviewedBy);
                            newSubmission.ReviewerName = reviewer.FirstName + " " + reviewer.LastName;
                            newSubmission.FullPaperReviewFileName = subItem.FullPaperReviewFileName;
                            newSubmission.FullPaperReviewFileName2 = subItem.FullPaperReviewFileName2;
                            newSubmission.FullPaperReviewFileName3 = subItem.FullPaperReviewFileName3;
                        }


                        newSubmission.ReviewDateString = subItem.ReviewDate.HasValue ? subItem.ReviewDate.Value.ToString(Constant.DateFormat) : string.Empty;

                        newConf.Submissions.Add(newSubmission);
                    }

                    confList.Add(newConf);

                }


            }
            return View(confList);
        }

        public ActionResult Edit(int id)
        {
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
        public ActionResult Edit(int id, string fullPaperStatus, string comment, HttpPostedFileBase file, HttpPostedFileBase file2, HttpPostedFileBase file3)
        {
            int conferenceId = 0;
            var conferenceName = string.Empty;
            var paperTitle = string.Empty;
            List<string> proofReaderEmail = new List<string>();

            if (fullPaperStatus == Constant.FullPaperStatus.Revised && file == null)
            {
                ModelState.AddModelError("file", "File cannot be empty");
            }

            if (fullPaperStatus == Constant.FullPaperStatus.Revised || fullPaperStatus == Constant.FullPaperStatus.Rejected)
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

                    sub.ReviewDate = DateTime.Now;

                    string currentUserEmail = HttpContext.User.Identity.Name;

                    int userId = entity.Users.SingleOrDefault(c => c.Email == currentUserEmail).UserId;

                    sub.ReviewedBy = userId;

                    if (fullPaperStatus == Constant.FullPaperStatus.Rejected || fullPaperStatus == Constant.FullPaperStatus.Reviewed)
                    {
                        sub.FullPaperReview = null;
                        sub.FullPaperReviewFileName = null;
                        sub.FullPaperReview2 = null;
                        sub.FullPaperReviewFileName2 = null;
                        sub.FullPaperReview3 = null;
                        sub.FullPaperReviewFileName3 = null;
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
                                sub.FullPaperReview = memoryStream.ToArray();
                                sub.FullPaperReviewFileName = file.FileName;
                            }
                        }

                        if (file2 != null)
                        {
                            using (Stream inputStream = file2.InputStream)
                            {
                                MemoryStream memoryStream = inputStream as MemoryStream;
                                if (memoryStream == null)
                                {
                                    memoryStream = new MemoryStream();
                                    inputStream.CopyTo(memoryStream);
                                }
                                sub.FullPaperReview2 = memoryStream.ToArray();
                                sub.FullPaperReviewFileName2 = file2.FileName;
                            }
                        }

                        if (file3 != null)
                        {
                            using (Stream inputStream = file3.InputStream)
                            {
                                MemoryStream memoryStream = inputStream as MemoryStream;
                                if (memoryStream == null)
                                {
                                    memoryStream = new MemoryStream();
                                    inputStream.CopyTo(memoryStream);
                                }
                                sub.FullPaperReview3 = memoryStream.ToArray();
                                sub.FullPaperReviewFileName3 = file3.FileName;
                            }
                        }

                        conferenceName = entity.Conferences.SingleOrDefault(c => c.ConferenceId == sub.ConferenceId).Name;
                        paperTitle = sub.Title;

                        proofReaderEmail.Add(entity.Users.SingleOrDefault(c => c.UserId == sub.ProofReaderId).Email);

                        Task.Run(() => Helper.EmailHelper.SendEmailNotification(proofReaderEmail, Constant.NotificationMode.AssignProofReader, conferenceName, paperTitle));

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