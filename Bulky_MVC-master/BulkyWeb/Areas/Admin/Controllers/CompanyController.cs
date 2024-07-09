using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult Index()
        {
            List<Company> objCompanyList = _unitOfWork.Company.GetAll().ToList();
            return Ok(objCompanyList);
        }

        [HttpGet("{id}")]
        public IActionResult Upsert(int? id)
        {
            if (id == null || id == 0)
            {
                // Create
                return Ok(new Company());
            }
            else
            {
                // Update
                Company companyObj = _unitOfWork.Company.Get(u => u.Id == id);
                if (companyObj == null)
                {
                    return NotFound(new { success = false, message = "Company not found" });
                }
                return Ok(companyObj);
            }
        }

        [HttpPost]
        public IActionResult Upsert([FromBody] Company companyObj)
        {
            if (ModelState.IsValid)
            {
                if (companyObj.Id == 0)
                {
                    _unitOfWork.Company.Add(companyObj);
                }
                else
                {
                    _unitOfWork.Company.Update(companyObj);
                }

                _unitOfWork.Save();
                return Ok(new { success = true, message = "Company created/updated successfully" });
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            List<Company> objCompanyList = _unitOfWork.Company.GetAll().ToList();
            return Ok(new { data = objCompanyList });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var companyToBeDeleted = _unitOfWork.Company.Get(u => u.Id == id);
            if (companyToBeDeleted == null)
            {
                return NotFound(new { success = false, message = "Error while deleting" });
            }

            _unitOfWork.Company.Remove(companyToBeDeleted);
            _unitOfWork.Save();

            return Ok(new { success = true, message = "Delete Successful" });
        }
    }
}
