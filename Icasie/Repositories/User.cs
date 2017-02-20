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
    
    public partial class User
    {
        public User()
        {
            this.Participants = new HashSet<Participant>();
            this.Submissions = new HashSet<Submission>();
            this.Submissions1 = new HashSet<Submission>();
            this.Submissions2 = new HashSet<Submission>();
        }
    
        public int UserId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public string Role { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Institution { get; set; }
        public string PhoneNumber { get; set; }
    
        public virtual ICollection<Participant> Participants { get; set; }
        public virtual ICollection<Submission> Submissions { get; set; }
        public virtual ICollection<Submission> Submissions1 { get; set; }
        public virtual ICollection<Submission> Submissions2 { get; set; }
    }
}
