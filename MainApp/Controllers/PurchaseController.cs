using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MainApp.Controllers
{
    [Authorize]
    public class PurchaseController : Controller
    {

        private readonly UserService _userService;
        public PurchaseController(UserService userService)
        {
            _userService = userService;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Purchase(string CardNumber, string Email)
        {
            var login = User.Identity.Name;

            if (login == null)
                return Unauthorized();

            await _userService.AddPremium(login);

            return RedirectToAction("Index");
        }
    }
}
