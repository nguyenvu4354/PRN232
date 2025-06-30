namespace ShoppingWeb.Models
{
    public class Ward
    {
        public int WardId { get; set; }
        public string Name { get; set; } = null!;
        public int DistrictId { get; set; }
        public virtual District District { get; set; } = null!;
    }
}
