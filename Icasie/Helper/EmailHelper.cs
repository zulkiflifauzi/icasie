using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace Icasie.Helper
{
    public class EmailHelper
    {
        public static string SendEmailNewUser(string emailTo, string fullName, string password, string url)
        {
            using (var client = new SmtpClient(ConfigurationManager.AppSettings["SmtpServerHost"], int.Parse(ConfigurationManager.AppSettings["SmtpServerPort"])))
            {

                client.EnableSsl = false;
                client.UseDefaultCredentials = false;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                
                NetworkCredential basicCredential = new NetworkCredential(ConfigurationManager.AppSettings["SmtpUser"], ConfigurationManager.AppSettings["SmtpPassword"]);
                client.Credentials = basicCredential;
                var msg = new MailMessage { From = new MailAddress(ConfigurationManager.AppSettings["AdminSenderEmail"], ConfigurationManager.AppSettings["AdminSenderName"]) };
                msg.To.Add(new MailAddress(emailTo));
                msg.Priority = MailPriority.Normal;
                msg.Subject = "ICASIE Registration";

                // Mail Message Body
                var emailContent = new StringBuilder();
                emailContent.AppendLine("ICASIE user account information:");
                emailContent.AppendLine("=================================");
                emailContent.AppendLine();
                emailContent.AppendLine("Dear " + fullName + ",");
                emailContent.AppendLine();
                emailContent.AppendLine("You have been allocated a new user account on ICASIE.");
                emailContent.AppendLine();
                emailContent.AppendLine("To login please go to the following link:");
                emailContent.AppendLine();
                emailContent.AppendLine(url);
                emailContent.AppendLine();
                emailContent.AppendLine("Your login is :");
                emailContent.AppendLine();
                emailContent.AppendLine(emailTo);
                emailContent.AppendLine();
                emailContent.AppendLine("Below is your temporary password for ICASIE:");
                emailContent.AppendLine();
                emailContent.AppendLine(password);
                emailContent.AppendLine();
                emailContent.AppendLine("If you have any queries please email " + ConfigurationManager.AppSettings["AdminSenderEmail"]);
                emailContent.AppendLine();
                emailContent.AppendLine("Thanks");
                emailContent.AppendLine();
                emailContent.AppendLine("The ICASIE Team");

                msg.Body = emailContent.ToString();
                client.Send(msg);
                return emailTo;
            }
        }

        public static string SendEmailResetPasswordSuccess(string emailTo, string fullName, string password)
        {
            using (var client = new SmtpClient(ConfigurationManager.AppSettings["SmtpServerHost"], int.Parse(ConfigurationManager.AppSettings["SmtpServerPort"])))
            {

                client.EnableSsl = false;
                client.UseDefaultCredentials = false;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;

                NetworkCredential basicCredential = new NetworkCredential(ConfigurationManager.AppSettings["SmtpUser"], ConfigurationManager.AppSettings["SmtpPassword"]);
                client.Credentials = basicCredential;
                var msg = new MailMessage { From = new MailAddress(ConfigurationManager.AppSettings["AdminSenderEmail"], ConfigurationManager.AppSettings["AdminSenderName"]) };
                msg.To.Add(new MailAddress(emailTo));
                msg.Priority = MailPriority.Normal;
                msg.Subject = "ICASIE Password Change";

                // Mail Message Body
                var emailContent = new StringBuilder();
                emailContent.AppendLine("ICASIE user account information:");
                emailContent.AppendLine("=================================");
                emailContent.AppendLine();
                emailContent.AppendLine("Dear " + fullName + ",");
                emailContent.AppendLine();
                emailContent.AppendLine("You have been reset your password on ICASIE.");
                emailContent.AppendLine();
                emailContent.AppendLine("Your login is :");
                emailContent.AppendLine();
                emailContent.AppendLine(emailTo);
                emailContent.AppendLine();
                emailContent.AppendLine("Below is your temporary password for ICASIE:");
                emailContent.AppendLine();
                emailContent.AppendLine(password);
                emailContent.AppendLine();
                emailContent.AppendLine("If you have any queries please email " + ConfigurationManager.AppSettings["AdminSenderEmail"]);
                emailContent.AppendLine();
                emailContent.AppendLine("Thanks");
                emailContent.AppendLine();
                emailContent.AppendLine("The ICASIE Team");

                msg.Body = emailContent.ToString();
                client.Send(msg);
                return emailTo;
            }
        }


        public static string SendEmailPasswordChange(string emailTo, string fullName, string password)
        {
            using (var client = new SmtpClient(ConfigurationManager.AppSettings["SmtpServerHost"], int.Parse(ConfigurationManager.AppSettings["SmtpServerPort"])))
            {

                client.EnableSsl = false;
                client.UseDefaultCredentials = false;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;

                NetworkCredential basicCredential = new NetworkCredential(ConfigurationManager.AppSettings["SmtpUser"], ConfigurationManager.AppSettings["SmtpPassword"]);
                client.Credentials = basicCredential;
                var msg = new MailMessage { From = new MailAddress(ConfigurationManager.AppSettings["AdminSenderEmail"], ConfigurationManager.AppSettings["AdminSenderName"]) };
                msg.To.Add(new MailAddress(emailTo));
                msg.Priority = MailPriority.Normal;
                msg.Subject = "ICASIE Password Change";

                // Mail Message Body
                var emailContent = new StringBuilder();
                emailContent.AppendLine("ICASIE user account information:");
                emailContent.AppendLine("=================================");
                emailContent.AppendLine();
                emailContent.AppendLine("Dear " + fullName + ",");
                emailContent.AppendLine();
                emailContent.AppendLine("You have been changed your password on ICASIE.");
                emailContent.AppendLine();
                emailContent.AppendLine("Your new password on ICASIE is:");
                emailContent.AppendLine();
                emailContent.AppendLine(password);
                emailContent.AppendLine();
                emailContent.AppendLine("If you have any queries please email " + ConfigurationManager.AppSettings["AdminSenderEmail"]);
                emailContent.AppendLine();
                emailContent.AppendLine("Thanks");
                emailContent.AppendLine();
                emailContent.AppendLine("The ICASIE Team");

                msg.Body = emailContent.ToString();
                client.Send(msg);
                return emailTo;
            }
        }

        public static string SendEmailPasswordReset(string emailTo, string fullName, string token)
        {
            using (var client = new SmtpClient(ConfigurationManager.AppSettings["SmtpServerHost"], int.Parse(ConfigurationManager.AppSettings["SmtpServerPort"])))
            {

                client.EnableSsl = false;
                client.UseDefaultCredentials = false;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;

                NetworkCredential basicCredential = new NetworkCredential(ConfigurationManager.AppSettings["SmtpUser"], ConfigurationManager.AppSettings["SmtpPassword"]);
                client.Credentials = basicCredential;
                var msg = new MailMessage { From = new MailAddress(ConfigurationManager.AppSettings["AdminSenderEmail"], ConfigurationManager.AppSettings["AdminSenderName"]) };
                msg.To.Add(new MailAddress(emailTo));
                msg.Priority = MailPriority.Normal;
                msg.Subject = "ICASIE Password Reset";

                // Mail Message Body
                var emailContent = new StringBuilder();
                emailContent.AppendLine("ICASIE user account information:");
                emailContent.AppendLine("=================================");
                emailContent.AppendLine();
                emailContent.AppendLine("Dear " + fullName + ",");
                emailContent.AppendLine();
                emailContent.AppendLine("Please click below link to reset your password");
                emailContent.AppendLine();
                emailContent.AppendLine("www.icasieregistration.com/Login/Reset/" + token);
                emailContent.AppendLine();
                emailContent.AppendLine("If you have any queries please email " + ConfigurationManager.AppSettings["AdminSenderEmail"]);
                emailContent.AppendLine();
                emailContent.AppendLine("Thanks");
                emailContent.AppendLine();
                emailContent.AppendLine("The ICASIE Team");

                msg.Body = emailContent.ToString();
                client.Send(msg);
                return emailTo;
            }
        }

        public static string SendEmailNotification(List<string> emailTo, string mode, string param1, string param2)
        {
            try {
                using (var client = new SmtpClient(ConfigurationManager.AppSettings["SmtpServerHost"], int.Parse(ConfigurationManager.AppSettings["SmtpServerPort"])))
                {

                    client.EnableSsl = false;
                    client.UseDefaultCredentials = false;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;

                    NetworkCredential basicCredential = new NetworkCredential(ConfigurationManager.AppSettings["SmtpUser"], ConfigurationManager.AppSettings["SmtpPassword"]);
                    client.Credentials = basicCredential;
                    var msg = new MailMessage { From = new MailAddress(ConfigurationManager.AppSettings["AdminSenderEmail"], ConfigurationManager.AppSettings["AdminSenderName"]) };

                    foreach (var item in emailTo)
                    {
                        msg.To.Add(new MailAddress(item));
                    }
                    msg.Priority = MailPriority.Normal;

                    StringBuilder emailContent = new StringBuilder();

                    switch(mode)
                    {
                        case Constant.NotificationMode.NewSubmission:
                            emailContent = NewSubmission(msg, param1, param2);
                            break;
                        case Constant.NotificationMode.AssignReviewer:
                            emailContent = AssignReviewer(msg, param1, param2);
                            break;
                        case Constant.NotificationMode.AssignProofReader:
                            emailContent = AssignProofReader(msg, param1, param2);
                            break;
                    }                        

                    msg.Body = emailContent.ToString();
                    client.Send(msg);
                }            
            }
            catch(Exception ex)
            {
                return Constant.SendEmailFail;
            }

            return string.Empty;
        }

        //param1 = conference name
        //param2 = paper title
        private static StringBuilder NewSubmission(MailMessage msg, string param1, string param2)
        {
            msg.Subject = "New Paper Submission";

            // Mail Message Body
            var emailContent = new StringBuilder();
            emailContent.AppendLine("New Paper Submission:");
            emailContent.AppendLine("=================================");
            emailContent.AppendLine();
            emailContent.AppendLine("Hi, Admin.");
            emailContent.AppendLine();
            emailContent.AppendLine("An author has been submitted new paper on " + param1 + " conference with title " + param2);
            emailContent.AppendLine();
            emailContent.AppendLine("Please assign a Reviewer to review & a Proof Reader to proof read the paper");
            emailContent.AppendLine();
            emailContent.AppendLine("Thanks");
            emailContent.AppendLine();
            emailContent.AppendLine("The ICASIE Team");
            return emailContent;
        }

        //param1 = conference name
        //param2 = paper title
        private static StringBuilder AssignReviewer(MailMessage msg, string param1, string param2)
        {
            msg.Subject = "Paper Review Assignment";

            // Mail Message Body
            var emailContent = new StringBuilder();
            emailContent.AppendLine("Paper Review Assignment:");
            emailContent.AppendLine("=================================");
            emailContent.AppendLine();
            emailContent.AppendLine("Hi,");
            emailContent.AppendLine();
            emailContent.AppendLine("You have been assigned a paper to review on " + param1 + " conference with title " + param2);
            emailContent.AppendLine();
            emailContent.AppendLine("Thanks");
            emailContent.AppendLine();
            emailContent.AppendLine("The ICASIE Team");
            return emailContent;
        }

        private static StringBuilder AssignProofReader(MailMessage msg, string param1, string param2)
        {
            msg.Subject = "Paper Proof Reading Assignment";

            // Mail Message Body
            var emailContent = new StringBuilder();
            emailContent.AppendLine("Paper Proof Reading Assignment:");
            emailContent.AppendLine("=================================");
            emailContent.AppendLine();
            emailContent.AppendLine("Hi,");
            emailContent.AppendLine();
            emailContent.AppendLine("You have been assigned a paper to proof read on " + param1 + " conference with title " + param2);
            emailContent.AppendLine();
            emailContent.AppendLine("Thanks");
            emailContent.AppendLine();
            emailContent.AppendLine("The ICASIE Team");
            return emailContent;
        }
    }
}