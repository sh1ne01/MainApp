using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly UserService _userService;

    public AdminController(UserService userService)
    {
        _userService = userService;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _userService.GetAllUsers();
        return View(users);
    }

    public async Task<IActionResult> ToggleBan(string login)
    {
        await _userService.ToggleBan(login);
        return RedirectToAction("Index");
    }
    public async Task<IActionResult> TogglePrem(string login)
    {
        await _userService.TogglePrem(login);
        return RedirectToAction("Index");
    }
}
