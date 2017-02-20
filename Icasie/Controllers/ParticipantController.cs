using Icasie.Helper;
using Icasie.Models;
using Icasie.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Icasie.Controllers
{
    [Authorize]
    public class ParticipantController : Controller
    {
        //
        // GET: /Participant/
        [Authorize(Roles="Participant")]
        public ActionResult Index()
        {
            //get user credential
            string currentUserEmail = HttpContext.User.Identity.Name;

            List<ViewModelConference> confList = new List<ViewModelConference>();

            using (IcasieEntities entity = new IcasieEntities())
            {
                var user = entity.Users.SingleOrDefault(c => c.Email == currentUserEmail);
                int userId = user.UserId;

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
                    newConf.Participants = new List<ViewModelParticipant>();

                    var participants = entity.Participants.Where(c => c.UserId == userId && c.ConferenceId == item.ConferenceId);
                    foreach (var subItem in participants)
                    {
                        ViewModelParticipant newParticipant = new ViewModelParticipant();
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

        [Authorize(Roles = "Participant")]
        public ActionResult ChangePayment(int id)
        {
            ViewData["ParticipantId"] = id;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Participant")]
        public ActionResult ChangePayment(int id, HttpPostedFileBase payment)
        {

            if (payment == null)
            {
                ModelState.AddModelError("payment", "Payment evidence cannot be empty");
            }

            if (!ModelState.IsValid)
            {
                ViewModelParticipant participant = new ViewModelParticipant();
                participant.ParticipantId = id;
                return View(participant);
            }
            
            using (IcasieEntities entity = new IcasieEntities())
            {
                var part = entity.Participants.SingleOrDefault(c => c.ParticipantId == id);

                string currentUserEmail = HttpContext.User.Identity.Name;

                var user = entity.Users.SingleOrDefault(c => c.Email == currentUserEmail);

                part.PaymentFileName = payment.FileName;
                part.PaymentDate = DateTime.Now;

                var fees = entity.Fees.ToList();

                if (user.Country != Constant.Indonesia)
                {
                    ViewData["Fee"] = fees.SingleOrDefault(c => c.Name.Equals(Constant.Fees.OverseasParticipant)).Price;
                }
                else
                {
                    ViewData["Fee"] = fees.SingleOrDefault(c => c.Name.Equals(Constant.Fees.IndonesianParticipant)).Price;
                }

                using (Stream inputStream = payment.InputStream)
                {
                    MemoryStream memoryStream = inputStream as MemoryStream;
                    if (memoryStream == null)
                    {
                        memoryStream = new MemoryStream();
                        inputStream.CopyTo(memoryStream);
                    }
                    part.Payment = memoryStream.ToArray();
                }
                
                entity.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Participant")]
        public ActionResult Register(int id)
        {
            string currentUserEmail = HttpContext.User.Identity.Name;

            using(IcasieEntities entity = new IcasieEntities())
            {
                var user = entity.Users.SingleOrDefault(c => c.Email.Equals(currentUserEmail));
                var fees = entity.Fees.ToList();

                if (user.Country != Constant.Indonesia)
                {
                    ViewData["Fee"] = fees.SingleOrDefault(c => c.Name.Equals(Constant.Fees.OverseasParticipant)).Price;
                }
                else
                {
                    ViewData["Fee"] = fees.SingleOrDefault(c => c.Name.Equals(Constant.Fees.IndonesianParticipant)).Price;
                }
            
            }            

            ViewData["ConferenceId"] = id;
           
            return View();        
        }

        [HttpPost]
        [Authorize(Roles = "Participant")]
        public ActionResult Register(int id, HttpPostedFileBase payment)
        {

            if (payment == null)
            {
                ModelState.AddModelError("payment", "Payment evidence cannot be empty");
            }

            if (!ModelState.IsValid)
            {
                ViewModelParticipant participant = new ViewModelParticipant();
                return View(participant);
            }

            using (IcasieEntities entity = new IcasieEntities())
            {
                Participant part = new Participant();

                string currentUserEmail = HttpContext.User.Identity.Name;

                var user = entity.Users.SingleOrDefault(c => c.Email == currentUserEmail);

                part.UserId = user.UserId;
                part.ConferenceId = id;
                part.PaymentFileName = payment.FileName;
                part.PaymentDate = DateTime.Now;
                part.PaymentStatus = Constant.PaymentStatus.Submitted;
                var fees = entity.Fees.ToList();

                if (user.Country != Constant.Indonesia)
                {
                    part.TotalPayment = fees.SingleOrDefault(c => c.Name.Equals(Constant.Fees.OverseasParticipant)).Price;
                }
                else
                {
                    part.TotalPayment = fees.SingleOrDefault(c => c.Name.Equals(Constant.Fees.IndonesianParticipant)).Price;
                } 

                using (Stream inputStream = payment.InputStream)
                {
                    MemoryStream memoryStream = inputStream as MemoryStream;
                    if (memoryStream == null)
                    {
                        memoryStream = new MemoryStream();
                        inputStream.CopyTo(memoryStream);
                    }
                    part.Payment = memoryStream.ToArray();
                }

                entity.Participants.Add(part);

                entity.SaveChanges();
            }

            return RedirectToAction("Index");
        }
	}
}