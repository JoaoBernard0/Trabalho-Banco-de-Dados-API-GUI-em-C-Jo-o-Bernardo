using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventoryApi.Data;
using InventoryApi.DTOs;
using InventoryApi.Models;

namespace InventoryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly InventoryContext _db;
        public CategoriesController(InventoryContext db) { _db = db; }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll()
        {
            var list = await _db.Categories
                .Select(c => new CategoryDto { Id = c.Id, Name = c.Name })
                .ToListAsync();
            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CategoryDto>> Get(int id)
        {
            var cat = await _db.Categories.FindAsync(id);
            if (cat == null) return NotFound();
            return Ok(new CategoryDto { Id = cat.Id, Name = cat.Name });
        }

        [HttpPost]
        public async Task<ActionResult<CategoryDto>> Create([FromBody] CategoryDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var exists = await _db.Categories.AnyAsync(c => c.Name == dto.Name);
            if (exists) return Conflict(new { error = "Category name must be unique." });

            var c = new Category { Name = dto.Name };
            _db.Categories.Add(c);
            await _db.SaveChangesAsync();

            dto.Id = c.Id;
            return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] CategoryDto dto)
        {
            if (id != dto.Id) return BadRequest();
            var cat = await _db.Categories.FindAsync(id);
            if (cat == null) return NotFound();

            var exists = await _db.Categories.AnyAsync(c => c.Name == dto.Name && c.Id != id);
            if (exists) return Conflict(new { error = "Category name must be unique." });

            cat.Name = dto.Name;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var cat = await _db.Categories.Include(c => c.Products).FirstOrDefaultAsync(c => c.Id == id);
            if (cat == null) return NotFound();
            if (cat.Products.Any()) return UnprocessableEntity(new { error = "Cannot delete category with products." });

            _db.Categories.Remove(cat);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
