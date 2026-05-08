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
        private readonly IUnitOfWork _unitOfWork;

        public DepartmentController(IRepository<Department> repo, IUnitOfWork unitOfWork)
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
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
        public async Task<IActionResult> Add(DepartmentDto departmentdto)
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
            await _unitOfWork.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = dep.Id }, dep);
        }

        [HttpPut]
        [Route("update")]
        public async Task<IActionResult> Update(Department department)
        {
            if (department == null || _repo.GetById(department.Id) == null)
                return BadRequest();
            _repo.Update(department);
            await _unitOfWork.SaveChangesAsync();
            return Ok(department);
        }

        [HttpDelete]
        [Route("delete/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var dep = _repo.GetById(id);
            if (dep == null)
                return NotFound();
            _repo.Remove(dep);
            await _unitOfWork.SaveChangesAsync();
            return Ok(dep);
        }
    }
}
