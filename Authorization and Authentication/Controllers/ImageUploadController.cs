using Authorization_and_Authentication.Auth;
using Authorization_and_Authentication.Models;
using Microsoft.AspNetCore.Mvc;


namespace Authorization_and_Authentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageUploadController : Controller
    {

        private readonly ApplicationDbContext _ApplicationDbContext;
        private readonly IWebHostEnvironment _environment;

        public ImageUploadController(ApplicationDbContext ApplicationDbContext, IWebHostEnvironment environment)
        {
            _ApplicationDbContext = ApplicationDbContext;
            _environment = environment;
        }




        [HttpPost("Image4")]
        [Consumes("multipart/form-data")]

        public async Task<IActionResult> Image4()
        {
            
            var httpRequest = HttpContext.Request.Form;
            var postedFile = httpRequest.Files["myFile"];
            if (postedFile == null)
                return BadRequest("No Image naaada");

            using (MemoryStream msStream = new MemoryStream())
                    {
                        await postedFile.CopyToAsync(msStream);
                        var Mydata = msStream.ToArray();
                        this._ApplicationDbContext.ProductImage.Add(new ProductImage()
                        {
                            Id = int.Parse(httpRequest["Id"]),
                            ProdPrice = httpRequest["ProdPrice"],
                            Quantity = int.Parse(httpRequest["Quantity"]),
                            ProdName = postedFile.FileName,
                            Category = httpRequest["Category"],
                            ImgData = Mydata
                        });
                        await this._ApplicationDbContext.SaveChangesAsync();

                return StatusCode(StatusCodes.Status200OK,
                new Response { Status = "Success", Message = $"Successfully Uploaded" });
            }


        }


        [HttpGet("GetImage")]

        public ActionResult<List<ProductImage>> GetImage()
        {
            var result = _ApplicationDbContext.ProductImage.ToList();
            return result;

        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_ApplicationDbContext.ProductImage.ToList());
        }








    }
}
