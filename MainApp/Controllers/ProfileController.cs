using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
public class ProfileController : Controller
{
    private readonly UserService _userService;

    public ProfileController(UserService userService)
    {
        _userService = userService;
    }

    public async Task<IActionResult> Index()
    {
        var login = User.Identity.Name;
        var user = await _userService.GetUserByLogin(login);

        return View(user);
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword(string newPassword)
    {
        var login = User.Identity.Name;

        await _userService.UpdatePassword(login, newPassword);

        ViewBag.Message = "Password updated successfully.";
        return RedirectToAction("Index");
    }
    [HttpPost]
    public async Task<IActionResult> UploadAvatar(IFormFile avatar)
    {
        if (avatar == null || avatar.Length == 0)
            return RedirectToAction("Index");

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
        var extension = Path.GetExtension(avatar.FileName).ToLower();

        if (!allowedExtensions.Contains(extension))
            return RedirectToAction("Index");

        var login = User.Identity.Name;

        var fileName = $"{login}_{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(Directory.GetCurrentDirectory(),
                                    "wwwroot/avatars",
                                    fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await avatar.CopyToAsync(stream);
        }

        string relativePath = $"/avatars/{fileName}";
        await _userService.UpdateAvatar(login, relativePath);

        return RedirectToAction("Index");
    }

}
