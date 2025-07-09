using ShoppingWeb.Models;

namespace ShoppingWeb.Services.Interface
{
    public interface IWishListService
    {
        public Task<bool> AddToWishList(int userId, int productId);
        public Task<bool> RemoveFromWishList(int userId, int productId);
        public Task<bool> IsInWishList(int userId, int productId);
        public Task<IEnumerable<Wishlist>> GetWishList(int userId);
    }
}
