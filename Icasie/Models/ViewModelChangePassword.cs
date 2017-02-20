using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Icasie.Models
{
    public class ViewModelChangePassword
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name="New Password")]
        public string NewPassword { get; set; }

        [Compare("NewPassword")]
        [DataType(DataType.Password)]
        [Display(Name = "ReType New Password")]
        public string RetypePassword { get; set; }
    }
}