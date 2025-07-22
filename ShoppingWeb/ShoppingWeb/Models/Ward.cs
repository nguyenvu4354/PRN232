using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ShoppingWeb.Models;

public partial class Ward
{
    [JsonProperty("WardCode")]
    public int Id { get; set; }
    [JsonProperty("WardName")]
    public string Name { get; set; } = null!;
    [JsonProperty("DistrictID")]
    public int DistrictId { get; set; }
    public virtual District District { get; set; } = null!;
}
