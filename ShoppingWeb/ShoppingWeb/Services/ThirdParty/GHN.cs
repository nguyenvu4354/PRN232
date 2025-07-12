using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ShoppingWeb.Models;
using System.Diagnostics;
using System.Text;

namespace ShoppingWeb.Services.ThirdParty
{
    public record CreateOrderRequest
    {
        [JsonProperty("to_name")]
        public required string ToName { get; set; }

        [JsonProperty("to_phone")]
        public required string ToPhone { get; set; }

        [JsonProperty("to_address")]
        public required string ToAddress { get; set; }

        [JsonProperty("to_ward_name")]
        public required string ToWardName { get; set; }

        [JsonProperty("to_district_name")]
        public required string ToDistrictName { get; set; }

        [JsonProperty("to_province_name")]
        public required string ToProvinceName { get; set; }

        [JsonProperty("cod_amount")]
        public required int CODAmount { get; set; }

        [JsonProperty("weight")]
        public required int Weight { get; set; }

        [JsonProperty("items")]
        public required GHNItem[] Items { get; set; }
    }
    public record GHNItem
    {
        [JsonProperty("name")]
        public required string Name { get; set; }

        [JsonProperty("quantity")]
        public required int Quantity { get; set; }
    }

    public record CreateOrderResponse
    {
        [JsonProperty("order_code")]
        public required string OrderCode { get; set; }

        [JsonProperty("expected_delivery_time")]
        public required string ExpectedDeliveryTime { get; set; }

        [JsonProperty("total_fee")]
        public required int TotalFee { get; set; }
    }
    public interface IGHN
    {
        public Task<List<Province>> GetProvinces();
        public Task<List<District>> GetDistricts(int provinceId);
        public Task<List<Ward>> GetWards(int districtId);
        public Task<int> GetServiceFee(string wardCode, int districtId, int weight);
        public Task<CreateOrderResponse> CreateOrder(CreateOrderRequest request);
        public Task<OrderStatusResponse> GetOrderStatus(string orderCode);
    }
    public class OrderStatusResponse
    {
        [JsonProperty("order_code")]
        public required string OrderCode { get; set; }
        [JsonProperty("status")]
        public required string Status { get; set; }
        [JsonProperty("updated_date")]
        public required DateTime UpdatedDate { get; set; }
        [JsonProperty("order_date")]
        public required DateTime OrderDate { get; set; }
        [JsonProperty("items")]
        public required List<GHNShippedItem> Items { get; set; }
    }

    public class GHNShippedItem
    {
        [JsonProperty("name")]
        public required string Name { get; set; }
        [JsonProperty("quantity")]
        public required int Quantity { get; set; }
        [JsonProperty("item_order_code")]
        public required string ItemOrderCode { get; set; }
        [JsonProperty("status")]
        public required string Status { get; set; }
    }
    public class GHN : IGHN
    {
        private readonly string _token;
        private readonly string _shopId;
        private HttpClient _client;

        private const string BASE_URL = "https://dev-online-gateway.ghn.vn/shiip/public-api";
        public GHN(IConfiguration configuration)
        {
            _token = configuration["GHN:Token"];
            _shopId = configuration["GHN:ShopId"];
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Add("Token", _token);
            _client.DefaultRequestHeaders.Add("ShopId", _shopId);
        }
        private async Task<JToken> DoRequest(Func<Task<string>> request)
        {
            Console.WriteLine("Requested");

            var responseStr = await request.Invoke();
            var responseTok = JToken.Parse(responseStr);

            var code = responseTok.Value<int>("code");
            var message = responseTok.Value<string>("message");
            if (code != 200)
                throw new Exception($"Error code: {code}, messgae: {message}");

            var data = responseTok["data"];
            Trace.Assert(data != null);
            return data;
        }
        private async Task<JToken> DoRequest(string url)
        {
            return await DoRequest(async () =>
            {
                HttpResponseMessage httpResp = await _client.GetAsync(url);
                /* 
                 * these guys are somewhat responsible and correctly indicate that a request has failed
                 * through http statuses through their api...
                if (!httpResp.IsSuccessStatusCode)
                    throw new Exception();
                */
                return await httpResp.Content.ReadAsStringAsync();
            });
        }
        private async Task<JToken> DoRequest(string url, object payload)
        {
            return await DoRequest(async () =>
            {
                string jsonRequest = JsonConvert.SerializeObject(payload);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                HttpResponseMessage httpResp = await _client.PostAsync(url, content);
                /*
                if (!httpResp.IsSuccessStatusCode)
                    throw new Exception();
                */
                return await httpResp.Content.ReadAsStringAsync();
            });
        }
        public async Task<List<Province>> GetProvinces()
        {
            string apiURL = BASE_URL + "/master-data/province";
            var response = await DoRequest(apiURL);
            return JsonConvert.DeserializeObject<List<Province>>(response.ToString()!)!;
        }
        public async Task<List<District>> GetDistricts(int provinceId)
        {
            string apiURL = BASE_URL + "/master-data/district";
            var response = await DoRequest(apiURL, new
            {
                province_id = provinceId,
            });
            return JsonConvert.DeserializeObject<List<District>>(response.ToString()!)
                ?? new List<District>();
        }
        public async Task<List<Ward>> GetWards(int districtId)
        {
            string apiURL = BASE_URL + "/master-data/ward";
            var response = await DoRequest(apiURL, new
            {
                district_id = districtId,
            });
            var data = response.ToString();
            var items = new List<Ward>();
            foreach (var item in JArray.Parse(data))
            {
                try
                {
                    var ward = item.ToObject<Ward>();
                    if (ward != null)
                    {
                        items.Add(ward);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing ward: {ex.Message}");
                }
            }
            return items;
        }

        public async Task<int> GetServiceFee(string wardCode, int districtId, int weight)
        {
            string apiURL = BASE_URL + "/v2/shipping-order/fee";
            var response = await DoRequest(apiURL, new
            {
                service_type_id = 2,
                to_ward_code = wardCode,
                to_district_id = districtId,
                weight = weight
            });

            return response.Value<int>("total");
        }
        public async Task<CreateOrderResponse> CreateOrder(CreateOrderRequest request)
        {
            string apiURL = BASE_URL + "/v2/shipping-order/create";
            var response = await DoRequest(apiURL, new
            {
                to_name = request.ToName,
                to_phone = request.ToPhone,
                to_address = request.ToAddress,
                to_ward_name = request.ToWardName,
                to_district_name = request.ToDistrictName,
                to_province_name = request.ToProvinceName,
                cod_amount = request.CODAmount,
                service_type_id = 2,
                payment_type_id = 2,
                weight = request.Weight,
                required_note = "KHONGCHOXEMHANG",
                items = request.Items
            });

            return JsonConvert.DeserializeObject<CreateOrderResponse>(response.ToString()!)!;
        }
        public async Task<OrderStatusResponse> GetOrderStatus(string orderCode)
        {
            string apiURL = BASE_URL + "/v2/shipping-order/detail";
            var response = await DoRequest(apiURL, new
            {
                order_code = orderCode
            });
            return JsonConvert.DeserializeObject<OrderStatusResponse>(response.ToString()!)!;
        }
    }
}
