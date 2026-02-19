using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MainApp.Models;

[Authorize(Roles = "Admin,Premium")]
public class ForumController : Controller
{
    private readonly ForumService _forumService;
    private readonly UserService _userService;

    public ForumController(ForumService forumService,UserService userservice)
    {
        _forumService = forumService;
        _userService = userservice;
    }


    public async Task<IActionResult> Index()
    {
        var posts = await _forumService.GetPosts();
        return View(posts);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(string title, string content)
    {
        var login = User.Identity.Name;
        string role = await _userService.GetUserRole(login);
        string avatar = await _forumService.GetUserAvatar(login);
        Console.WriteLine(role);
        await _forumService.AddPost(title, content, login, role,avatar);

        return RedirectToAction("Index");
    }
}
