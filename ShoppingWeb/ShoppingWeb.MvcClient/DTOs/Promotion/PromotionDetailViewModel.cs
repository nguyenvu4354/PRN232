using ShoppingWeb.MvcClient.DTOs.Product;

namespace ShoppingWeb.MvcClient.DTOs.Promotion
{
    public class PromotionDetailViewModel
    {
        public PromotionResponseDTO Promotion { get; set; }
        public List<ProductResponseDTO> Products { get; set; }
    }
}
