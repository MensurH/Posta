using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Posta.Data;
using Posta.Models;

namespace Posta.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }   
        [HttpGet("register")]
        public IActionResult Register(){
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel){
            if(ModelState.IsValid){
                var user = new ApplicationUser(){
                    UserName = registerViewModel.Username,
                    Email = registerViewModel.Email,
                    City = registerViewModel.City,
                    FullName = registerViewModel.FullName,
                    PhoneNumber = registerViewModel.PhoneNumber,
                    Address = registerViewModel.Address
                };
                var result =  await _userManager.CreateAsync(user, registerViewModel.Password);
                if(result.Succeeded){
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Home");
                }
                foreach(var error in result.Errors){
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(registerViewModel);
        }
        [HttpGet("login")]
        public IActionResult Login(){
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel){
            if(ModelState.IsValid){
                var user = await _userManager.FindByEmailAsync(loginViewModel.Email);
                var result = await _signInManager.PasswordSignInAsync(user.UserName, loginViewModel.Password, loginViewModel.RememberMe, false);
                if(result.Succeeded)
                {
                    return RedirectToAction("Index","Home");
                }
                ModelState.AddModelError(string.Empty, "Invalid credetintals!");
            }
            return View(loginViewModel);
        }
        public async Task<IActionResult> Logout()  
        {  
            await _signInManager.SignOutAsync();  
            return RedirectToAction("login", "account");  
        }  
    }
}