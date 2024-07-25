using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Authorization_and_Authentication.Auth
{
    public class StockModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProdId { get; set; }

        public string? ProdName { get; set; }
        public string? ProdPrice { get; set; }
        //[JsonConverter(typeof(JsonToByteArrayConverter))]
        public byte[]? ImgData { get; set; }

        public string? Category { get; set; }
        public string? Quantity { get; set; }

    }
}
