using Icasie.Helper;
using Icasie.Models;
using Icasie.Repositories;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Icasie.Controllers
{
    [Authorize(Roles = "Reviewer, Proof Reader, Format Checker, Administrator")]
    public class ConferenceController : Controller
    {
        //
        // GET: /Conference/
        public ActionResult Index()
        {
            List<ViewModelConference> results = new List<ViewModelConference>();
            using (IcasieEntities entity = new IcasieEntities())
            {
                var conferences = entity.Conferences.ToList();
                foreach (var item in conferences)
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

                    results.Add(newConf);
                }
            }
            return View(results);
        }

        public ActionResult Payments()
        {
            List<ViewModelConference> results = new List<ViewModelConference>();
            using (IcasieEntities entity = new IcasieEntities())
            {
                var conferences = entity.Conferences.ToList();
                foreach (var item in conferences)
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

                    results.Add(newConf);
                }
            }
            return View(results);
        }

        public ActionResult Assign(int id)
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        public ActionResult Edit(int id)
        {
            ViewModelConference result = new ViewModelConference();
            using (IcasieEntities entity = new IcasieEntities())
            {
                var conf = entity.Conferences.SingleOrDefault(c => c.ConferenceId == id);
                result.Name = conf.Name;
                result.ConferenceId = conf.ConferenceId;
                result.StartDateString = conf.StartDate.ToString(Constant.DateFormatInput);
                result.EndDateString = conf.EndDate.ToString(Constant.DateFormatInput);
                result.PaperSubStartDateString = conf.PaperSubStartDate.ToString(Constant.DateFormatInput);
                result.PaperSubEndDateString = conf.PaperSubEndDate.ToString(Constant.DateFormatInput);
            }

            return View(result);
        }

        [HttpPost]
        public ActionResult Edit(ViewModelConference model)
        {
            DateTime outStartDate = new DateTime();
            DateTime outEndDate = new DateTime();
            DateTime outPaperStartDate = new DateTime();
            DateTime outPaperEndDate = new DateTime();

            IFormatProvider provider = CultureInfo.CurrentCulture;
            if (!DateTime.TryParseExact(model.StartDateString, Constant.DateFormatInput, provider, DateTimeStyles.None, out outStartDate))
            {
                ModelState.AddModelError("StartDateString", "Start Date format is not valid");
            }

            if (!DateTime.TryParseExact(model.EndDateString, Constant.DateFormatInput, provider, DateTimeStyles.None, out outEndDate))
            {
                ModelState.AddModelError("EndDateString", "End Date format is not valid");
            }

            if (!DateTime.TryParseExact(model.PaperSubStartDateString, Constant.DateFormatInput, provider, DateTimeStyles.None, out outPaperStartDate))
            {
                ModelState.AddModelError("PaperSubStartDateString", "Paper Submission Start Date format is not valid");
            }

            if (!DateTime.TryParseExact(model.PaperSubEndDateString, Constant.DateFormatInput, provider, DateTimeStyles.None, out outPaperEndDate))
            {
                ModelState.AddModelError("PaperSubEndDateString", "Paper Submission End Date format is not valid");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }


            using (IcasieEntities entity = new IcasieEntities())
            {
                var odlConf = entity.Conferences.SingleOrDefault(c => c.ConferenceId == model.ConferenceId);

                odlConf.Name = model.Name;
                odlConf.StartDate = outStartDate;
                odlConf.EndDate = outEndDate;
                odlConf.PaperSubStartDate = outPaperStartDate;
                odlConf.PaperSubEndDate = outPaperEndDate;

                entity.SaveChanges();

            }
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            bool isExist = false;

            using (IcasieEntities entity = new IcasieEntities())
            {
                isExist = entity.Submissions.Any(c => c.ConferenceId == id);
                if (isExist == false)
                {
                    var conference = entity.Conferences.SingleOrDefault(c => c.ConferenceId == id);

                    entity.Conferences.Remove(conference);
                    entity.SaveChanges();
                }
            }

            if (isExist == true)
            {
                TempData["Message"] = "Conference cannot be deleted";
            }

            return RedirectToAction("index");
        }

        [HttpPost]
        public ActionResult Create(ViewModelConference model)
        {
            DateTime outStartDate = new DateTime();
            DateTime outEndDate = new DateTime();
            DateTime outPaperStartDate = new DateTime();
            DateTime outPaperEndDate = new DateTime();

            IFormatProvider provider = CultureInfo.CurrentCulture;
            if (!DateTime.TryParseExact(model.StartDateString, Constant.DateFormatInput, provider, DateTimeStyles.None, out outStartDate))
            {
                ModelState.AddModelError("StartDateString", "Start Date format is not valid");
            }

            if (!DateTime.TryParseExact(model.EndDateString, Constant.DateFormatInput, provider, DateTimeStyles.None, out outEndDate))
            {
                ModelState.AddModelError("EndDateString", "End Date format is not valid");
            }

            if (!DateTime.TryParseExact(model.PaperSubStartDateString, Constant.DateFormatInput, provider, DateTimeStyles.None, out outPaperStartDate))
            {
                ModelState.AddModelError("PaperSubStartDateString", "Paper Submission Start Date format is not valid");
            }

            if (!DateTime.TryParseExact(model.PaperSubEndDateString, Constant.DateFormatInput, provider, DateTimeStyles.None, out outPaperEndDate))
            {
                ModelState.AddModelError("PaperSubEndDateString", "Paper Submission End Date format is not valid");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }


            using (IcasieEntities entity = new IcasieEntities())
            {
                Conference conference = new Conference();

                var prefix = model.Name.Replace(" ", "");
                prefix = prefix.ToUpper();

                conference.Name = model.Name;
                conference.StartDate = outStartDate;
                conference.EndDate = outEndDate;
                conference.PaperSubStartDate = outPaperStartDate;
                conference.PaperSubEndDate = outPaperEndDate;
                conference.LastSequenceNumber = prefix + "-" + Constant.NumberSuffix;
                conference.LastParticipantNumber = prefix + "-PART-" + Constant.NumberSuffix;

                entity.Conferences.Add(conference);
                entity.SaveChanges();

            }
            return RedirectToAction("Index");
        }
    }
}