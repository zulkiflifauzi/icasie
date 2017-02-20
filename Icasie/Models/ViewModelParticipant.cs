using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Icasie.Models
{
    public class ViewModelParticipant
    {
        public int ParticipantId { get; set; }

        public string Number { get; set; }

        public int ConferenceId { get; set; }

        public int UserId { get; set; }

        public Byte[] Payment { get; set; }

        public string PaymentFileName { get; set; }

        public string PaymentDate { get; set; }

        public string PaymentStatus { get; set; }

        public string Name { get; set; }

        public decimal TotalPayment { get; set; }
    }
}