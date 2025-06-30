using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingWeb.Models;
using ShoppingWeb.DTOs;
using System.Threading.Tasks;

namespace ShoppingWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandManagementController : ControllerBase
    {
        private readonly ShoppingWebContext _context;

        public BrandManagementController(ShoppingWebContext context)
        {
            _context = context;
        }

        // GET: api/brands
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BrandDto>>> GetBrands()
        {
            var brands = await _context.Brands
                .Select(b => new BrandDto
                {
                    Id = b.BrandId,
                    Name = b.BrandName,
                    Description = b.Description
                })
                .ToListAsync();

            return brands;
        }

        // GET: api/brands/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BrandDto>> GetBrand(int id)
        {
            var brand = await _context.Brands
                .Where(b => b.BrandId == id)
                .Select(b => new BrandDto
                {
                    Id = b.BrandId,
                    Name = b.BrandName,
                    Description = b.Description
                })
                .FirstOrDefaultAsync();

            if (brand == null)
            {
                return NotFound();
            }
            return brand;
        }

        // POST: api/brands
        [HttpPost]
        public async Task<ActionResult<BrandDto>> CreateBrand(BrandDto brandDto)
        {
            if (string.IsNullOrEmpty(brandDto.Name))
            {
                return BadRequest("BrandName is required.");
            }

            var brand = new Brand
            {
                BrandName = brandDto.Name,
                Description = brandDto.Description,
                CreatedAt = DateTime.Now
            };

            _context.Brands.Add(brand);
            await _context.SaveChangesAsync();

            brandDto.Id = brand.BrandId;
            return CreatedAtAction(nameof(GetBrand), new { id = brandDto.Id }, brandDto);
        }

        // PUT: api/brands/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBrand(int id, BrandDto brandDto)
        {
            if (id != brandDto.Id)
            {
                return BadRequest();
            }

            var brand = await _context.Brands.FindAsync(id);
            if (brand == null)
            {
                return NotFound();
            }

            brand.BrandName = brandDto.Name;
            brand.Description = brandDto.Description;
            brand.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/brands/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBrand(int id)
        {
            var brand = await _context.Brands.FindAsync(id);
            if (brand == null)
            {
                return NotFound();
            }

            _context.Brands.Remove(brand);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}