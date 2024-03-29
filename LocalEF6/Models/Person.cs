using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LocalEF6.Models
{
    public class Person
    {
        public int ID { get; set; }

        [Required]
        [StringLength(15)]
        [Display(Name="이름")]
        public string LastName { get; set; }

        [Required]
        [StringLength(15, ErrorMessage = "최대 15자")]
        [Column("FirstName")]
        [Display(Name="성")]
        public string FirstMidName { get; set; }

        [Display(Name="Full Name")]
        public string FullName
        {
            get
            {
                return LastName + ", " + FirstMidName;
            }
        }

    }
}