using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LocalEF6.DAL;
using LocalEF6.ViewModels;

namespace LocalEF6.Controllers
{
    public class HomeController : Controller
    {
        private SchoolContext db = new SchoolContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            //LINQ QueryMethod
            /*IQueryable<EnrollmentDateGroup> data =
                from student in db.Students
                group student by student.EnrollmentDate
                into dateGroup
                select new EnrollmentDateGroup()
                {
                    EnrollmentDate = dateGroup.Key,
                    StudentCount = dateGroup.Count()
                };*/

            //RAW SQL QUERY
            string query = "SELECT EnrollmentDate, Count(*) as StudentCount " +
                           "FROM Person " +
                           "WHERE Discriminator = 'Student' " +
                           "GROUP BY EnrollmentDate ";
            var data = db.Database.SqlQuery<EnrollmentDateGroup>(query);

            return View(data.ToList());
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}