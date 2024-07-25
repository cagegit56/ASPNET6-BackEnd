using System;
using System.Collections.Generic;

namespace Authorization_and_Authentication.Models;

public partial class Image
{
    public int Id { get; set; }

    public byte[]? ImageData { get; set; }
}
