using Microsoft.AspNetCore.Mvc;
using ShoeStore.Client.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace ShoeStore.Client.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminUsersController : Controller
    {
        private readonly IUserService _userService;
        public AdminUsersController(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllAsync();
            return View(users);
        }
    }
} 