using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductionManagementSystem.Core.Models.Users;
using ProductionManagementSystem.WEB.Models.UserViewModels;

namespace ProductionManagementSystem.WEB.Controllers
{
    [Authorize(Roles=RoleEnum.Admin)]
    public class UsersController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {            
            return View(await _userManager.Users.ToListAsync());
        } 
        
        public async Task<ActionResult> Delete(string userName)
        {
            if (userName == "admin")
            {
                throw new Exception("Страница не найдена.");
            }

            var user = await _userManager.FindByNameAsync(userName);
            return View(user);
        }
        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<ActionResult> ChangeRole(string userName)
        {
            if (userName == null || userName == "admin")
            {
                throw new Exception("Страница не найдена.");
            }

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
 
            throw new Exception("Страница не найдена.");
        }

        public IActionResult Create()
        {
            CreateUserViewModel createUserViewModel = new CreateUserViewModel() { 
                AllRoles = _roleManager.Roles.ToList()
            };
            return View(createUserViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserViewModel createUserViewModel, List<string> roles)
        {
            if (ModelState.IsValid)
            {
                var user = new User() { UserName = createUserViewModel.UserName, EmailConfirmed = true };
                var result = await _userManager.CreateAsync(user, createUserViewModel.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRolesAsync(user, roles);

                    return RedirectToAction(nameof(Index));
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return RedirectToAction(nameof(Index));
        }
    }
}