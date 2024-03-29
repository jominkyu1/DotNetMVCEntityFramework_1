using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LocalEF6.DAL;
using LocalEF6.Models;
using LocalEF6.ViewModels;

namespace LocalEF6.Controllers
{
    public class InstructorController : Controller
    {
        private SchoolContext db = new SchoolContext();

        // GET: Instructor
        public ActionResult Index(int? id, int? courseID)
        {
            //var instructors = db.Instructors.Include(i => i.OfficeAssignment);

            //즉시로드가 반드시 필요한 상황은 아니나 성능 향상을 위해 사용함
            //항상 OfficeAssignment가 필요
            var viewModel = new InstructorIndexData();
            viewModel.Instructors = db.Instructors
                .Include(i => i.OfficeAssignment)
                .Include(i => i.Courses.Select(c => c.Department))
                .OrderBy(i => i.LastName);

            //Single() 호출시 컬렉션이 비어있거나 두개 이상의 항목이 존재하면 Throw Exception
            //Where(~).Single()을 .Single(i -> i.ID == id.Value)로 단축가능
            if (id != null)
            {
                ViewBag.InstructorID = id.Value; //for NULL Throw
                viewModel.Courses = viewModel.Instructors.Where(
                    i => i.ID == id.Value).Single().Courses;
            }

            if (courseID != null)
            {
                
                ViewBag.CourseID = courseID.Value;
                //지연로딩형태
                /*viewModel.Enrollments = viewModel.Courses.Where(
                    x => x.CourseID == courseID).Single().Enrollments;*/

                //즉시로딩형태
                //컬렉션 Collection, 단일엔티티 Reference
                var selectedCourse = viewModel.Courses.Where(x => x.CourseID == courseID).Single();
                db.Entry(selectedCourse).Collection(x => x.Enrollments).Load();
                foreach (var enrollment in selectedCourse.Enrollments)
                {
                    db.Entry(enrollment).Reference(x=>x.Student).Load();
                }

                viewModel.Enrollments = selectedCourse.Enrollments;
            }

            return View(viewModel);
        }

        // GET: Instructor/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Instructor instructor = db.Instructors.Find(id);
            if (instructor == null)
            {
                return HttpNotFound();
            }
            return View(instructor);
        }

        // GET: Instructor/Create
        public ActionResult Create()
        {
            var instructor = new Instructor
            {
                Courses = new List<Course>()
            }; //인스턴스 초기화를 하지않으면 널익셉션

            PopulateAssignedCourseData(instructor);
            return View();
        }

        // POST: Instructor/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(Include = "ID,LastName,FirstMidName,HireDate, OfficeAssignment")] Instructor instructor,
            string[] selectedCourses)
        {
            if (selectedCourses != null)
            {
                instructor.Courses = new List<Course>();
                foreach (var selected in selectedCourses)
                {
                    var courseToAdd = db.Courses.Find(int.Parse(selected));
                    instructor.Courses.Add(courseToAdd);
                }
            }

            if (ModelState.IsValid)
            {
                db.Instructors.Add(instructor);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            PopulateAssignedCourseData(instructor);
            return View(instructor);
        }

        // GET: Instructor/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            //Lazy Loading시 강사 테이블 + 강사오피스 테이블 쿼리 두개가 날라감
            //Instructor instructor = db.Instructors.Find(id); //LazyLoading
            

            //Eager Loading시 하나의 쿼리로 처리됨
            var instructor = db.Instructors
                .Include(i => i.OfficeAssignment)
                .Include(i => i.Courses)
                .Single(i => i.ID == id); 
            
            //Same Code
            /*var instructor = db.Instructors
                .Include(i => i.OfficeAssignment)
                .Where(i => i.ID == id)
                .Single();*/
            PopulateAssignedCourseData(instructor);
            if (instructor == null) return HttpNotFound();
            
            return View(instructor);
        }

        // POST: Instructor/Edit/5
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int? id, string[] selectedCourses)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var instructor = db.Instructors
                .Include(i => i.OfficeAssignment)
                .Include(i => i.Courses)
                .Single(i => i.ID == id);
            if (TryUpdateModel(instructor, "",
                    new string[] { "LastName", "FirstMidName", "HireDate", "OfficeAssignment" }))
                //갱신 허용할 속성에만 화이트리스트 추가. 오버포스팅 공격방지 가능
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(instructor.OfficeAssignment.Location))
                    {
                        instructor.OfficeAssignment = null;
                    }

                    UpdateInstructorCourses(selectedCourses, instructor);

                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (RetryLimitExceededException)
                {
                    ModelState.AddModelError("", "Failed Update Instructor");
                }
            }

            PopulateAssignedCourseData(instructor);
            return View(instructor);
        }

        // GET: Instructor/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Instructor instructor = db.Instructors.Find(id);
            if (instructor == null)
            {
                return HttpNotFound();
            }
            return View(instructor);
        }

        // POST: Instructor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            //Instructor instructor = db.Instructors.Find(id); //Lazy
            Instructor instructor = db.Instructors //Eager
                .Include(i => i.OfficeAssignment)
                .Single(i => i.ID == id);
            
            db.Instructors.Remove(instructor);

            var department = db.Departments
                .SingleOrDefault(d => d.DepartmentID == id); //학과 담당교수도 제거
            if (department.DepartmentID != null) department.InstructorID = null;

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private void PopulateAssignedCourseData(Instructor instructor)
        {
            var allCourses = db.Courses;
            var instructorCourses = new HashSet<int>(instructor.Courses.Select(c => c.CourseID));
            var viewModel = new List<AssignedCourseData>();
            foreach (var course in allCourses)
            {
                var assignedCourseData = new AssignedCourseData
                {
                    CourseID = course.CourseID,
                    Title = course.Title,
                    Assigned = instructorCourses.Contains(course.CourseID)
                };

                viewModel.Add(assignedCourseData);
            }

            ViewBag.Courses = viewModel;
        }

        private void UpdateInstructorCourses(string[] selectedCourses, Instructor instructorUpdate)
        {
            if (selectedCourses == null)
            {
                instructorUpdate.Courses = new List<Course>();
                return;
            }

            //중복허용X List
            var instructorCourses 
                = instructorUpdate.Courses.Select(c => c.CourseID).ToHashSet();
            var selectedList
                = selectedCourses.ToHashSet();

            foreach (var course in db.Courses)
            {
                if(selectedList.Contains(course.CourseID.ToString())) // 현재강의가 체크된 강의면
                {
                    if (!instructorCourses.Contains(course.CourseID)) // 체크된 강의가 새로 체크된상태면
                    {
                        instructorUpdate.Courses.Add(course); //추가
                    }
                }
                else // 현재강의가 체크가 안된 상태면
                {
                    if (instructorCourses.Contains(course.CourseID)) // 체크 해제된 강의가 이미 있는 강의면
                    {
                        instructorUpdate.Courses.Remove(course); //제거
                    }
                }
            }
        }
    }
}
