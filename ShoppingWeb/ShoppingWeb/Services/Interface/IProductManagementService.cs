using ShoppingWeb.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShoppingWeb.Services.Interface
{
    public interface IProductManagementService
    {
        Task<IEnumerable<Product>> GetProductsAsync();
        Task<Product> GetProductByIdAsync(int id);
        Task<Product> CreateProductAsync(Product product);
        Task<Product> UpdateProductAsync(Product product);
        Task<bool> DeleteProductAsync(int id);
    }
} 