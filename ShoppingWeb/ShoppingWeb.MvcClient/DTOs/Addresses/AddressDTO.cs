namespace ShoppingWeb.MvcClient.DTOs.Addresses
{
    public class ProvinceDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }
    public class DistrictDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int ProvinceId { get; set; }
    }
    public class WardDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int DistrictId { get; set; }
    }
    public class AllAddressDTO
    {
        public List<ProvinceDTO> Provinces { get; set; } = new List<ProvinceDTO>();
        public List<DistrictDTO> Districts { get; set; } = new List<DistrictDTO>();
        public List<WardDTO> Wards { get; set; } = new List<WardDTO>();
    }
}
