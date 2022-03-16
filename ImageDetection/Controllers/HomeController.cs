using ImageDetection.Models;
using ImageRecognitionService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace ImageDetection.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _env;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile Image,string ModelType= "yolov5n.onnx")
        {
            string unFolderId = Guid.NewGuid().ToString();
            string FilePath = Path.Combine(_env.WebRootPath + "/DataSource//imagesResult//" + unFolderId);
            if(!Directory.Exists(FilePath))
            Directory.CreateDirectory(FilePath+"/input");
            using (var fileStream = new FileStream(FilePath+ "/input/" + Image.FileName, FileMode.Create))
            {
                await Image.CopyToAsync(fileStream);
            }
            string ModelPath = Path.Combine(_env.WebRootPath + "/DataSource//OnnxFiles//", ModelType);
           var result = ImageService.RecognizeImage(FilePath, Image.FileName, ModelPath);
            if(result.success ==101)
            {
                return RedirectToAction("Result", new { lables = string.Join(",", result.Lables),fileName = Image.FileName,unFolderid= unFolderId });
            }
                /*string path = Path.Combine(FilePath, "output/") + Image.FileName;
                byte[] bytes = System.IO.File.ReadAllBytes(path);
                return File(bytes, "application/octet-stream", "Result_"+Image.FileName);*/
         
            else if(result.success == 100)
            {
                return Json("Unable to identify objects in image.");
            }
            else
            {
                return Json("Some error occurs while processing the image.");
            }
        }
        public IActionResult Result(string lables, string fileName,string unFolderid)
        {
            ImageOutputResult imageOutputResult = new ImageOutputResult { fileName = fileName, labels= lables.Split(',').ToList(),unFolderId=unFolderid };
            return View(imageOutputResult);

        }
        [HttpGet]
        public  IActionResult DownloadResult(Guid id,string fileName)
        {
            try
            {
                string FilePath = Path.Combine(_env.WebRootPath + "/DataSource//imagesResult//" + id);
                string path = Path.Combine(FilePath, "output/") + fileName;
                byte[] bytes = System.IO.File.ReadAllBytes(path);
                return File(bytes, "application/octet-stream", "Result_" + fileName);
            }
            catch(Exception ex)
            {
                return Json("Error occurred!!");
            }
        }
    }
}
