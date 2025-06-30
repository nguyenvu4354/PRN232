namespace ShoppingWeb.Models
{
    public class District
    {
        public int DistrictId { get; set; }
        public string Name { get; set; } = null!;
        public int ProvinceId { get; set; }
        public virtual Province Province { get; set; } = null!;
        public virtual ICollection<Ward> Wards { get; set; } = new List<Ward>();
    }
}
