using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Design;
using System.Linq;
using System.Web;

namespace LocalEF6.Models
{
    public class Student : Person
    {
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name="Enrollment Date")]
        public DateTime EnrollmentDate { get; set; }

        [Display(Name = "Enrollments List")]
        public virtual ICollection<Enrollment> Enrollments { get; set; }
    }
}