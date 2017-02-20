//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Icasie.Repositories
{
    using System;
    using System.Collections.Generic;
    
    public partial class Participant
    {
        public int ParticipantId { get; set; }
        public int UserId { get; set; }
        public int ConferenceId { get; set; }
        public byte[] Payment { get; set; }
        public string PaymentFileName { get; set; }
        public System.DateTime PaymentDate { get; set; }
        public string Number { get; set; }
        public string PaymentStatus { get; set; }
        public decimal TotalPayment { get; set; }
    
        public virtual Conference Conference { get; set; }
        public virtual User User { get; set; }
    }
}