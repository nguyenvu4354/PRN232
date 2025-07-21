using Microsoft.AspNetCore.Mvc;
using ShoppingWeb.Models;
using ShoppingWeb.DTOs;
using ShoppingWeb.Services.Interface;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace ShoppingWeb.Controllers
{
    [Authorize(Roles = "STAFF")]
    [Route("api/[controller]")]
    [ApiController]
    public class BrandManagementController : ControllerBase
    {
        private readonly IBrandService _brandService;

        public BrandManagementController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BrandDto>>> GetBrands()
        {
            var brands = await _brandService.GetBrandsAsync();
            var result = new List<BrandDto>();
            foreach (var b in brands)
            {
                result.Add(new BrandDto { Id = b.BrandId, Name = b.BrandName, Description = b.Description, IsDisabled = b.IsDisabled });
            }
            return result;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BrandDto>> GetBrand(int id)
        {
            var brand = await _brandService.GetBrandByIdAsync(id);
            if (brand == null) return NotFound();
            return new BrandDto { Id = brand.BrandId, Name = brand.BrandName, Description = brand.Description, IsDisabled = brand.IsDisabled };
        }

        [HttpPost]
        public async Task<ActionResult<BrandDto>> CreateBrand(BrandDto brandDto)
        {
            if (string.IsNullOrEmpty(brandDto.Name)) return BadRequest("BrandName is required.");
            var brand = new Brand { BrandName = brandDto.Name, Description = brandDto.Description };
            var created = await _brandService.CreateBrandAsync(brand);
            brandDto.Id = created.BrandId;
            return CreatedAtAction(nameof(GetBrand), new { id = brandDto.Id }, brandDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBrand(int id, BrandDto brandDto)
        {
            if (id != brandDto.Id) return BadRequest();
            var brand = new Brand { BrandId = brandDto.Id, BrandName = brandDto.Name, Description = brandDto.Description };
            var updated = await _brandService.UpdateBrandAsync(brand);
            if (updated == null) return NotFound();
            return NoContent();
        }

        [HttpPut("disable/{id}")]
        public async Task<IActionResult> DisableBrand(int id, [FromQuery] bool disable)
        {
            var brand = await _brandService.GetBrandByIdAsync(id);
            if (brand == null) return NotFound();
            brand.IsDisabled = disable;
            var updated = await _brandService.UpdateBrandAsync(brand);
            if (updated == null) return NotFound();
            var dto = new BrandDto { Id = brand.BrandId, Name = brand.BrandName, Description = brand.Description, IsDisabled = brand.IsDisabled };
            return Ok(dto);
        }
    }
}