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
    public class CategoryController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Category> objCategoryList = _unitOfWork.Category.GetAll().ToList();
            return Ok(objCategoryList);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var category = _unitOfWork.Category.Get(u => u.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            return Ok(category);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Category obj)
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                return BadRequest(new { success = false, message = "The DisplayOrder cannot exactly match the Name." });
            }

            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Add(obj);
                _unitOfWork.Save();
                return Ok(new { success = true, message = "Category created successfully" });
            }
            return BadRequest(ModelState);
        }

        [HttpPut("{id}")]
        public IActionResult Edit(int id, [FromBody] Category obj)
        {
            if (id != obj.Id)
            {
                return BadRequest(new { success = false, message = "Category ID mismatch" });
            }

            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Update(obj);
                _unitOfWork.Save();
                return Ok(new { success = true, message = "Category updated successfully" });
            }
            return BadRequest(ModelState);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var categoryFromDb = _unitOfWork.Category.Get(u => u.Id == id);
            if (categoryFromDb == null)
            {
                return NotFound(new { success = false, message = "Category not found" });
            }

            _unitOfWork.Category.Remove(categoryFromDb);
            _unitOfWork.Save();
            return Ok(new { success = true, message = "Category deleted successfully" });
        }

        [HttpGet("GetView")]
        public IActionResult Index()
        {
            List<Category> objCategoryList = _unitOfWork.Category.GetAll().ToList();
            return Ok(objCategoryList);
        }

        [HttpGet("CreateView")]
        public IActionResult Create()
        {
            return Ok();
        }

        [HttpPost("CreateView")]
        public IActionResult CreateView([FromBody] Category obj)
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                return BadRequest(new { success = false, message = "The DisplayOrder cannot exactly match the Name." });
            }

            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Add(obj);
                _unitOfWork.Save();
                return Ok(new { success = true, message = "Category created successfully" });
            }
            return BadRequest(ModelState);
        }

        [HttpGet("EditView/{id}")]
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound(new { success = false, message = "Category not found" });
            }
            Category? categoryFromDb = _unitOfWork.Category.Get(u => u.Id == id);
            if (categoryFromDb == null)
            {
                return NotFound(new { success = false, message = "Category not found" });
            }
            return Ok(categoryFromDb);
        }

        [HttpPost("EditView/{id}")]
        public IActionResult EditView(int id, [FromBody] Category obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Update(obj);
                _unitOfWork.Save();
                return Ok(new { success = true, message = "Category updated successfully" });
            }
            return BadRequest(ModelState);
        }

        [HttpGet("DeleteView/{id}")]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound(new { success = false, message = "Category not found" });
            }
            Category? categoryFromDb = _unitOfWork.Category.Get(u => u.Id == id);
            if (categoryFromDb == null)
            {
                return NotFound(new { success = false, message = "Category not found" });
            }
            return Ok(categoryFromDb);
        }

        [HttpPost("DeleteView/{id}")]
        public IActionResult DeleteView(int? id)
        {
            Category? obj = _unitOfWork.Category.Get(u => u.Id == id);
            if (obj == null)
            {
                return NotFound(new { success = false, message = "Category not found" });
            }
            _unitOfWork.Category.Remove(obj);
            _unitOfWork.Save();
            return Ok(new { success = true, message = "Category deleted successfully" });
        }
    }
}
