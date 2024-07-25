using System;
using System.Collections.Generic;

namespace Authorization_and_Authentication.Models;

public partial class Stock
{
    public int ProdId { get; set; }

    public string? ProdName { get; set; }

    public string ProdPrice { get; set; } = null!;

    public byte[]? ImgData { get; set; }

    public string? Category { get; set; }

    public string? Quantity { get; set; }
}
