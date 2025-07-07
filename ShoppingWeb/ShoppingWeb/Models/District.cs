using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ShoppingWeb.Models;

public partial class District
{
    [JsonProperty("DistrictID")]
    public int Id { get; set; }
    [JsonProperty("DistrictName")]
    public string Name { get; set; } = null!;

    [JsonProperty("ProvinceID")]
    public int ProvinceId { get; set; }

    public virtual Province Province { get; set; } = null!;

    public virtual ICollection<Ward> Wards { get; set; } = new List<Ward>();
}
