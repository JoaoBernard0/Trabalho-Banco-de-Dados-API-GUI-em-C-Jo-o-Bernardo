using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventoryApi.Data;
using InventoryApi.DTOs;
using InventoryApi.Models;

namespace InventoryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly InventoryContext _db;
        public ProductsController(InventoryContext db) { _db = db; }

        // GET /api/products?name=foo
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll([FromQuery] string? name)
        {
            var q = _db.Products.Include(p => p.Category).AsQueryable();
            if (!string.IsNullOrWhiteSpace(name))
                q = q.Where(p => p.Name.Contains(name));

            var list = await q.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                CategoryId = p.CategoryId,
                CategoryName = p.Category != null ? p.Category.Name : null,
                CreatedAt = p.CreatedAt
            }).ToListAsync();

            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductDto>> Get(int id)
        {
            var p = await _db.Products.Include(x => x.Category).FirstOrDefaultAsync(x => x.Id == id);
            if (p == null) return NotFound();

            return Ok(new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.Name,
                CreatedAt = p.CreatedAt
            });
        }

        [HttpPost]
        public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var cat = await _db.Categories.FindAsync(dto.CategoryId);
            if (cat == null) return UnprocessableEntity(new { error = "Category not found." });

            var prod = new Product
            {
                Name = dto.Name,
                Price = dto.Price,
                CategoryId = dto.CategoryId
            };

            _db.Products.Add(prod);
            await _db.SaveChangesAsync();

            var result = new ProductDto
            {
                Id = prod.Id,
                Name = prod.Name,
                Price = prod.Price,
                CategoryId = prod.CategoryId,
                CategoryName = cat.Name,
                CreatedAt = prod.CreatedAt
            };

            return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var prod = await _db.Products.FindAsync(id);
            if (prod == null) return NotFound();

            var cat = await _db.Categories.FindAsync(dto.CategoryId);
            if (cat == null) return UnprocessableEntity(new { error = "Category not found." });

            prod.Name = dto.Name;
            prod.Price = dto.Price;
            prod.CategoryId = dto.CategoryId;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var prod = await _db.Products.FindAsync(id);
            if (prod == null) return NotFound();

            _db.Products.Remove(prod);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
