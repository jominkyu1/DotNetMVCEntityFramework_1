using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Infrastructure.Design;
using System.Linq;
using System.Web;

namespace LocalEF6.Models
{
    public class Student
    {
        public int ID { get; set; }
        public string LastName { get; set; }
        public string FirstMidName { get; set; }
        public DateTime EnrollmentDate { get; set; }

        public string EmailAddress { get; set; }

        [Display(Name = "Enrollments List")]
        public virtual ICollection<Enrollment> Enrollments { get; set; }

    }
    
}