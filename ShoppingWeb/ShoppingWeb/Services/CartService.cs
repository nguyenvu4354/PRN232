using Microsoft.EntityFrameworkCore;
using ShoppingWeb.DTOs.Cart;
using ShoppingWeb.Models;
using ShoppingWeb.Services.Interface;

namespace ShoppingWeb.Services
{
    public class CartService : ICartService
    {
        private readonly ShoppingWebContext _context;
        public CartService(ShoppingWebContext context)
        {
            _context = context;
        }
        public async Task AddToCartAsync(int productId, int quantity, int userId)
        {
            // Retrieve the user's cart or create a new one if it doesn't exist  
            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId && c.IsCart);
            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    IsCart = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            // Check if the product exists  
            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == productId);
            if (product == null)
            {
                throw new ArgumentException("Product not found.");
            }

            // Check if the product is already in the cart  
            var orderDetail = await _context.OrderDetails.FirstOrDefaultAsync(od => od.CartId == cart.CartId && od.ProductId == productId);
            if (orderDetail != null)
            {
                // Update the quantity if the product is already in the cart  
                orderDetail.Quantity += quantity;
                orderDetail.UnitPrice = product.Price;
                _context.OrderDetails.Update(orderDetail);
            }
            else
            {
                // Add the product to the cart  
                orderDetail = new OrderDetail
                {
                    CartId = cart.CartId,
                    ProductId = productId,
                    Quantity = quantity,
                    UnitPrice = product.Price,
                    CreatedAt = DateTime.UtcNow
                };
                _context.OrderDetails.Add(orderDetail);
            }

            // Update the cart's total amount  
            cart.TotalAmount = await _context.OrderDetails
                .Where(od => od.CartId == cart.CartId)
                .SumAsync(od => od.Quantity * od.UnitPrice);
            cart.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task ClearCartAsync(int userId)
        {
            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId && c.IsCart);
            if (cart == null)
            {
                throw new ArgumentException("Cart not found.");
            }

            var orderDetails = _context.OrderDetails.Where(od => od.CartId == cart.CartId);
            _context.OrderDetails.RemoveRange(orderDetails);

            cart.TotalAmount = 0;
            cart.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task<int> GetCartItemCountAsync(int userId)
        {
            var cart = await _context.Carts.Include(c => c.OrderDetails).FirstOrDefaultAsync(c => c.UserId == userId && c.IsCart);
            if (cart == null)
            {
                return 0;
            }

            return cart.OrderDetails.Count;
        }

        public async Task<IEnumerable<OrderDetail>> GetCartItemsAsync(int userId)
        {
            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId && c.IsCart);
            if (cart == null)
            {
                return Enumerable.Empty<OrderDetail>();
            }

            return await _context.OrderDetails.Where(od => od.CartId == cart.CartId).ToListAsync();
        }

        public async Task<decimal> GetTotalPriceAsync(int userId)
        {
            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId && c.IsCart);
            if (cart == null)
            {
                return 0;
            }

            return await _context.OrderDetails.Where(od => od.CartId == cart.CartId).SumAsync(od => od.Quantity * od.UnitPrice);
        }

        public async Task RemoveFromCartAsync(int productId, int userId)
        {
            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId && c.IsCart);
            if (cart == null)
            {
                throw new ArgumentException("Cart not found.");
            }

            var orderDetail = await _context.OrderDetails.FirstOrDefaultAsync(od => od.CartId == cart.CartId && od.ProductId == productId);
            if (orderDetail == null)
            {
                throw new ArgumentException("Product not found in cart.");
            }

            _context.OrderDetails.Remove(orderDetail);

            cart.TotalAmount = await _context.OrderDetails.Where(od => od.CartId == cart.CartId).SumAsync(od => od.Quantity * od.UnitPrice);
            cart.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task ToOrderAsync(ToOrderDTO toOrderDTO)
        {
            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == toOrderDTO.UserId && c.IsCart);
            if (cart == null)
            {
                throw new ArgumentException("Cart not found.");
            }

            cart.IsCart = false;
            cart.OrderDate = DateTime.UtcNow;
            cart.UpdatedAt = DateTime.UtcNow;
            cart.ShippingAddress = toOrderDTO.ShippingAddress;
            cart.ProvinceId = toOrderDTO.ProvinceId;
            cart.DistrictId = toOrderDTO.DistrictId;
            cart.WardId = toOrderDTO.WardId;

            await _context.SaveChangesAsync();
        }

        public async Task<OrderDetail> UpdateCartItemAsync(int productId, int quantity, int userId)
        {
            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId && c.IsCart);
            if (cart == null)
            {
                throw new ArgumentException("Cart not found.");
            }

            var orderDetail = await _context.OrderDetails.FirstOrDefaultAsync(od => od.CartId == cart.CartId && od.ProductId == productId);
            if (orderDetail == null)
            {
                throw new ArgumentException("Product not found in cart.");
            }

            orderDetail.Quantity = quantity;

            _context.OrderDetails.Update(orderDetail);

            cart.TotalAmount = await _context.OrderDetails.Where(od => od.CartId == cart.CartId).SumAsync(od => od.Quantity * od.UnitPrice);
            cart.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return orderDetail;
        }
    }
}
