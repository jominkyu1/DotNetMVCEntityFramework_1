using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LocalEF6.Models
{
    public class OfficeAssignment //강사 사무실
    {
        //EF 명명규칙상 자동으로 기본키 인식하지 못하므로 명시적으로 지정
        //OneToOne 관계이므로 주종관계 설정 (이 클래스가 종)
        //사무실은 반드시 지정된 강사가 있어야 함
        [Key]
        [ForeignKey("Instructor")]
        public int InstructorID { get; set; }

        [StringLength(50)]
        [Display(Name = "Office Location")]
        public string Location { get; set; }

        public virtual Instructor Instructor { get; set; }
    }
}