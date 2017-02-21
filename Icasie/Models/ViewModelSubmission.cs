using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Icasie.Models
{
    public class ViewModelSubmission
    {
        public int SubmissionId { get; set; }

        public int UserId { get; set; }

        public string AuthorName { get; set; }

        public int ConferenceId { get; set; }

        [Required]
        public string Title { get; set; }

        public string FullPaperStatus { get; set; }

        public Byte[] FullPaper { get; set; }

        public string FullPaperFileName { get; set; }

        public DateTime? FullPaperDate { get; set; }

        public Byte[] Payment { get; set; }

        public string PaymentFileName { get; set; }

        public DateTime? PaymentDate { get; set; }

        public string Comment { get; set; }

        public int ProofReaderId { get; set; }

        public string ProofReaderName { get; set; }

        public Byte[] ProofReaderResult { get; set; }

        public string ProofReaderResultFileName { get; set; }

        public DateTime ProofReadingDate { get; set; }

        public string ProofReadingDateString { get; set; }

        public int FormatCheckerId { get; set; }

        public string FormatCheckerName { get; set; }

        public Byte[] FormatCheckingResult { get; set; }

        public string FormatCheckingResultFileName { get; set; }

        public DateTime FormatCheckingDate { get; set; }

        public string FormatCheckingDateString { get; set; }

        public int ReviewedBy { get; set; }

        public string ReviewerName { get; set; }

        public DateTime? ReviewDate { get; set; }

        public string ReviewDateString { get; set; }

        public Byte[] FullPaperReview { get; set; }

        public string FullPaperReviewFileName { get; set; }

        public Byte[] FullPaperReview2 { get; set; }

        public string FullPaperReviewFileName2 { get; set; }

        public Byte[] FullPaperReview3 { get; set; }

        public string FullPaperReviewFileName3 { get; set; }

        [Display(Name = "Sub Theme")]
        public int SubThemesId { get; set; }

        public string SubThemes { get; set; }

        public int PaymentType { get; set; }

        public string PaymentTypeString { get; set; }

        [Display(Name = "Co Authors")]
        public int CoAuthors { get; set; }

        public decimal TotalPayment { get; set; }

        public DateTime? PaymentVerificationDate { get; set; }

        public int SeminarType { get; set; }

        public bool TermsAgreement { get; set; }

        //additional properties
        public string Institution { get; set; }

        public string Number { get; set; }
    }
}