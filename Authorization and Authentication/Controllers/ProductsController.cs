using Authorization_and_Authentication.Auth;
using Authorization_and_Authentication.Migrations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Text.Json;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Authorization_and_Authentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _ApplicationDbContext;
        private readonly IWebHostEnvironment _environment;

        public ProductsController(ApplicationDbContext ApplicationDbContext, IWebHostEnvironment environment)
        {
            _ApplicationDbContext = ApplicationDbContext;
            _environment = environment;
        }


        [HttpPost]
        [Route("ImageUpload")]
        [Consumes("application/json")]
        public async Task<IActionResult> UploadImage([FromForm] IFormFile myFile, string ProdPrice, string FQuantity, string category)
        {

            if (myFile == null)
                return BadRequest("Invalid file KG");

            using (var ms = new MemoryStream())
            {
                await myFile.CopyToAsync(ms);
                var FileData = ms.ToArray();

                var imageModel = new StockModel
                {
                    ProdName = myFile.FileName,
                    ProdPrice = ProdPrice,
                    Quantity = FQuantity,
                    Category = category,
                    ImgData = FileData
                };


                _ApplicationDbContext.Stock.Add(imageModel);
                await _ApplicationDbContext.SaveChangesAsync();

                return Ok(imageModel.ProdId);
            }



        }





        [HttpPost]
        [Route("Upload2")]
        public async Task<IActionResult> Upload2([FromForm] StockModel model, [FromForm] IFormFile imgData)
        {

            if (imgData == null || imgData.Length == 0)
                return BadRequest("Invalid file");

            using (var ms = new MemoryStream())
            {
                await imgData.CopyToAsync(ms);
                var imageData = ms.ToArray();

                model.ProdName = imgData.FileName;
                model.ImgData = imageData;

                _ApplicationDbContext.Stock.Add(model);
                await _ApplicationDbContext.SaveChangesAsync();

                return Ok(model.ProdId);
            }



        }



        [HttpPost]
        [Route("Uploader")]
        public async Task<IActionResult> Uploader([FromBody] StockModel model, IFormFile image)
        {
            if (image == null || image.Length == 0)
                return BadRequest("No image provided");

            using var memoryStream = new MemoryStream();
            await image.CopyToAsync(memoryStream);

            var imageModel = new StockModel
            {
                ProdName = image.FileName,
                ImgData = memoryStream.ToArray()
            };

            _ApplicationDbContext.Stock.Add(imageModel);
            await _ApplicationDbContext.SaveChangesAsync();

            return Ok("Image uploaded successfully");
        }





        [HttpPost("ImageUpload3")]
        [Consumes("application/json")]
        public async Task<IActionResult> UploadedImage2([FromBody] StockModel model)
        {
            if (model.ImgData == null)
                return BadRequest("Failed null errorrrrrr");
            var stream = new MemoryStream();
            IFormFile file = new FormFile(stream, 0, model.ImgData.Length, "name", "fileName");

            await file.CopyToAsync(stream);
            var FileData = stream.ToArray();            
         
            string jsonString = System.Text.Json.JsonSerializer.Serialize(FileData);         
            byte[] byteArray = Encoding.UTF8.GetBytes(jsonString);

            //Second Solution
           // byte[] byteArray2 = System.Text.Encoding.UTF8.GetBytes(jsonString);



            //Console.WriteLine($"Changed Vaue Is: {convertedValue}");
            //var jsonString = JsonSerializer.Serialize(FileData);
            //byte[] buffer = Encoding.UTF8.GetBytes(jsonString);
            //byte[] byteArray = JsonSerializer.SerializeToUtf8Bytes(FileData);            
            //byte[] myModel1 = JsonSerializer.Deserialize<byte[]>(FileData);
            //StockModel myModel = JsonSerializer.Deserialize<StockModel>(FileData); 

            //var newModel = new StockModel { 
               //  ImgData = byteArray,
               //  ProdName = file.FileName
           // };

            model.ImgData = byteArray;
            model.ProdName = file.FileName;

            _ApplicationDbContext.Stock.Add(model);
            _ApplicationDbContext.SaveChanges();

            return Ok("Image Saved Successfully");


        }


        // **********************TRY THIS ONE (converts to system byte)*****************************

        [HttpPost("Image")]
        [Consumes("application/json")]
        public async Task<IActionResult> Image([FromBody] StockModel model)
        {

            if (model.ImgData == null)
                return BadRequest("Invalid file KG");

           
            using (var ms = new MemoryStream())
            {
                IFormFile file = new FormFile(ms, 0, model.ImgData.Length, "file", "FileName");
                await file.CopyToAsync(ms);
                ms.ToArray();
                string imageDt = file.ContentType;

                var imageModel = new StockModel
                {
                    ProdName = file.FileName,
                    ProdPrice = model.ProdPrice,
                    Quantity = model.Quantity,
                    Category = model.Category,
                    ImgData = System.IO.File.ReadAllBytes(imageDt) 
                };


                _ApplicationDbContext.Stock.Add(imageModel);
                await _ApplicationDbContext.SaveChangesAsync();

                return Ok(model.ProdId);
            }
        }


        [HttpPost("All")]
        public async Task<IActionResult> AllSave([FromBody] StockModel model)
        {
           
            _ApplicationDbContext.Stock.Add(model);
            await _ApplicationDbContext.SaveChangesAsync();

            return Ok(model.ProdId);


        }


        [HttpPost]
        [Route("Test")]

        public async Task<IActionResult> myTest([FromBody] StockModel model, string myPic)
        {
            var myModel = model.ImgData;

            try
            {
                // Convert the Base64 image data to a byte array
                byte[] imageByteArray = Convert.FromBase64String(myPic);

                return Ok(imageByteArray);
            }
            catch (FormatException)
            {
                return BadRequest("Invalid Base64 string.");
            }
          


        }

        [HttpPost("SendPic")]
        [Consumes("application/json")] 


        public async Task<IActionResult> myPicSend([Bind("ImgData")] StockModel blog, IFormFile Image)
        {

            if (Image != null && Image.Length > 0)
            {
                byte[] imageBytes;
                using (var stream = new MemoryStream())
                {
                    await Image.CopyToAsync(stream);
                    imageBytes = stream.ToArray();
                }
                blog.ImgData = imageBytes;

                // Save the blog entry to the database
                _ApplicationDbContext.Add(blog);
                await _ApplicationDbContext.SaveChangesAsync();
                return Ok("Successfully uploaded");
            }
            else
            {
                return BadRequest("Invalid file KG errrooor");
            }

            
        }


        // ************************SHOWS NEW ERROR (instead of json conversion error)*************************

        [HttpPost("SendPic2")]
        //[Consumes("application/json")]


        public async Task<IActionResult> myPicSend2(StockModel blog, [FromForm] IFormFile Imagepic)
        {

            if (Imagepic != null && Imagepic.Length > 0)
            {
                byte[] imageBytes;
                using (var stream = new MemoryStream())
                {
                    await Imagepic.CopyToAsync(stream);
                    imageBytes = stream.ToArray();
                }
                blog.ImgData = imageBytes;

                // Save the blog entry to the database
                _ApplicationDbContext.Add(blog);
                await _ApplicationDbContext.SaveChangesAsync();
                return Ok("Successfully uploaded");
            }
            else
            {
                return BadRequest("Invalid file KG errrooor");
            }


        }




        //[HttpPost]
        //[Route("Upload")]
        //[Consumes("application/json")]
        //public async Task<IActionResult> Upload([FromBody] StockModel model)
        //{
        //if (model.ImgData == null)
        //return BadRequest("Invalid file");

        //using (var ms = new MemoryStream())
        //{
        //await model.ImgData.CopyToAsync(ms);
        //  ms.ToArray();
        // model.ProdName = model.ImgData.FileName;


        //_ApplicationDbContext.Stock.Add(model);
        //await _ApplicationDbContext.SaveChangesAsync();

        //return Ok(model.ProdId);
        //}

        //}



        // [HttpGet("{id}")]
        // public IActionResult GetImage(int id)
        // {
        //     var image = _ApplicationDbContext.ImgProducts.Find(id);
        //     if (image == null)
        //         return NotFound();

        //     return File(image.imgData, image.prodPrice, image.prodName);
        // }

        // var options = new JsonSerializerOptions();
        //options.Converters.Add(new JsonToByteArrayConverter());

        //string json = JsonSerializer.Serialize(model.ImgData, options);
        //var deserializedObject = JsonSerializer.Deserialize<StockModel>(json, options);






    }
}
