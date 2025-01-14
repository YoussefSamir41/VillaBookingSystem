using Application.Common.Interfaces;
using Application.Common.Utility;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VillaBooking.ViewModels;

namespace VillaBooking.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public AccountController(IUnitOfWork unitOfWork,
            UserManager<ApplicationUser> userManager
         , SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            this.unitOfWork = unitOfWork;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
        }
        public IActionResult Login(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            LoginVM loginVM = new LoginVM()
            {
                RedirectUrl = returnUrl
            };

            return View(loginVM);
        }
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Register()
        {
            if (!roleManager.RoleExistsAsync(SD.RoleAdmin).GetAwaiter().GetResult())
            {
                roleManager.CreateAsync(new IdentityRole(SD.RoleAdmin)).Wait();
                roleManager.CreateAsync(new IdentityRole(SD.RoleCustomer)).Wait();


            }


            RegisterVM registerVM = new()
            {
                RoleList = roleManager.Roles.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id
                })
            };
            return View(registerVM);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            ApplicationUser user = new()
            {
                Name = registerVM.Name,
                Email = registerVM.Email,
                PhoneNumber = registerVM.PhoneNumber,
                NormalizedEmail = registerVM.Email.ToUpper(),
                EmailConfirmed = true,
                UserName = registerVM.Email,
                CreatedAt = DateTime.Now,
            };

            var res = await userManager.CreateAsync(user, registerVM.Password);

            if (res.Succeeded)
            {
                string roleName = registerVM.Role;
                if (!string.IsNullOrEmpty(roleName))
                {
                    var role = await roleManager.FindByIdAsync(roleName);
                    if (role != null)
                    {
                        roleName = role.Name;
                    }
                    else
                    {
                        // Handle case where the role is not found by ID
                        ModelState.AddModelError("", "Role does not exist.");
                        return View(registerVM);
                    }
                }
                else
                {
                    roleName = SD.RoleCustomer;
                }

                await userManager.AddToRoleAsync(user, roleName);
                await signInManager.SignInAsync(user, isPersistent: false);

                if (string.IsNullOrEmpty(registerVM.RedirectUrl))
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return LocalRedirect(registerVM.RedirectUrl);
                }
            }

            foreach (var error in res.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            registerVM.RoleList = roleManager.Roles.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id
            });

            return View(registerVM);
        }

        #region Register Error
        //public async Task<IActionResult> Register(RegisterVM registerVM)
        //{
        //    ApplicationUser user = new()
        //    {
        //        Name = registerVM.Name,
        //        Email = registerVM.Email,
        //        PhoneNumber = registerVM.PhoneNumber,
        //        NormalizedEmail = registerVM.Email.ToUpper(),
        //        EmailConfirmed = true,
        //        UserName = registerVM.Email,
        //        CreatedAt = DateTime.Now,
        //    };

        //    var res = await userManager.CreateAsync(user, registerVM.Password);

        //    if (res.Succeeded)
        //    {
        //        if (!string.IsNullOrEmpty(registerVM.Role)) { 
        //          await userManager.AddToRoleAsync(user, registerVM.Role);

        //        }else
        //        {
        //            await userManager.AddToRoleAsync(user, SD.RoleCustomer); 
        //        }
        //        await signInManager.SignInAsync(user, isPersistent: false);

        //        if (string.IsNullOrEmpty(registerVM.RedirectUrl)) {
        //            return RedirectToAction("Index", "Home");

        //        }
        //        else
        //        {
        //            return LocalRedirect(registerVM.RedirectUrl);
        //        }
        //    }


        //    foreach (var error in res.Errors)
        //    {
        //        ModelState.AddModelError("", error.Description); 
        //    }

        //    registerVM.RoleList = roleManager.Roles.Select(x => new SelectListItem
        //    {
        //        Text = x.Name,
        //        Value = x.Id
        //    }); 



        //    return View(registerVM);

        //}

        #endregion

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (ModelState.IsValid)
            {
                var res = await signInManager.
                PasswordSignInAsync(loginVM.Email, loginVM.Password, loginVM.RemeberMe, lockoutOnFailure: false);
                if (res.Succeeded)
                {
                    var user = await userManager.FindByEmailAsync(loginVM.Email);
                    if (await userManager.IsInRoleAsync(user, SD.RoleAdmin))
                    {
                        return RedirectToAction("Index", "Dashboard");
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(loginVM.RedirectUrl))
                        {
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            return LocalRedirect(loginVM.RedirectUrl);
                        }
                    }

                }
                else
                {
                    ModelState.AddModelError("", "Invalid login attempt");
                }
            }

            return View(loginVM);

        }



        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
