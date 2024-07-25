using System;
using System.Collections.Generic;

namespace Authorization_and_Authentication.Models;

public  class ProductImage
{
    public int Id { get; set; }

    public string? ProdName { get; set; }

    public string? ProdPrice { get; set; }

    public byte[]? ImgData { get; set; }
}
