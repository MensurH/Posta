using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Posta.Data;
using Posta.Models;

namespace Posta.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdministrationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        
        public AdministrationController(ApplicationDbContext context ,RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }
        [HttpGet("dashboard/admin")]
        public async Task<IActionResult> AdminDashboard(){
            var roles = _roleManager.Roles.ToList();
            ViewBag.Roles = roles;
            List<Member> members = new List<Member>();
            foreach(var role in roles)
            {
                var users = await _userManager.GetUsersInRoleAsync(role.Name);
                foreach(var user in users) 
                {
                    var member = new Member()
                    {
                        User = user,
                        Role = role.Name
                    };
                    members.Add(member);
                }
            }
            return View(members);
        }
        [HttpGet("roles/create")]
        public IActionResult CreateRole(){
            return View();
        }
        [HttpPost("roles/create")]
        public async Task<IActionResult> CreateRole(RoleViewModel roleViewModel){
            if(ModelState.IsValid){
                IdentityRole identityRole = new IdentityRole(){
                    Name = roleViewModel.RoleName
                };
                var result = await _roleManager.CreateAsync(identityRole);
                if(result.Succeeded){
                    return RedirectToAction("AdminDashboard");
                }
                foreach(var error in result.Errors){
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(roleViewModel);
        }
        [HttpPost("roles/delete")]
        public async Task<IActionResult> DeleteRole(string id){
            var role = await _roleManager.FindByIdAsync(id);
            if(role == null){
                //return to error view 
                return RedirectToAction("AdminDashboard");
            }
            var result = await _roleManager.DeleteAsync(role);
            if(result.Succeeded){
                return RedirectToAction("AdminDashboard");
            }
            // foreach(var error in result.Errors){
            //     ModelState.AddModelError(string.Empty, error.Description);
            // }
            return RedirectToAction("AdminDashboard");
        }
    
        [HttpGet("roles/edit/{id}")]
        public async Task<IActionResult> EditRole(string id){
            var role = await _roleManager.FindByIdAsync(id);
            if(role == null)
            {
                return RedirectToAction("AdminDashboard");
            }
            var model = new EditRoleViewModel(){
                Id = role.Id,
                RoleName = role.Name
            };

            model.Users = await _userManager.GetUsersInRoleAsync(role.Name);
            return View(model);
        }
        [HttpPost("roles/edit")]
        public async Task<IActionResult> Edit(EditRoleViewModel model){
            if(ModelState.IsValid){
                var role = await _roleManager.FindByIdAsync(model.Id);
                if(role == null)
                {
                    return RedirectToAction("AdminDashboard");
                }
                role.Name = model.RoleName;
                var result = await _roleManager.UpdateAsync(role);
                if(result.Succeeded)
                {
                    return RedirectToAction("AdminDashboard");
                }
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View("EditRole",model);
        }
        
        [HttpGet("roles/users/add/{roleId}")]
        public async Task<IActionResult> AddUserToRole(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                return RedirectToAction("AdminDashboard");
            }
            var model = new AddUserViewModel()
            {
                RoleId = roleId,
            };
            List<ApplicationUser> usersInRole = new List<ApplicationUser>();
            foreach(var user in _context.Users.ToList())
            {
                if(_context.UserRoles.FirstOrDefault(ur => ur.UserId == user.Id) != null)
                {
                    usersInRole.Add(user);
                }
            }
            
            model.Users = _context.Users.Where(u => !usersInRole.Contains(u));
            return View(model);
        }
        [HttpPost("roles/users/add")]
        public async Task<IActionResult> AddUser(AddUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var role = await _roleManager.FindByIdAsync(model.RoleId);
                var user = await _userManager.FindByIdAsync(model.UserId);
                if(role == null || user == null)
                {
                    /*ModelState.AddModelError(string.Empty, "User or role does not exist!");*/
                    return RedirectToAction("AdminDashboard");
                }
                if(!(await _userManager.IsInRoleAsync(user, role.Name)))
                {
                    var result = await _userManager.AddToRoleAsync(user, role.Name);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("EditRole", new { id = role.Id });
                    }
                    foreach(var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View("AddUserToRole", model);
        }
        public async Task<IActionResult> RemoveUserFromRole(string userId, string roleId){
            var user = await _userManager.FindByIdAsync(userId);
            var role = await _roleManager.FindByIdAsync(roleId);
            if(user == null || role == null){
                return RedirectToAction("AdminDashboard");
            }
            var result = await _userManager.RemoveFromRoleAsync(user, role.Name);
            if(result.Succeeded){
                return RedirectToAction("EditRole", new {id = roleId});
            }
            return RedirectToAction("AdminDashboard");
        }
    }
}