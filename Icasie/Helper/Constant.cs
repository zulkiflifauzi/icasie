using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Icasie.Helper
{
    public static class Constant
    {
        public const string DateFormat = "dd MMM yyyy";
        public const string DateFormatInput = "dd-MM-yyyy";
        public const string InitialComment = "Waiting for review";
        public const string InitialCommentProofRead = "Waiting for proof read";
        public const string NumberSuffix = "000";
        public const string Indonesia = "id";
        public const string SendEmailFail = "Sending email failed, please contact administrator";
        public const int ExchangeRate = 13000;

        public static class NotificationMode
        {
            public const string NewSubmission = "NewSubmission";
            public const string AssignReviewer = "AssignReviewer";
            public const string AssignProofReader = "AssignProofReader";
        }

        public static class EditPaperMode
        {
            public const string Review = "Review";
            public const string ProofRead = "ProofRead";
        }

        public static class Fees
        {
            public const string OverseasFull = "OF";
            public const string OverseasSeminar = "OS";
            public const string OverseasCoauthor = "OC";
            public const string OverseasParticipant = "OP";
            public const string IndonesianFull = "IF";
            public const string IndonesianSeminar = "IS";
            public const string IndonesianCoauthor = "IC";
            public const string IndonesianParticipant = "IP";
        }

        public static class PaymentType
        {
            public const int Full = 1;
            public const int Partial = 2;
            public const string FullString = "Conference & IOP Proceeding";
            public const string PartialString = "Conference & Icasie Proceeding";
        }

        public static class PaymentStatus
        {
            public const string Submitted = "Submitted";
            public const string Accepted = "Accepted";
            public const string Rejected = "Rejected";
        }

        public static class Role
        {
            public const string Reviewer = "Reviewer";
            public const string Author = "Author";
            public const string Administrator = "Administrator";
            public const string Participant = "Participant";
            public const string ProofReader = "Proof Reader";
            public const string FormatChecker = "Format Checker";
        }

        public static class CookieExpiration
        {
            public const int Normal = 525600;
            public const int RememberMe = 525600;
        }

        public static class FullPaperStatus
        {
            public const string Pending = "Pending";
            public const string Accepted = "Accepted";
            public const string FormatRevised = "Formatting Revised";
            public const string FormatChecked = "Formatting Checked";
            public const string Revised = "Revised";
            public const string Reviewed = "Reviewed";
            public const string ProofReadingRevised = "Proof Reading Revised";
            public const string Paid = "Paid";
            public const string Rejected = "Rejected";
            public const string PaymentVerified = "Payment Verified";
        }

        public static class DownloadType
        {
            public const string FullPaper = "FullPaper";
            public const string Payment = "Payment";
            public const string Review = "Review";
            public const string Review2 = "Review2";
            public const string Review3 = "Review3";
            public const string ParticipantPayment = "Participant";
            public const string ProofReading = "ProofReading";
            public const string FormatChecking = "FormatChecking";
        }

        public static class Gender
        {
            public const string Male = "Male";
            public const string Female = "Female";
        }
    }
}