using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using OfficeLeaveCore.Data;
using OfficeLeaveCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeLeaveCore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext context;

        public UsersController(RoleManager<IdentityRole> _roleManager,UserManager<ApplicationUser> _userManager, ApplicationDbContext _context)
        {
            roleManager = _roleManager;
            userManager = _userManager;
            context = _context;
        }


        //GET /Users/
        [HttpGet]
        
        public IActionResult Index()
        {
            var users = context.Users.ToList();
            List<Dto.User> Users = new List<Dto.User>();

            foreach (var user in users)
            {
                //get all user roles
                var userRoles = context.UserRoles.Where(x=>x.UserId.Equals(user.Id)).ToList();

                if(userRoles == null)
                {
                    Users.Add(new Dto.User()
                    {
                        Id = user.Id,
                        FullName = user.FullName,
                        Email = user.Email,
                        Roles =  "N/A",
                        UserName = user.UserName,
                    });
                }
                else
                {
                    var roleList = new List<string>();
                    foreach (var role in userRoles)
                    {
                        var userRole = context.Roles.Where(r => r.Id.Equals(role.RoleId)).Select(r => r.Name).FirstOrDefault();
                        roleList.Add(userRole);
                    }

                    Users.Add(new Dto.User()
                    {
                        Id = user.Id,
                        FullName = user.FullName,
                        Email = user.Email,
                        Roles = String.Join(',', roleList),
                        UserName = user.UserName,
                    });
                }
            }

            ViewBag.Roles = roleManager.Roles.Select(r => r.Name).ToList();
            return View(Users);
        }

        //GET /Users/
        [HttpGet]
        public IActionResult Create()
        {
            //get roles
            var roles = context.Roles.Select(r=>r.Name).ToList();
            ViewBag.Roles = roles;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ApplicationUser user)
        {
            if (ModelState.IsValid)
            {
                user.UserName = user.Email;
                var userCreated = await userManager.CreateAsync(user,"Password@123");

                if(userCreated.Succeeded)
                {
                    //add role
                    var userRole = roleManager.FindByNameAsync(user.Role).Result;
                    if (userRole != null)
                    {
                        var assignRole = await userManager.AddToRoleAsync(user, userRole.Name);
                    }

                    var code = await userManager.GenerateEmailConfirmationTokenAsync(user);

                    TempData["SuccessMsg"] = "User has been created";
                    return RedirectToAction(actionName: "Index");
                }

                
            }

            TempData["ErrorMsg"] = "Something went wrong";
            return RedirectToAction(actionName:"Create");

        }

        //GET /Users/Details/id
        public IActionResult Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = context.Users.Where(x => x.Id.Equals(id)).FirstOrDefault();
            return View(user);
        }

        [HttpGet]
        public IActionResult Edit(string id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var user = context.Users.Where(x => x.Id.Equals(id)).FirstOrDefault();

            return View(user);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id,ApplicationUser applicationUser)
        {
            if(ModelState.IsValid)
            {
                var user = context.Users.Where(u => u.Id.Equals(id)).FirstOrDefault();
                user.FullName = applicationUser.FullName;

                context.Users.Update(user);
                await  context.SaveChangesAsync();

                TempData["SuccessMsg"] = "User information updated.";
                return RedirectToAction(actionName: "Index");
            }

            TempData["ErrorMsg"] = "Something went wrong.";
            return RedirectToAction(actionName:"Edit");

        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            var user = context.Users.Where(u => u.Id.Equals(id)).FirstOrDefault();

            if(user != null)
            {
                context.Users.Remove(user);
                await context.SaveChangesAsync();

                TempData["SuccessMsg"] = "User has been deleted";
                return Json("success");
            }
            else
            {
                TempData["ErrorMsg"] = "Something went wrong.";
                return Json("error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Assign(string id,string assign_role)
        {
            //get user and role
            var user = context.Users.Where(u => u.Id.Equals(id)).FirstOrDefault();
            var role = roleManager.FindByNameAsync(assign_role).Result;
            if (role != null)
            {
                var assignRole = await userManager.AddToRoleAsync(user, role.Name);

                if(assignRole.Succeeded)
                {
                    TempData["SuccessMsg"] = "New role has been assigned.";
                    //return RedirectToAction(actionName: "Index");
                    return Json("success");
                }
                else
                {
                    TempData["SuccessMsg"] = assignRole.Errors.FirstOrDefault();
                    //return RedirectToAction(actionName: "Index");
                    return Json("success");
                }
            }
            

            TempData["ErrorMsg"] = "Something went wrong.";
            return Json("error");
        }

        [HttpPut]
        public IActionResult Revoke(string id, List<ApplicationRole> applicationRoles)
        {
            return Json(applicationRoles[0].Name);
            //return applicationRole.
            //get user and role
            /*var user = context.Users.Where(u => u.Id.Equals(id)).FirstOrDefault();
            var role = roleManager.FindByNameAsync(assign_role).Result;
            if (role != null)
            {
                var assignRole = await userManager.AddToRoleAsync(user, role.Name);

                if (assignRole.Succeeded)
                {
                    TempData["SuccessMsg"] = "New role has been assigned.";
                    //return RedirectToAction(actionName: "Index");
                    return Json("success");
                }
                else
                {
                    TempData["SuccessMsg"] = assignRole.Errors.FirstOrDefault();
                    //return RedirectToAction(actionName: "Index");
                    return Json("success");
                }
            }


            TempData["ErrorMsg"] = "Something went wrong.";
            return Json("error");*/
        }
    }
}
