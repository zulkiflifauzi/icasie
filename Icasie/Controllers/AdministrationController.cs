using Icasie.Helper;
using Icasie.Models;
using Icasie.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Icasie.Controllers
{
    [Authorize(Roles="Administrator")]
    public class AdministrationController : Controller
    {
        //
        // GET: /Administration/
        public ActionResult ParticipantPayment(int id)
        {
            //get user credential
            string currentUserEmail = HttpContext.User.Identity.Name;

            List<ViewModelConference> confList = new List<ViewModelConference>();

            using (IcasieEntities entity = new IcasieEntities())
            {

                var conference = entity.Conferences.Where(c => c.ConferenceId == id).ToList();

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
                    newConf.Participants = new List<ViewModelParticipant>();

                    var participants = entity.Participants.Where(c => c.ConferenceId == item.ConferenceId);
                    foreach (var subItem in participants)
                    {
                        var user = entity.Users.FirstOrDefault(c => c.UserId == subItem.UserId);

                        ViewModelParticipant newParticipant = new ViewModelParticipant();
                        newParticipant.Name = user.FirstName + " " + user.LastName;
                        newParticipant.UserId = subItem.UserId;
                        newParticipant.ConferenceId = subItem.ConferenceId;
                        newParticipant.ParticipantId = subItem.ParticipantId;
                        newParticipant.Payment = subItem.Payment;
                        newParticipant.PaymentDate = subItem.PaymentDate.ToString(Icasie.Helper.Constant.DateFormat);
                        newParticipant.PaymentFileName = subItem.PaymentFileName;
                        newParticipant.PaymentStatus = subItem.PaymentStatus;
                        newParticipant.Number = subItem.Number;
                        newParticipant.TotalPayment = subItem.TotalPayment;

                        newConf.Participants.Add(newParticipant);
                    }

                    confList.Add(newConf);

                }

            }
            return View(confList);
        }

        public ActionResult AuthorPayment(int id)
        {
            //get user credential
            string currentUserEmail = HttpContext.User.Identity.Name;

            ViewModelConference newConf = new ViewModelConference();

            using (IcasieEntities entity = new IcasieEntities())
            {
                var subThemes = entity.SubThemes.ToList();

                var conference = entity.Conferences.SingleOrDefault(c => c.ConferenceId == id);

                //fill the properties
                newConf.ConferenceId = conference.ConferenceId;
                newConf.Name = conference.Name;
                newConf.StartDate = conference.StartDate;
                newConf.EndDate = conference.EndDate;
                newConf.PaperSubStartDate = conference.PaperSubStartDate;
                newConf.PaperSubEndDate = conference.PaperSubEndDate;
                newConf.Submissions = new List<ViewModelSubmission>();

                var uses = entity.Users.ToList();

                var submissions = entity.Submissions.Where(c => c.ConferenceId == conference.ConferenceId && (c.FullPaperStatus == Constant.FullPaperStatus.Pending || c.FullPaperStatus == Constant.FullPaperStatus.PaymentVerified ));
                foreach (var subItem in submissions)
                {
                    var user = uses.SingleOrDefault(c => c.UserId == subItem.UserId);

                    ViewModelSubmission newSubmission = new ViewModelSubmission();
                    newSubmission.UserId = subItem.UserId;
                    newSubmission.PaymentVerificationDate = subItem.PaymentVerificationDate.HasValue ? subItem.PaymentVerificationDate.Value : newSubmission.PaymentVerificationDate;
                    newSubmission.AuthorName = user.FirstName + " " + user.LastName;
                    newSubmission.ConferenceId = subItem.ConferenceId;
                    newSubmission.FullPaperStatus = subItem.FullPaperStatus;
                    newSubmission.Title = subItem.Title;
                    newSubmission.SubmissionId = subItem.SubmissionId;
                    newSubmission.Number = subItem.PaperNumber;
                    newSubmission.SubThemesId = subItem.SubThemesId;
                    newSubmission.TotalPayment = subItem.TotalPayment.Value;
                    newSubmission.SubThemes = subThemes.FirstOrDefault(c => c.SubThemesId == subItem.SubThemesId).Title;
                    newSubmission.PaymentTypeString = subItem.PaymentType == Constant.PaymentType.Full ? Constant.PaymentType.FullString : Constant.PaymentType.PartialString;
                    
                    newSubmission.PaymentDate = subItem.PaymentDate;


                    newConf.Submissions.Add(newSubmission);
                }

            }
            return View(newConf);
        }

        public ActionResult VerifyPayment(int id, int conferenceId)
        {
            using (IcasieEntities entity = new IcasieEntities())
            {
                var submission = entity.Submissions.SingleOrDefault(c => c.SubmissionId == id);
                submission.FullPaperStatus = Constant.FullPaperStatus.PaymentVerified;
                submission.PaymentVerificationDate = DateTime.Now;
                entity.SaveChanges();
            }

            return RedirectToAction("AuthorPayment", new { id = conferenceId });
        }

        public ActionResult VerifyPaymentParticipant(int id, string status, int conferenceId)
        {
            using (IcasieEntities entity = new IcasieEntities())
            {
                var payment = entity.Participants.SingleOrDefault(c => c.ParticipantId == id);
                if (status == Constant.PaymentStatus.Accepted)
                {
                    payment.Number = GetLatestSequenceNumber(conferenceId);
                    payment.PaymentStatus = Constant.PaymentStatus.Accepted;
                }
                else
                    payment.PaymentStatus = Constant.PaymentStatus.Rejected;

                entity.SaveChanges();
            }

            return RedirectToAction("ParticipantPayment", new { id = conferenceId });
        }

        [HttpPost]
        public ActionResult Assign(int reviewer, int submission, int proofReader)
        {
            string conferenceName = string.Empty;
            string title = string.Empty;
            List<string> reviewerEmail = new List<string>();
            using (var db = new IcasieEntities())
            {
                var sub = db.Submissions.SingleOrDefault(c => c.SubmissionId == submission);
                conferenceName = db.Conferences.SingleOrDefault(c => c.ConferenceId == sub.ConferenceId).Name;
                title = sub.Title;
                sub.ReviewedBy = reviewer;
                sub.ProofReaderId = proofReader;
                db.SaveChanges();

                reviewerEmail.Add(db.Users.SingleOrDefault(c => c.UserId == reviewer).Email);
            }

            Task.Run(() => Helper.EmailHelper.SendEmailNotification(reviewerEmail, Constant.NotificationMode.AssignReviewer, conferenceName, title));

            return RedirectToAction("Assign");
        }

        public ActionResult Assign(int id)
        {
            //get user credential
            string currentUserEmail = HttpContext.User.Identity.Name;

            ViewModelConference newConf = new ViewModelConference();

            PrepareSelectList();

            using (IcasieEntities entity = new IcasieEntities())
            {
                var subThemes = entity.SubThemes.ToList();

                var conference = entity.Conferences.SingleOrDefault(c => c.ConferenceId == id);

                //fill the properties
                newConf.ConferenceId = conference.ConferenceId;
                newConf.Name = conference.Name;
                newConf.StartDate = conference.StartDate;
                newConf.EndDate = conference.EndDate;
                newConf.PaperSubStartDate = conference.PaperSubStartDate;
                newConf.PaperSubEndDate = conference.PaperSubEndDate;
                newConf.Submissions = new List<ViewModelSubmission>();

                var uses = entity.Users.ToList();

                var submissions = entity.Submissions.Where(c => c.ConferenceId == conference.ConferenceId);
                foreach (var subItem in submissions)
                {
                    var user = uses.SingleOrDefault(c => c.UserId == subItem.UserId);
                    var reviwer = uses.SingleOrDefault(c => c.UserId == subItem.ReviewedBy);
                    var proofReader = uses.SingleOrDefault(c => c.UserId == subItem.ProofReaderId);

                    ViewModelSubmission newSubmission = new ViewModelSubmission();
                    newSubmission.UserId = subItem.UserId;
                    newSubmission.AuthorName = user.FirstName + " " + user.LastName;
                    newSubmission.ConferenceId = subItem.ConferenceId;
                    newSubmission.FullPaperStatus = subItem.FullPaperStatus;
                    newSubmission.Title = subItem.Title;
                    newSubmission.SubmissionId = subItem.SubmissionId;
                    newSubmission.Number = subItem.PaperNumber;
                    newSubmission.SubThemesId = subItem.SubThemesId;
                    newSubmission.SubThemes = subThemes.FirstOrDefault(c => c.SubThemesId == subItem.SubThemesId).Title;
                    newSubmission.PaymentTypeString = subItem.PaymentType == Constant.PaymentType.Full ? Constant.PaymentType.FullString : Constant.PaymentType.PartialString;
                    newSubmission.PaymentType = subItem.PaymentType;
                    newSubmission.ReviewerName = subItem.ReviewedBy.HasValue ? reviwer.FirstName + " " + reviwer.LastName : "";
                    newSubmission.ReviewedBy = subItem.ReviewedBy.HasValue ? subItem.ReviewedBy.Value : 0;
                    newSubmission.ProofReaderName = subItem.ProofReaderId.HasValue ? proofReader.FirstName + " " + proofReader.LastName : "";
                    newSubmission.ProofReaderId = subItem.ProofReaderId.HasValue ? subItem.ProofReaderId.Value : 0;


                    newConf.Submissions.Add(newSubmission);
                }

            }
            return View(newConf);
        }

        private void PrepareSelectList()
        {
            List<SelectListItem> reviewerList = new List<SelectListItem>();


            using (IcasieEntities entity = new IcasieEntities())
            {
                var reviewers = entity.Users.Where(c => c.Role.Equals(Constant.Role.Reviewer)).ToList();

                foreach (var item in reviewers)
                {
                    reviewerList.Add(new SelectListItem { Text = item.FirstName + " " + item.LastName, Value = item.UserId.ToString() });
                }
            }


            ViewData["Reviewers"] = reviewerList;

            List<SelectListItem> proofReaderList = new List<SelectListItem>();


            using (IcasieEntities entity = new IcasieEntities())
            {
                var proofReaders = entity.Users.Where(c => c.Role.Equals(Constant.Role.ProofReader)).ToList();

                foreach (var item in proofReaders)
                {
                    proofReaderList.Add(new SelectListItem { Text = item.FirstName + " " + item.LastName, Value = item.UserId.ToString() });
                }
            }


            ViewData["ProofReaders"] = proofReaderList;
        }

        private string GetLatestSequenceNumber(int conferenceId)
        {
            using (var db = new IcasieEntities())
            {
                var conference = db.Conferences.SingleOrDefault(c => c.ConferenceId == conferenceId);
                var newSequenceNumber = NumberGenerator.NxtKeyCode(conference.LastParticipantNumber);
                conference.LastParticipantNumber = newSequenceNumber;
                db.SaveChanges();

                return newSequenceNumber;
            }
        }

	}
}