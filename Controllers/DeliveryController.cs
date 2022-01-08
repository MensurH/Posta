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
    [Authorize]
    public class DeliveryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DeliveryController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [HttpGet("deliveries/add")]
        public IActionResult AddDelivery(){
            return View();
        }
        [HttpPost("deliveries/add")]
        public IActionResult AddDelivery(Delivery delivery)
        {
            if(ModelState.IsValid){
                delivery.OrderStatus = "Registered";
                delivery.UserId = _userManager.GetUserId(HttpContext.User);
                _context.Deliveries.Add(delivery);
                _context.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return View(delivery);
        }
        [HttpPost("deliveries/cancel")]
        public IActionResult CancelDelivery(int id)
        {
            var Delivery = _context.Deliveries.FirstOrDefault(d => d.DeliveryId == id);
            if(Delivery == null)
            {
                return RedirectToAction("Index","Home");
            }
            Delivery.OrderStatus = "Canceled";
            _context.Deliveries.Update(Delivery);
            _context.SaveChanges();
            return RedirectToAction("Index","Home");
        }
        [HttpGet("deliveries/{id}")]
        public IActionResult Delivery(int id){
            var Delivery = _context.Deliveries.FirstOrDefault(d => d.DeliveryId == id);
            if(Delivery == null){
                return RedirectToAction("Index", "Home");
            }
            return View(Delivery);
        }
        [HttpPost("deliveries/update")]
        public IActionResult UpdateDelivery(Delivery viewDelivery){
            
            if(ModelState.IsValid){
                var delivery = _context.Deliveries.FirstOrDefault(d => d.DeliveryId == viewDelivery.DeliveryId);
                if(delivery == null)
                {
                    return RedirectToAction("Index","Home");
                }
                delivery.RecieverName = viewDelivery.RecieverName;
                delivery.PhoneNumber = viewDelivery.PhoneNumber;
                delivery.City = viewDelivery.City;
                delivery.Address = viewDelivery.Address;
                _context.Update(delivery);
                _context.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return View("Delivery", viewDelivery);
        }
        [HttpGet("deliveries")]
        public async Task<IActionResult> ListAll(){
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            ViewBag.Deliveries = _context.Deliveries.Where(d => d.UserId == currentUser.Id).OrderByDescending(d => d.CreatedAt).ToList();
            return View("ListDeliveries");
        }
        [HttpGet("delivery/{id}")]
        public IActionResult GetDelivery(int id)
        {
            var delivery = _context.Deliveries.Include(d => d.User).FirstOrDefault(d => d.DeliveryId == id);
            if(delivery == null)
            {
                return RedirectToAction("PostierDashboard","Postier");
            }
            ViewBag.Delivery = delivery;
            return View("DeliveryStatus");
        }
        [HttpPost]
        public IActionResult UpdateStatus(UpdateStatusModel ups)
        {
            if(ModelState.IsValid){
                var Delivery = _context.Deliveries.FirstOrDefault(d => d.DeliveryId == ups.DeliveryId);
                Delivery.OrderStatus = ups.OrderStatus;
                _context.Deliveries.Update(Delivery);
                _context.SaveChanges();
                return RedirectToAction("PostierDashboard","Postier");
            }
            return View("DeliveryStatus", ups);
        }
    }
}