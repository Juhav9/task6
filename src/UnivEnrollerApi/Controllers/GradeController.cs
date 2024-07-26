using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Internal;
using Microsoft.EntityFrameworkCore;
using UnivEnrollerApi.Data;

namespace UnivEnrollerApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GradeController : Controller
    {
        private readonly UnivEnrollerContext _context;

        public GradeController(UnivEnrollerContext context)
        {
            _context = context;
        }

        [HttpPut]
        public async Task<ActionResult> UpdateStudentGrade(GradeUpdate gu)
        {
            var enrollment = await _context.Enrollments
                                     .Where(e=>e.StudentId==gu.StudentId)
                                     .Where(e=>e.CourseId==gu.CourseId)
                                     .FirstOrDefaultAsync();
            if (enrollment == null) return NotFound();
            if(enrollment.Grade!=null&& !gu.Override) return Conflict(
                          $"Has existing value, will not make changes");

            enrollment.Grade = gu.Grade;
            enrollment.GradingDate = gu.GradingDate;

            _context.Enrollments.Update(enrollment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
