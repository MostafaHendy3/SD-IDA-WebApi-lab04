using System.Collections.Generic;
using System.Linq;
using Lab04.Models;
using Lab04.Models.DTOs;
using Lab04.Repositories;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lab04.Controllers
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
                    Msg = d.Students?.Count > 1 ? "Overloaded" : "Normal",
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
        public IActionResult Add(DepartmentDto departmentdto)
        {
            if (departmentdto == null)
                return BadRequest();
            var dep = new Department
            {
                Name = departmentdto.DepName,
                Location = departmentdto.DepLocation,
                PhoneNumber = departmentdto.DepPhoneNumber,
                Manager = departmentdto.DepManager,
            };

            _repo.Add(dep);
            return CreatedAtAction(nameof(GetById), new { id = dep.Id }, dep);
        }

        [HttpPut]
        [Route("update")]
        public IActionResult Update(Department department)
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
