using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProductionManagementSystem.DAL.Entities;
using ProductionManagementSystem.Models;
using ProductionManagementSystem.WEB.Models.UserViewModels;

namespace ProductionManagementSystem.Controllers
{
    [Authorize(Roles=RoleEnum.Admin)]
    public class UsersController : Controller
    {
        private UserManager<ProductionManagementSystemUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;

        public UsersController(UserManager<ProductionManagementSystemUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {            
            return View(_userManager.Users.ToList());
        } 
        
        public async Task<string> SetRoles()
        {
            string adminEmail = "admin";
            string password = "123456";
            if (await _roleManager.FindByNameAsync(RoleEnum.Admin) == null)
            {
                await _roleManager.CreateAsync(new IdentityRole(RoleEnum.Admin));
            }
            if (await _roleManager.FindByNameAsync(RoleEnum.OrderPicker) == null)
            {
                await _roleManager.CreateAsync(new IdentityRole(RoleEnum.OrderPicker));
            }
            if (await _roleManager.FindByNameAsync(RoleEnum.Assembler) == null)
            {
                await _roleManager.CreateAsync(new IdentityRole(RoleEnum.Assembler));
            }
            if (await _roleManager.FindByNameAsync(RoleEnum.Tuner) == null)
            {
                await _roleManager.CreateAsync(new IdentityRole(RoleEnum.Tuner));
            }
            if (await _roleManager.FindByNameAsync(RoleEnum.Collector) == null)
            {
                await _roleManager.CreateAsync(new IdentityRole(RoleEnum.Collector));
            }
            if (await _roleManager.FindByNameAsync(RoleEnum.Validating) == null)
            {
                await _roleManager.CreateAsync(new IdentityRole(RoleEnum.Validating));
            }
            if (await _roleManager.FindByNameAsync(RoleEnum.Shipper) == null)
            {
                await _roleManager.CreateAsync(new IdentityRole(RoleEnum.Shipper));
            }
            if (await _roleManager.FindByNameAsync(adminEmail) == null)
            {
                var admin = new ProductionManagementSystemUser() { Email = adminEmail, UserName = adminEmail };
                IdentityResult result = await _userManager.CreateAsync(admin, password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(admin, "Администратор");
                }
            }

            return "Ok";
        }

        public async Task<ActionResult> Delete(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            return View(user);
        }
        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string userName)
        {
            var user = await _userManager.FindByIdAsync(userName);
            if (user != null)
            {
                IdentityResult result = await _userManager.DeleteAsync(user);
            }
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> ChangeRole(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            ChangeRoleViewModel changeRoleViewModel = new ChangeRoleViewModel()
            {
                AllRoles = _roleManager.Roles.ToList(),
                UserId = user.Id,
                UserName = user.UserName,
                UserRoles = await _userManager.GetRolesAsync(user)
            };
            
            return View(changeRoleViewModel);
        }
        
        [HttpPost]
        public async Task<IActionResult> ChangeRole(string userId, List<string> roles)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if(user!=null)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var addedRoles = roles.Except(userRoles);
                var removedRoles = userRoles.Except(roles);
 
                await _userManager.AddToRolesAsync(user, addedRoles);
 
                await _userManager.RemoveFromRolesAsync(user, removedRoles);
 
                return RedirectToAction(nameof(Index));
            }
 
            return NotFound();
        }
    }
}