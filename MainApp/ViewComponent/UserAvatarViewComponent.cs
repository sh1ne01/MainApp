using Microsoft.AspNetCore.Mvc;

namespace MainApp.ViewComponents
{
    public class UserAvatarViewComponent : ViewComponent
    {
        private readonly UserService _userService;

        public UserAvatarViewComponent(UserService userService)
        {
            _userService = userService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (!User.Identity.IsAuthenticated)
                return View(null);

            var user = await _userService.GetUserByLogin(User.Identity.Name);
            return View(user);
        }
    }
}
