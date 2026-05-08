using Lab04.Models;
using Lab04.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Lab04.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IRepository<Student> _repo;
        private readonly IUnitOfWork _unitOfWork;

        public StudentController(IRepository<Student> repo, IUnitOfWork unitOfWork)
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
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
        public async Task<IActionResult> Add(Student student)
        {
            if (student == null || _repo.GetByName(student.SSN) != null)
            {
                return BadRequest();
            }
            _repo.Add(student);
            await _unitOfWork.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = student.SSN }, student);
        }

        //Update
        [HttpPut]
        [Route("update/")]
        public async Task<IActionResult> Put(Student student)
        {
            if (student == null || _repo.GetByName(student.SSN) == null)
            {
                return BadRequest();
            }
            _repo.Update(student);
            await _unitOfWork.SaveChangesAsync();
            return Ok(student);
        }

        //Delete
        [HttpDelete]
        [Route("delete/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var student = _repo.GetById(id);
            if (student == null)
            {
                return NotFound();
            }
            _repo.Remove(student);
            await _unitOfWork.SaveChangesAsync();
            return Ok(student);
        }
    }
}
