using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ShoppingWeb.Models;

public partial class Province
{
    [JsonProperty("ProvinceID")]
    public int Id { get; set; }
    [JsonProperty("ProvinceName")]
    public string Name { get; set; } = null!;

    public virtual ICollection<District> Districts { get; set; } = new List<District>();
}
