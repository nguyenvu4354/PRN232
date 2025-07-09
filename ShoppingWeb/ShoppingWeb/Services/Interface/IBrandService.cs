using ShoppingWeb.Models;
using ShoppingWeb.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShoppingWeb.Services.Interface
{
    public interface IBrandService
    {
        Task<IEnumerable<Brand>> GetBrandsAsync();
        Task<Brand> GetBrandByIdAsync(int id);
        Task<Brand> CreateBrandAsync(Brand brand);
        Task<Brand> UpdateBrandAsync(Brand brand);
        Task<bool> DeleteBrandAsync(int id);
    }
} 