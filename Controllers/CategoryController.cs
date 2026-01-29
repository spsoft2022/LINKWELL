using LinkwellProductionSystem.Data;
using LinkwellProductionSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LinkwellProductionSystem.Controllers
{
    [Route("category")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }


        public IActionResult Index()
        {
            // Optional: server-side admin check
            if (HttpContext.Session.GetString("Role") != "Admin")
                return Unauthorized();

            return View();
        }


        [HttpPost("create")]
        public IActionResult Create([FromBody] Category model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid data");

            model.CreatedOn = DateTime.Now;
            model.CreatedBy = "Admin"; // get from session later
            model.IsActive = true;

            _context.Category.Add(model);
            _context.SaveChanges();

            return Ok(new
            {
                success = true,
                message = "Category created successfully"
            });
        }

        [HttpGet("all")]
        public IActionResult GetAll()
        {
            var data = _context.Category
                               .OrderBy(x => x.CategoryName)
                               .ToList();

            return Ok(data);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var category = _context.Category.Find(id);

            if (category == null)
                return NotFound("Category not found");

            return Ok(category);
        }


        [HttpPut("toggle/{id}")]
        public IActionResult Toggle(int id, [FromBody] Category model)
        {
            var category = _context.Category.Find(id);

            if (category == null)
                return NotFound("Category not found");

            category.IsActive = model.IsActive;
            category.ModifiedBy = "Admin";
            category.ModifiedOn = DateTime.Now;

            _context.SaveChanges();

            return Ok(new
            {
                success = true,
                message = "Status updated successfully"
            });
        }


        [HttpPut("update/{id}")]
        public IActionResult Update(int id, [FromBody] Category model)
        {
            var category = _context.Category.Find(id);

            if (category == null)
                return NotFound("Category not found");

            category.CategoryCode = model.CategoryCode;
            category.CategoryName = model.CategoryName;
            category.Description = model.Description;
            category.IsActive = model.IsActive;

            category.ModifiedBy = "Admin";
            category.ModifiedOn = DateTime.Now;

            _context.SaveChanges();

            return Ok(new
            {
                success = true,
                message = "Category updated successfully"
            });
        }


        [HttpDelete("delete/{id}")]
        public IActionResult Delete(int id)
        {
            var category = _context.Category.Find(id);

            if (category == null)
                return NotFound("Category not found");

            category.IsActive = false;
            category.ModifiedOn = DateTime.Now;
            category.ModifiedBy = "Admin";

            _context.SaveChanges();

            return Ok(new
            {
                success = true,
                message = "Category deleted successfully"
            });
        }






        [HttpGet("get-categories")]
        public IActionResult GetCategories()
        {
              var result = _context.CategoryVM.FromSqlRaw("EXEC usp_Category_GetCategories").ToList();

                return Ok(new
                {
                    success = true,
                    data = result
                });
           
        }
    }

}
