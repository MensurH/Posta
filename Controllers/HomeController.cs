using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Posta.Data;
using Posta.Models;

namespace Posta.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            if(await _userManager.IsInRoleAsync(currentUser, "Admin")){
                return RedirectToAction("AdminDashboard","Administration");
            }
            else if (await _userManager.IsInRoleAsync(currentUser, "Postier"))
            {
                return RedirectToAction("PostierDashboard", "Postier");
            }
            else
            {
                ViewBag.Deliveries = _context.Deliveries.Where(d => d.OrderStatus != "Delivered" && d.OrderStatus != "Canceled" && d.UserId == currentUser.Id)
                .OrderByDescending(d => d.CreatedAt).ToList();
            }
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
    }
}
