using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Icasie.Helper
{
    public static class Helper
    {
        public static List<SelectListItem> GetAbstractStatusList()
        {
            List<SelectListItem> result = new List<SelectListItem>();

            SelectListItem revised = new SelectListItem();
            revised.Text = Constant.FullPaperStatus.Revised;
            revised.Value = Constant.FullPaperStatus.Revised;

            result.Add(revised);

            SelectListItem accepted = new SelectListItem();
            accepted.Text = Constant.FullPaperStatus.Reviewed;
            accepted.Value = Constant.FullPaperStatus.Reviewed;

            result.Add(accepted);

            SelectListItem rejected = new SelectListItem();
            rejected.Text = Constant.FullPaperStatus.Rejected;
            rejected.Value = Constant.FullPaperStatus.Rejected;

            result.Add(rejected);

            return result;
        }

        public static List<SelectListItem> GetProofReadingStatusList()
        {
            List<SelectListItem> result = new List<SelectListItem>();

            SelectListItem revised = new SelectListItem();
            revised.Text = Constant.FullPaperStatus.ProofReadingRevised;
            revised.Value = Constant.FullPaperStatus.ProofReadingRevised;

            result.Add(revised);

            SelectListItem accepted = new SelectListItem();
            accepted.Text = Constant.FullPaperStatus.Accepted;
            accepted.Value = Constant.FullPaperStatus.Accepted;

            result.Add(accepted);

            return result;
        }

        public static List<SelectListItem> GetFormatCheckingStatusList()
        {
            List<SelectListItem> result = new List<SelectListItem>();

            SelectListItem revised = new SelectListItem();
            revised.Text = Constant.FullPaperStatus.FormatRevised;
            revised.Value = Constant.FullPaperStatus.FormatRevised;

            result.Add(revised);

            SelectListItem accepted = new SelectListItem();
            accepted.Text = Constant.FullPaperStatus.FormatChecked;
            accepted.Value = Constant.FullPaperStatus.FormatChecked;

            result.Add(accepted);

            return result;
        }

        public static List<SelectListItem> GetGender()
        {
            List<SelectListItem> result = new List<SelectListItem>();

            SelectListItem male = new SelectListItem();
            male.Text = "Male";
            male.Value = Constant.Gender.Male;

            result.Add(male);

            SelectListItem female = new SelectListItem();
            female.Text = "Female";
            female.Value = Constant.Gender.Female;

            result.Add(female);

            return result;
        }

        public static List<SelectListItem> GetRole()
        {
            List<SelectListItem> result = new List<SelectListItem>();

            SelectListItem author = new SelectListItem();
            author.Text = Constant.Role.Author;
            author.Value = Constant.Role.Author;

            result.Add(author);

            SelectListItem participant = new SelectListItem();
            participant.Text = Constant.Role.Participant;
            participant.Value = Constant.Role.Participant;

            result.Add(participant);

            return result;
        }

        public static List<SelectListItem> GetCountryList()
        {
            List<SelectListItem> result = new List<SelectListItem>();
            foreach (var cultureInfo in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
            {
                var regionInfo = new RegionInfo(cultureInfo.Name);
                if (!result.Any(c => c.Text.Equals(regionInfo.EnglishName)))
                {
                    SelectListItem female = new SelectListItem();
                    female.Text = regionInfo.EnglishName;
                    female.Value = regionInfo.TwoLetterISORegionName.ToLower();
                    result.Add(female);
                }
            }

            result = result.OrderBy(c => c.Text).ToList();

            return result;
        }

        public static string CreatePasswordHash(string pwd, string salt)
        {
            var saltAndPwd = String.Concat(pwd, salt);
            var hashedPwd = FormsAuthentication.HashPasswordForStoringInConfigFile(saltAndPwd, "sha1");

            return hashedPwd;
        }

        public static string CreateSalt(int size)
        {
            //Generate a cryptographic random number.
            var rng = new RNGCryptoServiceProvider();
            var buff = new byte[size];
            rng.GetBytes(buff);

            // Return a Base64 string representation of the random number.
            return Convert.ToBase64String(buff);
        }

        public static string GenerateRandomGuidPassword()
        {
            return Guid.NewGuid().ToString();
        }
    }
}