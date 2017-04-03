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
    [Authorize]
    public class SubmissionController : Controller
    {
        //
        // GET: /Submission/
        [Authorize(Roles = "Author, Administrator, Reviewer")]
        public ActionResult Index()
        {
            //get user credential
            string currentUserEmail = HttpContext.User.Identity.Name;

            List<ViewModelConference> confList = new List<ViewModelConference>(); 

            using (IcasieEntities entity = new IcasieEntities())
            {
                var user = entity.Users.SingleOrDefault(c => c.Email == currentUserEmail);
                int userId = user.UserId;

                var subThemes = entity.SubThemes.ToList();

                var conference = entity.Conferences.ToList();                

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

                    var submissions = entity.Submissions.Where(c => c.UserId == userId && c.ConferenceId == item.ConferenceId);
                    foreach (var subItem in submissions)
                    {
                        ViewModelSubmission newSubmission = new ViewModelSubmission();
                        newSubmission.UserId = subItem.UserId;
                        newSubmission.ConferenceId = subItem.ConferenceId;
                        newSubmission.FullPaperStatus = subItem.FullPaperStatus;
                        newSubmission.FullPaperDate = subItem.FullPaperDate;
                        newSubmission.PaymentDate = subItem.PaymentDate;
                        newSubmission.Comment = subItem.Comment;
                        newSubmission.Title = subItem.Title;
                        newSubmission.SubmissionId = subItem.SubmissionId;
                        newSubmission.Number = subItem.PaperNumber;
                        newSubmission.SubThemesId = subItem.SubThemesId;
                        newSubmission.SubThemes = subThemes.FirstOrDefault(c => c.SubThemesId == subItem.SubThemesId).Title;
                        newSubmission.PaymentTypeString = subItem.PaymentType == Constant.PaymentType.Full ? Constant.PaymentType.FullString : Constant.PaymentType.PartialString;
                        newSubmission.PaymentType = subItem.PaymentType;
                        newSubmission.CoAuthors = subItem.CoAuthors;
                        newSubmission.TotalPayment = subItem.TotalPayment.Value;

                        if(subItem.ReviewedBy != null)
                        {   
                            var reviewer = entity.Users.SingleOrDefault(c => c.UserId == subItem.ReviewedBy);
                            newSubmission.ReviewerName = reviewer.FirstName + " " + reviewer.LastName;
                            newSubmission.FullPaperReviewFileName = subItem.FullPaperReviewFileName;
                            newSubmission.FullPaperReviewFileName2 = subItem.FullPaperReviewFileName2;
                            newSubmission.FullPaperReviewFileName3 = subItem.FullPaperReviewFileName3;
                        }

                        if (subItem.ProofReaderId != null)
                        {
                            var proofReader = entity.Users.SingleOrDefault(c => c.UserId == subItem.ProofReaderId);
                            newSubmission.ProofReaderName = proofReader.FirstName + " " + proofReader.LastName;
                            newSubmission.ProofReaderResultFileName = subItem.ProofReadingResultFileName;
                        }

                        if (subItem.FormatCheckerId != null)
                        {
                            var formatChecker = entity.Users.SingleOrDefault(c => c.UserId == subItem.FormatCheckerId);
                            newSubmission.FormatCheckerName = formatChecker.FirstName + " " + formatChecker.LastName;
                            newSubmission.FormatCheckingResultFileName = subItem.FormatCheckingResultFileName;
                        }


                        newSubmission.ReviewDateString = subItem.ReviewDate.HasValue ? subItem.ReviewDate.Value.ToString(Constant.DateFormat) : string.Empty;
                        newSubmission.ProofReadingDateString = subItem.ProofReadingDate.HasValue ? subItem.ProofReadingDate.Value.ToString(Constant.DateFormat) : string.Empty;
                        newSubmission.FormatCheckingDateString = subItem.FormatCheckingDate.HasValue ? subItem.FormatCheckingDate.Value.ToString(Constant.DateFormat) : string.Empty;  

                        newConf.Submissions.Add(newSubmission);
                    }

                    confList.Add(newConf);

                }


            }
            return View(confList);
        }


        [Authorize(Roles = "Author, Administrator, Reviewer")]
        public ActionResult EditFullPaper(int id)
        {
            ViewModelSubmission sub = new ViewModelSubmission();
            using (IcasieEntities entity = new IcasieEntities())
            {
                var oldSub = entity.Submissions.SingleOrDefault(c => c.SubmissionId == id);
                sub.SubmissionId = oldSub.SubmissionId;
                sub.Title = oldSub.Title;
                sub.SubThemesId = oldSub.SubThemesId;
                sub.CoAuthors = oldSub.CoAuthors;
                sub.PaymentType = oldSub.PaymentType;
                sub.FullPaperStatus = oldSub.FullPaperStatus;

                PrepareViewData(entity);
            }

            PrepareSelectList();

            return View(sub);
        }

        [Authorize(Roles = "Author, Administrator, Proof Reader")]
        public ActionResult EditProofReading(int id)
        {
            ViewModelSubmission sub = new ViewModelSubmission();
            using (IcasieEntities entity = new IcasieEntities())
            {
                var oldSub = entity.Submissions.SingleOrDefault(c => c.SubmissionId == id);
                sub.SubmissionId = oldSub.SubmissionId;
                sub.Title = oldSub.Title;
                sub.SubThemesId = oldSub.SubThemesId;
            }
            PrepareSelectList();
            return View(sub);
        }

        [HttpPost]
        [Authorize(Roles = "Author, Administrator, Proof Reader")]
        public ActionResult EditProofReading(int id, HttpPostedFileBase file)
        {
            if (file == null)
            {
                ModelState.AddModelError("file", "File cannot be empty");

                ViewModelSubmission sub = new ViewModelSubmission();
                using (IcasieEntities entity = new IcasieEntities())
                {
                    var oldSub = entity.Submissions.SingleOrDefault(c => c.SubmissionId == id);
                    sub.SubmissionId = oldSub.SubmissionId;
                    sub.Title = oldSub.Title;
                    sub.SubThemesId = oldSub.SubThemesId;
                }

                PrepareSelectList();

                return View(sub);
            }

            using (IcasieEntities entity = new IcasieEntities())
            {
                var sub = entity.Submissions.SingleOrDefault(c => c.SubmissionId == id);
                sub.FullPaperStatus = Constant.FullPaperStatus.Reviewed;
                sub.ProofReadingDate = null;
                sub.ProofReadingResultFileName = null;
                sub.ProofReadingResult = null;
                sub.Comment = Constant.InitialCommentProofRead;
                
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
                        sub.FullPaper = memoryStream.ToArray();
                        sub.FullPaperFileName = file.FileName;
                    }
                }


                entity.SaveChanges();
            }

            return RedirectToAction("Index");
        }


        [Authorize(Roles="Administrator")]
        public ActionResult PaperList(int id)
        {
            List<ViewModelSubmission> results = new List<ViewModelSubmission>();
            using (IcasieEntities entity = new IcasieEntities())
            {
                var conference = entity.Conferences.SingleOrDefault(c => c.ConferenceId == id);
                ViewData["ConferenceName"] = conference.Name;

                var submissions = entity.Submissions.Where(c => c.ConferenceId == id).OrderBy(c => c.FullPaperDate);
                foreach (var subItem in submissions)
                {
                    ViewModelSubmission newSubmission = new ViewModelSubmission();

                    var user = entity.Users.SingleOrDefault(c => c.UserId == subItem.UserId);
                    var formatChecker = entity.Users.SingleOrDefault(c => c.UserId == subItem.FormatCheckerId);
                    var reviewer = entity.Users.SingleOrDefault(c => c.UserId == subItem.ReviewedBy);
                    var proofReader = entity.Users.SingleOrDefault(c => c.UserId == subItem.ProofReaderId);

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
                    newSubmission.FormatCheckerName = reviewer != null ? formatChecker.FirstName + " " + formatChecker.LastName : "";
                    newSubmission.ReviewerName = reviewer != null ? reviewer.FirstName + " " + reviewer.LastName : "";
                    newSubmission.ProofReaderName = proofReader != null ? proofReader.FirstName + " " + proofReader.LastName : "";
                    results.Add(newSubmission);
                }
            }
            return View(results);
        }

        private void PrepareViewData(IcasieEntities entity)
        {
            string currentUserEmail = HttpContext.User.Identity.Name;

            var user = entity.Users.SingleOrDefault(c => c.Email.Equals(currentUserEmail));
            var fees = entity.Fees.ToList();

            if (user.Country != Constant.Indonesia)
            {
                ViewData["Full"] = fees.SingleOrDefault(c => c.Name.Equals(Constant.Fees.OverseasFull)).Price;
                ViewData["Seminar"] = fees.SingleOrDefault(c => c.Name.Equals(Constant.Fees.OverseasSeminar)).Price;
                ViewData["Coauthor"] = fees.SingleOrDefault(c => c.Name.Equals(Constant.Fees.OverseasCoauthor)).Price;
                ViewData["IsIndonesian"] = "true";
            }
            else
            {
                ViewData["Full"] = fees.SingleOrDefault(c => c.Name.Equals(Constant.Fees.IndonesianFull)).Price;
                ViewData["Seminar"] = fees.SingleOrDefault(c => c.Name.Equals(Constant.Fees.IndonesianSeminar)).Price;
                ViewData["Coauthor"] = fees.SingleOrDefault(c => c.Name.Equals(Constant.Fees.IndonesianCoauthor)).Price;
                ViewData["IsIndonesian"] = "false";
            }
        }

        [HttpPost]
        [Authorize(Roles = "Author, Administrator, Reviewer")]
        public ActionResult EditFullPaper(int id, string title, HttpPostedFileBase file, int subThemesId, int coAuthors = 0, int paymentType = 0)
        {
            if (file == null)
            {
                ModelState.AddModelError("file", "File cannot be empty");

                ViewModelSubmission sub = new ViewModelSubmission();
                using (IcasieEntities entity = new IcasieEntities())
                {
                    var oldSub = entity.Submissions.SingleOrDefault(c => c.SubmissionId == id);
                    sub.SubmissionId = oldSub.SubmissionId;
                    sub.Title = oldSub.Title;
                    sub.SubThemesId = oldSub.SubThemesId;                 
                    sub.CoAuthors = oldSub.CoAuthors;
                    sub.PaymentType = oldSub.PaymentType;
                    sub.FullPaperStatus = oldSub.FullPaperStatus;

                    PrepareViewData(entity);
                }

                PrepareSelectList();

                return View(sub);
            }

            using (IcasieEntities entity = new IcasieEntities())
            {
                var sub = entity.Submissions.SingleOrDefault(c => c.SubmissionId == id);
                sub.Title = title;
                //sub.FullPaperStatus = Constant.FullPaperStatus.Pending;
                sub.ReviewDate = null;
                sub.FullPaperReview = null;
                sub.FullPaperReviewFileName = null;
                sub.FullPaperReview2 = null;
                sub.FullPaperReviewFileName2 = null;
                sub.FullPaperReview3 = null;
                sub.FullPaperReviewFileName3 = null;
                sub.ProofReadingResult = null;
                sub.ProofReadingResultFileName = null;
                sub.FormatCheckingResult = null;
                sub.FormatCheckingResultFileName = null;
                sub.Comment = Constant.InitialComment;
                sub.SubThemesId = subThemesId;                
                sub.CoAuthors = coAuthors;
                sub.PaymentType = paymentType;

                var user = entity.Users.SingleOrDefault(c => c.UserId == sub.UserId);


                if (sub.FullPaperStatus.Equals(Constant.FullPaperStatus.Pending))
                {
                    var fees = entity.Fees.ToList();

                    decimal full = 0;
                    decimal seminar = 0;
                    decimal coauthor = 0;


                    if (user.Country != Constant.Indonesia)
                    {
                        full = fees.SingleOrDefault(c => c.Name.Equals(Constant.Fees.OverseasFull)).Price;
                        seminar = fees.SingleOrDefault(c => c.Name.Equals(Constant.Fees.OverseasSeminar)).Price;
                        coauthor = fees.SingleOrDefault(c => c.Name.Equals(Constant.Fees.OverseasCoauthor)).Price;
                    }
                    else
                    {
                        full = fees.SingleOrDefault(c => c.Name.Equals(Constant.Fees.IndonesianFull)).Price;
                        seminar = fees.SingleOrDefault(c => c.Name.Equals(Constant.Fees.IndonesianSeminar)).Price;
                        coauthor = fees.SingleOrDefault(c => c.Name.Equals(Constant.Fees.IndonesianCoauthor)).Price;
                    }

                    decimal total = 0;
                    if (paymentType == Constant.PaymentType.Full)
                    {
                        total += full;
                    }
                    else
                    {
                        total += seminar;
                    }

                    total += (coAuthors * coauthor);
                    sub.TotalPayment = total;   
                }
                else if(sub.FullPaperStatus.Equals(Constant.FullPaperStatus.FormatRevised))
                {
                    sub.FullPaperStatus = Constant.FullPaperStatus.PaymentVerified;
                }
                else if (sub.FullPaperStatus.Equals(Constant.FullPaperStatus.Revised))
                {
                    sub.FullPaperStatus = Constant.FullPaperStatus.FormatChecked;
                }
                else if (sub.FullPaperStatus.Equals(Constant.FullPaperStatus.ProofReadingRevised))
                {
                    sub.FullPaperStatus = Constant.FullPaperStatus.Reviewed;
                }
                             

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
                        sub.FullPaper = memoryStream.ToArray();
                        sub.FullPaperFileName = file.FileName;
                    }
                }
                

                entity.SaveChanges();
            }            

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Author, Administrator, Reviewer")]
        public ActionResult AddFullPaper(int id)
        {
            ViewData["ConferenceId"] = id;
            
            string currentUserEmail = HttpContext.User.Identity.Name;

            using (IcasieEntities entity = new IcasieEntities())
            {
                PrepareViewData(entity);
            }

            PrepareSelectList();

            return View();
        }

        private void PrepareSelectList()
        {
            List<SelectListItem> subThemeList = new List<SelectListItem>();

            subThemeList.Add(new SelectListItem { Text = "Choose Sub Theme", Value = "" });

            using (IcasieEntities entity = new IcasieEntities())
            {
                var subThemes = entity.SubThemes.ToList();

                foreach (var item in subThemes)
                {
                    subThemeList.Add(new SelectListItem { Text = item.Title, Value = item.SubThemesId.ToString() });
                }
            }


            ViewData["SubThemesId"] = subThemeList; 
        }

        [HttpPost]
        [Authorize(Roles = "Author, Administrator, Reviewer")]
        public ActionResult AddFullPaper(int id, string title, HttpPostedFileBase file, HttpPostedFileBase payment, int subThemesId, int coAuthors, int paymentType, bool termsAgreement)
        {
            List<string> adminEmails = new List<string>();
            string conferenceName = string.Empty;

            //get user credential
            string currentUserEmail = HttpContext.User.Identity.Name;

            if (file == null)
            {
                ModelState.AddModelError("file", "File cannot be empty");    
            }

            if (payment == null)
            {
                ModelState.AddModelError("payment", "Payment Evidence cannot be empty");
            }

            if (termsAgreement == false)
            {
                ModelState.AddModelError("termsAgreement", "You must accept this agreement to proceed"); 
            }


            if (!ModelState.IsValid)
            {
                PrepareSelectList();

                using (IcasieEntities entity = new IcasieEntities())
                {
                    PrepareViewData(entity);
                }

                return View();
            }
            
                       
            Submission subs = new Submission();
            subs.ConferenceId = id;
            subs.FullPaperFileName = file.FileName;
            subs.FullPaperDate = DateTime.Now;
            subs.PaymentFileName = payment.FileName;
            subs.PaymentDate = DateTime.Now;
            subs.PaperNumber = GetLatestSequenceNumber(id);
            subs.SubThemesId = subThemesId;
            subs.FullPaperStatus = Constant.FullPaperStatus.Pending;
            subs.Title = title;
            subs.CoAuthors = coAuthors;
            subs.PaymentType = paymentType;
            subs.Comment = Constant.InitialComment;
            subs.TermsAgreement = termsAgreement;
            using (Stream inputStream = file.InputStream)
            {
                MemoryStream memoryStream = inputStream as MemoryStream;
                if (memoryStream == null)
                {
                    memoryStream = new MemoryStream();
                    inputStream.CopyTo(memoryStream);
                }
                subs.FullPaper = memoryStream.ToArray();

            }

            using (Stream inputStream = payment.InputStream)
            {
                MemoryStream memoryStream = inputStream as MemoryStream;
                if (memoryStream == null)
                {
                    memoryStream = new MemoryStream();
                    inputStream.CopyTo(memoryStream);
                }
                subs.Payment = memoryStream.ToArray();

            }
            using (IcasieEntities entity = new IcasieEntities())
            {
                var user = entity.Users.SingleOrDefault(c => c.Email.Equals(currentUserEmail));
                var fees = entity.Fees.ToList();

                decimal full = 0;
                decimal seminar = 0;
                decimal coauthor = 0;


                if (user.Country != Constant.Indonesia)
                {
                    full = fees.SingleOrDefault(c => c.Name.Equals(Constant.Fees.OverseasFull)).Price;
                    seminar = fees.SingleOrDefault(c => c.Name.Equals(Constant.Fees.OverseasSeminar)).Price;
                    coauthor = fees.SingleOrDefault(c => c.Name.Equals(Constant.Fees.OverseasCoauthor)).Price;
                }
                else
                {
                    full = fees.SingleOrDefault(c => c.Name.Equals(Constant.Fees.IndonesianFull)).Price;
                    seminar = fees.SingleOrDefault(c => c.Name.Equals(Constant.Fees.IndonesianSeminar)).Price;
                    coauthor = fees.SingleOrDefault(c => c.Name.Equals(Constant.Fees.IndonesianCoauthor)).Price;
                }

                subs.UserId = user.UserId;

                decimal total = 0;
                if (paymentType == Constant.PaymentType.Full)
                {
                    total += full;
                }
                else
                {
                    total += seminar;
                }

                total += (coAuthors * coauthor);
                subs.TotalPayment = total;

                entity.Submissions.Add(subs);
                entity.SaveChanges();

                adminEmails = entity.Users.Where(c => c.Role.Equals(Constant.Role.Administrator)).Select(c => c.Email).ToList();
                conferenceName = entity.Conferences.SingleOrDefault(c => c.ConferenceId == id).Name;
            }

            Task.Run(() => Helper.EmailHelper.SendEmailNotification(adminEmails, Constant.NotificationMode.NewSubmission, conferenceName, title));
                       
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Author, Administrator, Reviewer")]
        public ActionResult SubmitPayment(int id)
        {
            ViewModelSubmission sub = new ViewModelSubmission();
            sub.SubmissionId = id;
            return View(sub);
        }


        private string GetLatestSequenceNumber(int conferenceId)
        {
            using (var db = new IcasieEntities())
            {
                var conference = db.Conferences.SingleOrDefault(c => c.ConferenceId == conferenceId);
                var newSequenceNumber = NumberGenerator.NxtKeyCode(conference.LastSequenceNumber);
                conference.LastSequenceNumber = newSequenceNumber;
                db.SaveChanges();

                return newSequenceNumber;
            }
        }

        [HttpPost]
        [Authorize(Roles = "Author, Administrator, Reviewer")]
        public ActionResult SubmitPayment(int id, HttpPostedFileBase payment)
        {

            if (payment == null)
            {
                ModelState.AddModelError("payment", "Payment evidence cannot be empty");
            }

            if (!ModelState.IsValid)
            {
                ViewModelSubmission sub = new ViewModelSubmission();
                sub.SubmissionId = id;
                return View(sub);
            }
            
            using (IcasieEntities entity = new IcasieEntities())
            {
                var sub = entity.Submissions.SingleOrDefault(c => c.SubmissionId == id);
                sub.PaymentFileName = payment.FileName;
                sub.FullPaperDate = DateTime.Now;
                sub.PaymentDate = DateTime.Now;
                
                using (Stream inputStream = payment.InputStream)
                {
                    MemoryStream memoryStream = inputStream as MemoryStream;
                    if (memoryStream == null)
                    {
                        memoryStream = new MemoryStream();
                        inputStream.CopyTo(memoryStream);
                    }
                    sub.Payment = memoryStream.ToArray();
                }

                entity.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public FileResult Download(int id, string type)
        {
            Submission sub = new Submission();
            Participant par = new Participant();
            using (IcasieEntities entity = new IcasieEntities())
            {
                sub = entity.Submissions.SingleOrDefault(c => c.SubmissionId == id);
                par = entity.Participants.SingleOrDefault(c => c.ParticipantId == id);
            }

            byte[] fileBytes = null;
            string fileName = string.Empty;
            
            if (type == Constant.DownloadType.Review)
            {
                fileBytes = sub.FullPaperReview;
                fileName = sub.FullPaperReviewFileName ;
            }

            if (type == Constant.DownloadType.Review2)
            {
                fileBytes = sub.FullPaperReview2;
                fileName = sub.FullPaperReviewFileName2;
            }

            if (type == Constant.DownloadType.Review3)
            {
                fileBytes = sub.FullPaperReview3;
                fileName = sub.FullPaperReviewFileName3;
            }

            if (type == Constant.DownloadType.FullPaper)
            {
                fileBytes = sub.FullPaper;
                fileName = sub.FullPaperFileName;
            }

            if (type == Constant.DownloadType.Payment)
            {
                fileBytes = sub.Payment;
                fileName = sub.PaymentFileName;
            }

            if (type == Constant.DownloadType.ParticipantPayment)
            {
                fileBytes = par.Payment;
                fileName = par.PaymentFileName;
            }

            if (type == Constant.DownloadType.ProofReading)
            {
                fileBytes = sub.ProofReadingResult;
                fileName = sub.ProofReadingResultFileName;
            }

            if (type == Constant.DownloadType.FormatChecking)
            {
                fileBytes = sub.FormatCheckingResult;
                fileName = sub.FormatCheckingResultFileName;
            }
            
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }
	}
}