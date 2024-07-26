using System.ComponentModel.DataAnnotations;

namespace UnivEnrollerApi.Data
{
    public class UniversityDTO
    {
        public int? id {  get; set; }
        [Required]
        public string name { get; set; }
    }
}
