using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Posta.Data;
using Posta.Models;

namespace Posta.Controllers
{
    [Authorize(Roles = "Postier")]
    public class PostierController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        
        public PostierController(ApplicationDbContext context ,RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }
        [HttpGet("dashboard/postier")]
        public async Task<IActionResult> PostierDashboard()
        {
            var postier = await _userManager.GetUserAsync(HttpContext.User);
            var deliveriesToCollect = _context.Deliveries.Include(d => d.User).Where(d => d.OrderStatus == "Registered" && d.User.City == postier.City).ToList();
            ViewBag.DeliveriesToCollect = deliveriesToCollect;
            var deliveriesToDistribute = _context.Deliveries.Where(d => d.OrderStatus == "En Route" && d.City == postier.City).ToList();
            ViewBag.deliveriesToDistribute = deliveriesToDistribute;
            return View();
        }
    }
}