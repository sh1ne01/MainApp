using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MainApp.Controllers
{
    [Authorize]
    public class PremiumController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult DownloadFile()
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/files", "cheat.exe");

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Файл не найден!");
            }

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            string fileName = "cheat.exe";
            return File(fileBytes, "application/octet-stream", fileName);
        }
    }

}
