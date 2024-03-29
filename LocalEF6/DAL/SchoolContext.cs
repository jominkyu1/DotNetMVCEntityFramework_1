using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Diagnostics;
using System.Linq;
using System.Web;
using LocalEF6.Models;

namespace LocalEF6.DAL
{
    //EF의 Context -> JPA의 Repository
    public class SchoolContext : DbContext
    {
        public SchoolContext() : base("SchoolContext")
        {
            // DB 쿼리 디버그출력
           // Database.Log = log => Debug.WriteLine(log);
           
        }

        public DbSet<Person> People { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<OfficeAssignment> OfficeAssignments { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //생성되는 Table명을 복수형이아닌 단수형으로 만듦 (ex: Students -> Student)
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            // ManyToMany 조인 테이블 구성 Course <-> Instructor
            modelBuilder.Entity<Course>()
                .HasMany(c => c.Instructors)
                .WithMany(i => i.Courses)
                .Map(t => t.MapLeftKey("CourseID").MapRightKey("InstructorID")
                    .ToTable("CourseInstructor"));

            //저장프로시저를 이용하여 CRUD 수행지시
            modelBuilder.Entity<Department>().MapToStoredProcedures();
        }
    }
}