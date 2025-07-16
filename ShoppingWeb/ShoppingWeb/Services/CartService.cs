using Microsoft.EntityFrameworkCore;
using Net.payOS.Types;
using ShoppingWeb.Data;
using ShoppingWeb.DTOs;
using ShoppingWeb.Models;
using ShoppingWeb.Services.Interface;
using ShoppingWeb.Services.ThirdParty;

namespace ShoppingWeb.Services
{
    public class CartService : ICartService
    {
        private readonly ShoppingWebContext _context;
        private readonly IPayOS _payOS;
        private readonly IGHN _ghn;
        public CartService(ShoppingWebContext context, IPayOS payOS, IGHN ghn)
        {
            _context = context;
            _payOS = payOS;
            _ghn = ghn;
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
                    ShippingAddress = string.Empty,
                    TotalAmount = 0,
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

        public async Task<CreateOrderResponse> CreateOrder(int cartId)
        {
            var cart = await _context.Carts.Include(c => c.Ward).Include(c => c.District).Include(c => c.Province)
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.CartId == cartId && c.IsCart);
            if (cart == null)
            {
                throw new ArgumentException("Cart not found.");
            }
            var orderDetails = await _context.OrderDetails.Include(od => od.Product)
                .Where(od => od.CartId == cart.CartId)
                .ToListAsync();

            var orderRequest = new CreateOrderRequest
            {
                CODAmount = (int)cart.TotalAmount,
                ToAddress = cart.ShippingAddress,
                ToDistrictName = cart.District?.Name ?? string.Empty,
                ToProvinceName = cart.Province?.Name ?? string.Empty,
                ToWardName = cart.Ward?.Name ?? string.Empty,
                ToName = cart.User?.FullName ?? "Unknown User",
                ToPhone = cart.User?.Phone ?? "0000000000",
                Items = orderDetails.Select(od => new GHNItem
                {
                    Name = od.Product.ProductName,
                    Quantity = od.Quantity,
                }).ToArray(),
                Weight = orderDetails.Sum(od => od.Quantity * 50)
            };
            CreateOrderResponse response = await _ghn.CreateOrder(orderRequest);
            cart.OrderDate = DateTime.UtcNow;
            cart.IsCart = false;
            cart.UpdatedAt = DateTime.UtcNow;
            cart.OrderCode = response.OrderCode;
            await _context.SaveChangesAsync();
            return response;
        }

        public async Task<CreatePaymentResponse> CreatePayment(CreatePaymentRequest request)
        {
            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.CartId == request.Id && c.IsCart);
            if (cart == null)
            {
                throw new ArgumentException("Cart not found.");
            }
            var orderDetails = await _context.OrderDetails.Include(orderDetails => orderDetails.Product)
                .Where(od => od.CartId == cart.CartId)
                .ToListAsync();
            if (!orderDetails.Any())
            {
                throw new ArgumentException("Cart is empty.");
            }
            CreatePaymentRequest paymentRequest = new()
            {
                Amount = (int)cart.TotalAmount,
                Description = $"Order from user {cart.UserId} on {cart.OrderDate}",
                Id = cart.CartId,
                Items = new List<ItemData>()
            };
            CreatePaymentResponse response = await _payOS.CreatePayment(request);
            cart.IsCart = false;
            cart.UpdatedAt = DateTime.UtcNow;
            cart.PaymentCode = response.OrderCode.ToString();
            await _context.SaveChangesAsync();
            return response;
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

        public async Task<IEnumerable<CartItemDTO>> GetCartItemsAsync(int userId)
        {
            var cart = await _context.Carts.Include(c => c.OrderDetails).ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId && c.IsCart);
            if (cart == null)
            {
                return Enumerable.Empty<CartItemDTO>();
            }

            return cart.OrderDetails.Select(od => new CartItemDTO
            {
                ProductId = od.ProductId,
                ProductName = od.Product.ProductName,
                ProductImage = od.Product.ImageUrl,
                Price = od.UnitPrice,
                Quantity = od.Quantity
            }).ToList();
        }

        public async Task<List<District>> GetDistricts(int provinceId)
        {
            var districts = await _context.Districts
                .Where(d => d.ProvinceId == provinceId)
                .ToListAsync();
            if (districts == null || !districts.Any())
            {
                throw new ArgumentException("No districts found for the given province.");
            }
            return districts;
        }

        public async Task<OrderStatusResponse> GetOrderStatus(int cartId)
        {
            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.CartId == cartId && !c.IsCart);
            if (cart == null)
            {
                throw new ArgumentException("Order not found.");
            }
            var orderCode = cart.ShippingAddress.Split('#').LastOrDefault();
            return await _ghn.GetOrderStatus(orderCode);
        }

        public async Task<PaymentLinkInformation> GetPaymentInfo(int cartid)
        {
            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.CartId == cartid && !c.IsCart);
            if (cart == null)
            {
                throw new ArgumentException("Order not found.");
            }
            var PaymentCode = cart.PaymentCode;
            if (long.TryParse(PaymentCode, out long orderCodeLong) == false)
            {
                throw new ArgumentException("Invalid order code or no code.");
            }
            return await _payOS.GetPayment(orderCodeLong);
        }

        public async Task<List<Province>> GetProvinces()
        {
            var provinces = await _context.Provinces.ToListAsync();
            if (provinces == null || !provinces.Any())
            {
                throw new ArgumentException("No provinces found.");
            }
            return provinces;
        }

        public async Task<int> GetShippingFee(string wardCode, int districtId, int weight)
        {
            return await _ghn.GetServiceFee(wardCode, districtId, weight);
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

        public async Task<List<Ward>> GetWards(int districtId)
        {
            var wards = await _context.Wards
                .Where(w => w.DistrictId == districtId)
                .ToListAsync();
            if (wards == null || !wards.Any())
            {
                throw new ArgumentException("No wards found for the given district.");
            }
            return wards;
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

        public async Task UpdateCartItemAsync(int productId, int quantity, int userId)
        {
            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId && c.IsCart);
            if (cart == null)
            {
                throw new ArgumentException("Cart not found.");
            }

            var orderDetail = await _context.OrderDetails.Include(od => od.Product).FirstOrDefaultAsync(od => od.CartId == cart.CartId && od.ProductId == productId);
            if (orderDetail == null)
            {
                throw new ArgumentException("Product not found in cart.");
            }
            if(quantity <= 0)
            {
                throw new ArgumentException("Quantity must be greater than zero.");
            }
            if(orderDetail.Product.StockQuantity < quantity)
            {
                throw new ArgumentException("Insufficient stock for the requested quantity.");
            }
            orderDetail.Quantity = quantity;

            _context.OrderDetails.Update(orderDetail);

            cart.TotalAmount = await _context.OrderDetails.Where(od => od.CartId == cart.CartId).SumAsync(od => od.Quantity * od.UnitPrice);
            cart.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

        }
    }
}
