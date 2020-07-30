using AspNet.Security.OAuth.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenIddictServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenIddictServer.Controllers
{
    [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public UsersController(
                UserManager<ApplicationUser> userManager,
                SignInManager<ApplicationUser> signInManager,
                ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [HttpPost("~/api/register"), Produces("application/json")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegistrationViewModel model)
        {
            //bool hasPermission = await userHasRole("Admin");
            //if (!hasPermission) return BadRequest();

            //var user = new ApplicationUser { FirstName = model.Firstname, LastName = model.Lastname, UserName = model.Username, Email = model.Username };
            //var result = await _userManager.CreateAsync(user, model.Password);

            bool created = await AddUserToRole(model.Username, model.Password, model.Role, model.Name);

            if (created)
            {
                return Ok();
            }

            // If we got this far, something failed, send back badrequest
            return BadRequest();
        }

        // GET: users
        [HttpGet("~/api/users")]
        public async Task<IEnumerable<ApplicationUser>> GetAsync()
        {
            bool hasPermission = await userHasRole("Admin");
            if (!hasPermission) return null;

            return await _context.Users.Select(c => new ApplicationUser
            {
                Id = c.Id,
                Email = c.Email,
                Name = c.Name,
            }).ToListAsync();
        }

        private async Task<bool> userHasRole(string role)
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            IEnumerable<string> roles = await _userManager.GetRolesAsync(user);

            return roles.Contains(role);
        }

        /// <summary>
        /// Add user to a role if the user exists, otherwise, create the user and adds him to the role.
        /// </summary>
        /// <param name="userEmail">User Email</param>
        /// <param name="userPwd">User Password. Used to create the user if not exists.</param>
        /// <param name="roleName">Role Name</param>
        /// <param name="firstName">First Name</param>
        /// <param name="lastName">Last Name</param>
        private async Task<bool> AddUserToRole(string userEmail, string userPwd, string roleName, string name)
        {
            var checkAppUser = await _userManager.FindByEmailAsync(userEmail);

            ApplicationUser appUser = null;

            if (checkAppUser == null)
            {
                var newAppUser = new ApplicationUser
                {
                    UserName = userEmail,
                    NormalizedUserName = userEmail,
                    Email = userEmail,
                    NormalizedEmail = userEmail,
                    Name = name,
                    EmailConfirmed = true,
                    LockoutEnabled = false,
                    SecurityStamp = Guid.NewGuid().ToString()
                };

                var taskCreateAppUser = await _userManager.CreateAsync(newAppUser, userPwd);

                if (taskCreateAppUser.Succeeded)
                {
                    appUser = newAppUser;

                    // add role if not user
                    if (!String.IsNullOrEmpty(roleName) && roleName.ToLower() != "user")
                    {
                        await _userManager.AddToRoleAsync(appUser, roleName);
                    }

                    return true;
                }
            }

            return false;
        }

        // // GET book/5
        // [HttpGet("{id}", Name = "GetBook")]
        // public IActionResult Get(int id)
        // {
        //     var book = _bookRepository.GetById(id);
        //     if (book == null)
        //     {
        //         return NotFound();
        //     }

        //     return Ok(book);
        // }
    }
}