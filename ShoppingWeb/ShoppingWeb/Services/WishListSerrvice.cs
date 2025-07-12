using Microsoft.EntityFrameworkCore;
using ShoppingWeb.Data;
using ShoppingWeb.Models;
using ShoppingWeb.Services.Interface;

namespace ShoppingWeb.Services
{
    public class WishListSerrvice : IWishListService
    {
        private readonly ShoppingWebContext _context;
        public WishListSerrvice(ShoppingWebContext context)
        {
            _context = context;
        }
        public async Task<bool> AddToWishList(int userId, int productId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                return false; // User not found
            }
            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == productId);
            if (product == null)
            {
                return false; // Product not found
            }
            var existingWishlistItem = await _context.Wishlists
                .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);
            if (existingWishlistItem != null)
            {
                return false; // Item already in wishlist
            }
            var wishlistItem = new Wishlist
            {
                UserId = userId,
                ProductId = productId,
                CreatedAt = DateTime.UtcNow
            };
            _context.Wishlists.Add(wishlistItem);
            await _context.SaveChangesAsync();
            return true; // Successfully added to wishlist
        }

        public async Task<IEnumerable<Wishlist>> GetWishList(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            var wishList = await _context.Wishlists
                .Where(w => w.UserId == userId)
                .Include(w => w.Product)
                .ToListAsync();
            return wishList;
        }

        public async Task<bool> IsInWishList(int userId, int productId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == productId);
            if (product == null)
            {
                throw new Exception("Product not found");
            }
            var existingWishlistItem = _context.Wishlists
                .AnyAsync(w => w.UserId == userId && w.ProductId == productId);
            return await existingWishlistItem; // Returns true if item is in wishlist, false otherwise
        }

        public async Task<bool> RemoveFromWishList(int userId, int productId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == productId);
            if (product == null)
            {
                throw new Exception("Product not found");
            }
            var existingWishlistItem = await _context.Wishlists
                .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);
            if (existingWishlistItem == null)
            {
                return false; // Item not found in wishlist
            }
            _context.Wishlists.Remove(existingWishlistItem);
            await _context.SaveChangesAsync();
            return true; // Successfully removed from wishlist
        }
    }
}
