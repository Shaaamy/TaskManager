using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManager.Core.Entities.Identity;
using TaskManager.Repository.Identity;

namespace TaskManager.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClaimsSetupController : ControllerBase
    {
        private readonly AppIdentityDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<ClaimsSetupController> _logger;
        public ClaimsSetupController(AppIdentityDbContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, ILogger<ClaimsSetupController> logger)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }
        [HttpGet]
        public async Task<ActionResult> GetAllClaims(string email)
        {
            var User = await _userManager.FindByEmailAsync(email);
            if (User == null) return BadRequest(new { error = "Email DoesNot Exist" });
            var Claims = await _userManager.GetClaimsAsync(User);
            if (Claims == null) return BadRequest(new { error = "Claims Does not Exist" });
            return Ok(Claims);
        }
        [HttpPost]
        public async Task<ActionResult> AddClaimsToUser(string email, string claimName, string claimValue)
        {
            
            var User = await _userManager.FindByEmailAsync(email);
            if (User == null) return BadRequest(new { error = "Email DoesNot Exist" });
            var UserClaim = new Claim(claimName, claimValue);
            var Result = await _userManager.AddClaimAsync(User, UserClaim);
            if (!Result.Succeeded) return BadRequest(new { error = "Can't Add The Claim" });
            return Ok(new {result="CLaim Added Successfully"});
        }
    }
}
