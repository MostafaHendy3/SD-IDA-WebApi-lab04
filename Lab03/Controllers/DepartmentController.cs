using System.Collections.Generic;
using System.Linq;
using Lab03.Models;
using Lab03.Models.DTOs;
using Lab03.Repositories;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lab03.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAll")]
    public class DepartmentController : ControllerBase
    {
        private readonly IRepository<Department> _repo;

        public DepartmentController(IRepository<Department> repo)
        {
            _repo = repo;
        }

        [HttpGet]
        [Route("getAll")]
        public IActionResult GetAll()
        {
            var deps = _repo.GetAll();

            var dtos = deps.Select(d => new CustomDTO
                {
                    DepartmentName = d.Name,

                    StudentsNames = d.Students?.Select(s => s.Name).ToList() ?? new List<string>(),
                    CountStd = d.Students?.Count ?? 0,
                    Msg = d.Students?.Count > 1 ? "Overloaded" : "Normal"
            })
                .ToList();

            return Ok(dtos);
        }

        [HttpGet]
        [Route("getById/{id:int}")]
        public IActionResult GetById(int id)
        {
            var dep = _repo.GetById(id);
            if (dep == null)
                return NotFound();
            return Ok(dep);
        }

        [HttpPost]
        [Route("add")]
        public IActionResult Add(Department department  )
        {
            if (department == null)
                return BadRequest();
            
            _repo.Add(department);
            return CreatedAtAction(nameof(GetById), new { id = department.Id }, department);
        }

        [HttpPut]
        [Route("update")]
        public IActionResult Put(Department department)
        {
            if (department == null || _repo.GetById(department.Id) == null)
                return BadRequest();
            _repo.Update(department);
            return Ok(department);
        }

        [HttpDelete]
        [Route("delete/{id:int}")]
        public IActionResult Delete(int id)
        {
            var dep = _repo.GetById(id);
            if (dep == null)
                return NotFound();
            _repo.Remove(dep);
            return Ok(dep);
        }
    }
}
