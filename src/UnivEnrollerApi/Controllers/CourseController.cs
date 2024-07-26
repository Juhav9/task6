using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UnivEnrollerApi.Data;

namespace UnivEnrollerApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CourseController : Controller
    {
        private readonly UnivEnrollerContext _context;

        public CourseController(UnivEnrollerContext context)
        {
            _context = context;
        }
        [HttpPut]
        [Route("{id:int}")]
        public async Task<ActionResult> UpDateCourse(int id, CourseDTO course)
        {
            if (String.IsNullOrEmpty(course.name))
            {
                return BadRequest();
            }
            var c = await _context.Courses.FindAsync(id);
            if (c == null) return NotFound();

            c.Name = course.name;
            _context.Courses.Update(c);
            await _context.SaveChangesAsync(); 
            return NoContent();
        }
        [HttpGet]
        [Route("{id:int}")]
        public async Task<object> GetCourseById(int id)
        {
            return await _context.Courses.FindAsync(id);
        }
    }
}
