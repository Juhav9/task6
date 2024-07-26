using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UnivEnrollerApi.Data;

namespace UnivEnrollerApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StudentController : Controller
    {
        private readonly UnivEnrollerContext _context;
        public StudentController(UnivEnrollerContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<Student>> Get()
        {
            return await _context.Students.Select(s=>s).ToArrayAsync();
        }

        [HttpGet("{id:int}/courses")]
        public async Task<ActionResult<IEnumerable<object>>> GetStudentCourses(int id) 
        {
            Student? student = await _context.Students.FindAsync(id);
            if(student==null)return NotFound();
            return await _context.Enrollments
                                 .Where(e=>e.StudentId==student.Id)
                                 .Include(e=>e.Course)
                                 .Select(e=> new
                                 {
                                     id=e.Id,
                                     courseId=e.CourseId,
                                     course=e.Course.Name,
                                     grade=e.Grade,
                                     gradingDate=e.GradingDate
                                 })
                                 .ToArrayAsync();  
        }
        [HttpDelete("{studentId:int}/course/{courseId:int}")]
        public async Task<object> DeleteStudentEnrollment(int studentId, int courseId)
        {
            var enrollment = await _context.Enrollments
                                           .Where(c=>c.CourseId==courseId)
                                           .Where(c=>c.StudentId==studentId)
                                           .FirstOrDefaultAsync();
            
            if(enrollment == null)return NotFound();
            if (enrollment.Grade != null) return Conflict();

            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();  
            return NoContent();
        }
    }
}
