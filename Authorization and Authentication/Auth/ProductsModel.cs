using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Authorization_and_Authentication.Auth
{
    public class ProductsModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int prodId { get; set; }

        public string? prodName { get; set; }
        [Required(ErrorMessage = "Product Price is required")]
        public string? prodPrice { get; set; }
        public byte[]? imgData { get; set; }
       // public IFormFile? imgData { get; set; }

    }
}
