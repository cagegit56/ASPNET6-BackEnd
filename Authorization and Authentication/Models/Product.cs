using System;
using System.Collections.Generic;

namespace Authorization_and_Authentication.Models;

public class Product
{
    public int ProdId { get; set; }

    public string? ProdName { get; set; }

    public string? ProdPrice { get; set; }

    public byte[]? ImgData { get; set; }

    public string? Category { get; set; }
}
