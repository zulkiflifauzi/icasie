using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Icasie.Models
{
    public class ViewModelConference
    {
        public int ConferenceId { get; set; }

        [Required]
        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        [Required]
        public string StartDateString { get; set; }

        public DateTime EndDate { get; set; }

        [Required]
        public string EndDateString { get; set; }

        public DateTime PaperSubStartDate { get; set; }

        [Required]
        public string PaperSubStartDateString { get; set; }

        public DateTime PaperSubEndDate { get; set; }

        [Required]
        public string PaperSubEndDateString { get; set; }
        
        public bool PaperSubActive { get; set; }

        public List<ViewModelSubmission> Submissions { get; set; }

        public List<ViewModelParticipant> Participants { get; set; }
    }
}