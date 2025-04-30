using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Entities.Identity;
using TaskManager.Repository.Identity;

namespace TaskManager.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SetupController : ControllerBase
    {
        private readonly AppIdentityDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<SetupController> _logger;

        public SetupController(AppIdentityDbContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, ILogger<SetupController> logger)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }


        [HttpGet]
        public async Task<ActionResult> GetAllRoles()
        {
            var Roles = await _roleManager.Roles.ToListAsync();
            return Ok(Roles);
        }
        [HttpPost]
        public async Task<ActionResult> CreateRole([FromBody] string roleName)
        {
            var RoleExists = await _roleManager.RoleExistsAsync(roleName);
            if (RoleExists) return BadRequest(new { error = "Role Already Exists" });
            var RoleResult = await _roleManager.CreateAsync(new IdentityRole(roleName));
            if (RoleResult.Succeeded)
            {
                _logger.LogInformation($"The Role {roleName} is Added Successfully");
                return Ok(new
                {
                    result = $"The Role {roleName} Is Added Successfully"
                });
            }
            else
            {
                _logger.LogInformation($"The Role {roleName} is Not Added");
                return BadRequest(new
                {
                    error = $"The Role {roleName} Is Not Added "
                });
            }

        }
        [HttpGet("Users")]
        public async Task<ActionResult> GetAllUsers()
        {
            var Users = await _userManager.Users.ToListAsync();
            return Ok(Users);
        }
        [HttpPost("AddUserToRole")]
        public async Task<ActionResult> AddUserToRole(string email, string roleName)
        {
            var User = await _userManager.FindByEmailAsync(email);
            if (User == null)
            {
                _logger.LogInformation($"The User {email} Does not Exist");
                return BadRequest(new { error = "No User With This Email" });

            }
            var RoleExists = await _roleManager.RoleExistsAsync(roleName);
            if (!RoleExists)
            {
                _logger.LogInformation($"The Role {roleName} Does not Exist");
                return BadRequest(new { error = "Role Does Not Exist" });
            }
            var Result = await _userManager.AddToRoleAsync(User, roleName);
            if (!Result.Succeeded) return BadRequest(Result);
            return Ok(new { result = $"{User.UserName} is {roleName} now" });
        }
        [HttpGet("GetUserRoles")]
        public async Task<ActionResult> GetUserRoles(string email)
        {
            var User = await _userManager.FindByEmailAsync(email);
            if (User == null)
            {
                _logger.LogInformation($"The User {email} Does not Exist");
                return BadRequest(new { error = "No User With This Email" });

            }
            var Roles = await _userManager.GetRolesAsync(User);
            if(Roles.Count == 0)
            {
                _logger.LogInformation($"The User {User.UserName} Does not Have Roles");
                return BadRequest(new { error = $"The User {User.DisplayName} Does not Have Roles" });
            }
            return Ok(Roles);
        }
        [HttpPost("RemoveUserFromRole")]
        public async Task<ActionResult> RemoveUserFromRole(string email ,string roleName)
        {
            var User = await _userManager.FindByEmailAsync(email);
            if (User == null)
            {
                _logger.LogInformation($"The User {email} Does not Exist");
                return BadRequest(new { error = "No User With This Email" });

            }
            var RoleExists = await _roleManager.RoleExistsAsync(roleName);
            if (!RoleExists)
            {
                _logger.LogInformation($"The Role {roleName} Does not Exist");
                return BadRequest(new { error = "Role Does Not Exist" });
            }
            var Result  = await _userManager.RemoveFromRoleAsync(User, roleName);
            if (!Result.Succeeded)
            {
                _logger.LogInformation($"The Role {roleName} is not able to be removed from user {User.DisplayName}");
                return BadRequest(new { error = $"The Role {roleName} is not able to be removed from user {User.DisplayName}" });
            }
            return Ok(new { result = $"The User {User.DisplayName} is not {roleName} anymore " });
        }


    }
}
