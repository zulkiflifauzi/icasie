﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class IcasieEntities : DbContext
    {
        public IcasieEntities()
            : base("name=IcasieEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<Conference> Conferences { get; set; }
        public DbSet<Fee> Fees { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<Submission> Submissions { get; set; }
        public DbSet<SubTheme> SubThemes { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
