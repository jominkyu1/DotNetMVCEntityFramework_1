using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
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
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Course> Courses { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //생성되는 Table명을 복수형이아닌 단수형으로 만듦
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}