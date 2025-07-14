using System;
using System.Collections.Generic;

namespace ShoppingWeb.Models;

public partial class Blog
{
    public int BlogId { get; set; }

    public string Title { get; set; } = null!;
    public string Thumbnail {  get; set; } = null!;

    public string Content { get; set; } = null!;

    public int AuthorId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User Author { get; set; } = null!;
}
