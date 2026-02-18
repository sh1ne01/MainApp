using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;

[Authorize]
public class DashboardController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

}
