using System.ComponentModel.DataAnnotations;

namespace UnivEnrollerApi.Data
{
    public class GradeUpdate
    {
        public int StudentId {  get; set; }
        public int CourseId { get; set; }
        [Range(0,5)]
        public int Grade { get; set; }
        public DateTime? GradingDate {  get; set; }
        public bool Override {  get; set; }

    }
}
