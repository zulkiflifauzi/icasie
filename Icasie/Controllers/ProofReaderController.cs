using Icasie.Helper;
using Icasie.Models;
using Icasie.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Icasie.Controllers
{
    [Authorize(Roles = "Proof Reader, Administrator")]
    public class ProofReaderController : Controller
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

                var proofReaderId = entity.Users.SingleOrDefault(c => c.Email == currentUserEmail);

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

                    var submissions = entity.Submissions.Where(c => c.ConferenceId == item.ConferenceId && c.ProofReaderId == proofReaderId.UserId && (c.FullPaperStatus == Constant.FullPaperStatus.Reviewed || c.FullPaperStatus == Constant.FullPaperStatus.ProofReadingRevised));
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


                        if (subItem.ProofReaderId != null)
                        {
                            var proofReader = entity.Users.SingleOrDefault(c => c.UserId == subItem.ProofReaderId);
                            newSubmission.ProofReaderName = proofReader.FirstName + " " + proofReader.LastName;
                            newSubmission.ProofReaderResultFileName = subItem.ProofReadingResultFileName;
                        }


                        newSubmission.ProofReadingDateString = subItem.ProofReadingDate.HasValue ? subItem.ProofReadingDate.Value.ToString(Constant.DateFormat) : string.Empty;

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
        public ActionResult Edit(int id, string fullPaperStatus, string comment, HttpPostedFileBase file)
        {
            int conferenceId = 0;
            if (fullPaperStatus == Constant.FullPaperStatus.ProofReadingRevised && file == null)
            {
                ModelState.AddModelError("file", "File cannot be empty");
            }

            if (fullPaperStatus == Constant.FullPaperStatus.ProofReadingRevised)
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

                    sub.ProofReadingDate = DateTime.Now;

                    string currentUserEmail = HttpContext.User.Identity.Name;

                    int userId = entity.Users.SingleOrDefault(c => c.Email == currentUserEmail).UserId;

                    sub.ProofReaderId = userId;

                    if (fullPaperStatus == Constant.FullPaperStatus.Accepted)
                    {
                        sub.ProofReadingResult = null;
                        sub.ProofReadingResultFileName = null;
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
                                sub.ProofReadingResult = memoryStream.ToArray();
                                sub.ProofReadingResultFileName = file.FileName;
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