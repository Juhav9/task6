using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using UnivEnrollerApi.Data;

namespace UnivEnrollerApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UniversityController : Controller
    {
        private readonly UnivEnrollerContext _context;

        public UniversityController(UnivEnrollerContext context) {
            _context = context;
        }
        [HttpGet]
        public async Task<IEnumerable<object>> GetAllUniversity()
        {
            return await _context.Universities
                                  .Select(u => new
                                  {
                                    Id = u.Id,
                                    Name = u.Name
                                  })
                                  .ToArrayAsync();
        }
        [HttpGet]
        [Route("{Id}")]
        public async Task<University> GetUniversityByID(int Id)
        {
            return await _context.Universities
                                 .Where(u=>u.Id==Id)
                                 .SingleOrDefaultAsync();
            
            
        }
        [HttpPost]
        public async Task<ActionResult<UniversityDTO>> CreateNewUniversity(UniversityDTO universityDTO)
        {
            var university = new University
            {
                Name = universityDTO.name
            };
            _context.Universities.Add(university);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUniversityByID),
                                   new { id = university.Id },
                                   university);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUniversity(int id)
        {
            var university = await _context.Universities
                                           .Where(u => u.Id == id)
                                           .Include(u => u.Courses)
                                           .FirstOrDefaultAsync();
            if(university == null) return NotFound();
            if(university.Courses.Count >0) return Conflict();

            _context.Universities.Remove(university);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        [Route("make-some-coffee")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status418ImATeapot)]
        public ActionResult MakeSomeCoffee()
        {
            return StatusCode(418);
        }
        private static UniversityDTO ItemUniversityDTO(University uni) =>
            new UniversityDTO
            {
                id = uni.Id,
                name = uni.Name,
            };
    }
}
