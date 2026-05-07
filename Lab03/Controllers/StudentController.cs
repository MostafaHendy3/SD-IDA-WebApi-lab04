using Lab03.Models;
using Lab03.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Lab03.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private IRepository<Student> _repo;

        // private readonly UniDbContext _context;

        public StudentController(IRepository<Student> repo)
        {
            _repo = repo;
        }

        //getAll
        [HttpGet]
        [Route("getAll")]
        public IActionResult GetAll()
        {
            var students = _repo.GetAll();
            return Ok(students);
        }

        [HttpPost]
        [Route("TestDTO/{dto}")]
        //public IActionResult TestDTO(CustomDTO dto)
        //{
        //    //print the comming dto from the body of the request
        //    Console.WriteLine($"Id: {dto.Id}, Name: {dto.Name}");
        //    return Ok(dto);
        //}
        public IActionResult TestDTO(int dto, int age)
        {
            //print the comming dto from the body of the request
            Console.WriteLine($"Id: {dto}, age: {age}");
            return Ok(dto);
        }

        //getById
        [HttpGet]
        [Route("getById/{id:int}")]
        public IActionResult GetById(int id)
        {
            var student = _repo.GetById(id);
            if (student == null)
            {
                return NotFound();
            }
            return Ok(student);
        }

        //getByName
        [HttpGet]
        [Route("getByName/{name:alpha}")]
        public IActionResult GetByName(string name)
        {
            var students = _repo.GetByName(name);
            if (students == null)
            {
                return NotFound();
            }
            return Ok(students);
        }

        //add
        [HttpPost]
        [Route("add")]
        public IActionResult Add(Student student)
        {
            if (student == null || _repo.GetByName(student.SSN) != null)
            {
                return BadRequest();
            }
            _repo.Add(student);
            return CreatedAtAction(nameof(GetById), new { id = student.SSN }, student);
        }

        //Update
        [HttpPut]
        [Route("update/")]
        public IActionResult Put(Student student)
        {
            if (student == null || _repo.GetByName(student.SSN) == null)
            {
                return BadRequest();
            }
            _repo.Update(student);
            return Ok(student);
        }

        //Delete
        [HttpDelete]
        [Route("delete/{id:int}")]
        public IActionResult Delete(int id)
        {
            var student = _repo.GetById(id);
            if (student == null)
            {
                return NotFound();
            }
            _repo.Remove(student);
            return Ok(student);
        }
    }
}
