using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using MainApp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

public class AccountController : Controller
{
    private readonly UserService _userService;

    public AccountController(UserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(string login, string password)
    {
        await _userService.AddUser(login, password);
        return RedirectToAction("Login");
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string login, string password)
    {
        bool isValid = await _userService.CheckPassword(login, password);

        if (!isValid)
        {
            ViewBag.Error = "Неверный логин или пароль";
            return View();
        }
        bool isBanned = await _userService.IsUserBanned(login);

        if (isBanned)
        {
            ViewBag.Error = "Вы были забанены!";
            return View();
        }


        string? role = await _userService.GetUserRole(login);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, login),
            new Claim(ClaimTypes.Role, role ?? "User")
        };



    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties
        );

        return RedirectToAction("Index", "Dashboard");
    }

    public override async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        if (User.Identity.IsAuthenticated)
        {
            var login = User.Identity.Name;
            bool isBanned = await _userService.IsUserBanned(login);

            if (isBanned)
            {
                await HttpContext.SignOutAsync();
                context.Result = RedirectToAction("Login", "Account");
                return;
            }
        }

        await next();
    }



    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }
}
