using Lab01.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Lab01.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly UniDbContext _context;

        public StudentController(UniDbContext context) {
            _context = context;
         }

        //getAll
        [HttpGet]
        [Route("getAll")]
        public IActionResult GetAll()
        {
            var students = _context.Students.ToList();
            return Ok(students);
        }


        //getById
        [HttpGet]
        [Route("getById/{id:int}")]
        public IActionResult GetById(int id) {
            var student = _context.Students.Find(id);
            if (student == null)
            {
                return NotFound();
            }
            return Ok(student);
        }

        //getByName
        [HttpGet]
        [Route("getByName/{name:alpha}")]
        public IActionResult GetByName(string name) {
            
            var students = _context.Students.Where(s => s.Name.Contains(name)).ToList();
            if(students.IsNullOrEmpty()) {
                return NotFound();
            }
            return Ok(students);
        }
        //add
        [HttpPost]
        [Route("add")]
        public IActionResult Add(Student student) {
            if (student == null || _context.Students.Any(s => s.SSN == student.SSN))
            {
                return BadRequest();
            }
            _context.Students.Add(student);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetById), new { id = student.SSN }, student);
        }
        //Update
        [HttpPut]
        [Route("update/")]
        public IActionResult Put(Student student)
        {
            if (student == null || !_context.Students.Any(s => s.SSN == student.SSN))
            {
                return BadRequest();
            }
            _context.Students.Update(student);
            _context.SaveChanges();
            return Ok(student);
        }
        //Delete
        [HttpDelete]
        [Route("delete/{id:int}")]
        public IActionResult Delete(int id)
        {
            var student = _context.Students.Find(id);
            if (student == null)
            {
                return NotFound();
            }
            _context.Students.Remove(student);
            _context.SaveChanges();
            return Ok(student);
        }
        }
}
