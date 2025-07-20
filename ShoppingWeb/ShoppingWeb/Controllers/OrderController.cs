using Microsoft.AspNetCore.Mvc;
using ShoppingWeb.DTOs;
using ShoppingWeb.Models;
using ShoppingWeb.Services.Interface;
using ShoppingWeb.Services.ThirdParty;
using System.Security.Claims;

namespace ShoppingWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : Controller
    {
        private readonly ICartService _cartService;
        public OrderController(ICartService cartService)
        {
            _cartService = cartService;
        }
        [HttpPost("place-order")]
        public async Task<IActionResult> PlaceOrder([FromBody] int cartId)
        {
            //var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            //{
            //    return Unauthorized("User not authenticated.");
            //}
            if(cartId <= 0)
            {
                return BadRequest("Invalid cart ID.");
            }
            var result = await _cartService.CreateOrder(cartId);
            return Ok(result);
        }

        [HttpPost("create-order")]
        public async Task<IActionResult> CreateOrder([FromBody] ToOrderDTO toOrderDTO)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized("User not authenticated.");
            }
            toOrderDTO.UserId = userId;
            if (toOrderDTO == null || !ModelState.IsValid)
            {
                return BadRequest("Invalid order data.");
            }
            try
            {
                await _cartService.ToOrderAsync(toOrderDTO);
                return Ok("Order created successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("order-status/{cartId}")]
        public async Task<IActionResult> GetOrderStatus(int cartId)
        {
            if (cartId <= 0)
            {
                return BadRequest("Invalid cart ID.");
            }
            try
            {
                var orderStatus = await _cartService.GetOrderStatus(cartId);
                return Ok(orderStatus);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("shipping-fee")]
        public async Task<IActionResult> GetShippingFee(string wardCode, int districtId, int weight)
        {
            if (string.IsNullOrEmpty(wardCode) || districtId <= 0 || weight <= 0)
            {
                return BadRequest("Invalid shipping details.");
            }
            try
            {
                var shippingFee = await _cartService.GetShippingFee(wardCode, districtId, weight);
                return Ok(shippingFee);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("payment-info")]
        public async Task<IActionResult> GetPaymentInfo(int cartid)
        {
            //var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            //{
            //    return Unauthorized("User not authenticated.");
            //}
            try
            {
                var paymentInfo = await _cartService.GetPaymentInfo(cartid);
                return Ok(paymentInfo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPost("create-payment")]
        public async Task<IActionResult> CreatePayment([FromBody] int cartId)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized("User not authenticated.");
            }
            try
            {
                var response = await _cartService.CreatePayment(cartId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("provinces")]
        public async Task<IActionResult> GetProvinces()
        {
            try
            {
                var provinces = await _cartService.GetProvinces();
                return Ok(provinces);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("districts/{provinceId}")]
        public async Task<IActionResult> GetDistricts(int provinceId)
        {
            if (provinceId <= 0)
            {
                return BadRequest("Invalid province ID.");
            }
            try
            {
                var districts = await _cartService.GetDistricts(provinceId);
                return Ok(districts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("wards/{districtId}")]
        public async Task<IActionResult> GetWards(int districtId)
        {
            if (districtId <= 0)
            {
                return BadRequest("Invalid district ID.");
            }
            try
            {
                var wards = await _cartService.GetWards(districtId);
                return Ok(wards);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("all-addresses")]
        public async Task<IActionResult> GetAllAddresses()
        {
            try
            {
                var provinces = await _cartService.GetProvinces();
                var districts = new List<District>();
                var wards = new List<Ward>();
                foreach (var province in provinces)
                {
                    try
                    {
                        var districtList = await _cartService.GetDistricts(province.Id);
                        districts.AddRange(districtList);
                        foreach (var district in districtList)
                        {
                            bool result = new Random().NextDouble() < 0.75;
                            if (result)
                            {
                                Console.WriteLine($"Skipping wards for district {district.Id} due to random condition.");
                                continue;
                            }
                            try
                            {
                                var wardList = await _cartService.GetWards(district.Id);
                                wards.AddRange(wardList);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error fetching wards for district {district.Id}: {ex.Message}");
                                continue;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error fetching districts for province {province.Id}: {ex.Message}");
                        continue;
                    }
                }
                return Ok(new AllAddressesDTO { 
                    Provinces = provinces.Select(p => new ProvinceDTO
                    {
                        Id = p.Id,
                        Name = p.Name
                    }).ToList(),
                    Districts = districts.Select(d => new DistrictDTO
                    {
                        Id = d.Id,
                        Name = d.Name,
                        ProvinceId = d.ProvinceId
                    }).ToList(),
                    Wards = wards.Select(w => new WardDTO
                    {
                        Id = w.Id,
                        Name = w.Name,
                        DistrictId = w.DistrictId
                    }).ToList()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
