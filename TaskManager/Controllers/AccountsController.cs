using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using TaskManager.APIs.DTOs;
using TaskManager.Core.Entities.Identity;
using TaskManager.Core.Services;

namespace TaskManager.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountsController(UserManager<AppUser> userManager ,SignInManager<AppUser> signInManager, IMapper mapper , ITokenService tokenService , RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _tokenService = tokenService;
            _roleManager = roleManager;
        }


        [HttpPost("Register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto model)
        {
            var User = new AppUser()
            {
                DisplayName = model.DisplayName,
                Email = model.Email,
                UserName = (model.Email.Split('@')[0]),
                PhoneNumber = model.PhoneNumber
            };
            var Result = await _userManager.CreateAsync(User, model.Password);
            if (!Result.Succeeded)
                return BadRequest();
            //var MappedUser = _mapper.Map<UserDto>(User);
            await _userManager.AddToRoleAsync(User, "AppUser");
            var MappedUser = new UserDto()
            {
                DisplayName = User.DisplayName,
                Email = User.Email,
                Token = await _tokenService.CreateTokenAsync(User, _userManager,_roleManager),
                RoleName = await _userManager.GetRolesAsync(User)
            };
            return Ok(MappedUser);
        }
        [HttpPost("Login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto model)
        {
            var User = await _userManager.FindByEmailAsync(model.Email);
            if (User == null)
                return Unauthorized();
            var Result = await _signInManager.CheckPasswordSignInAsync(User, model.Password, false);
            if (!Result.Succeeded) return Unauthorized();
             
            return Ok(new UserDto()
                {
                    DisplayName = User.DisplayName,
                    Email = User.Email,
                    Token = await _tokenService.CreateTokenAsync(User, _userManager,_roleManager),
                    RoleName = await _userManager.GetRolesAsync(User)

            });
        }
        [HttpPost("RemoveUser")]
        public async Task<ActionResult> RemoveUser(string email)
        {
            var User = await _userManager.FindByEmailAsync(email);
            if (User == null) return BadRequest(new { error = "No User With This Email" });
            var Result = await _userManager.DeleteAsync(User);
            if (!Result.Succeeded) return BadRequest(new { error = $"This User {User.DisplayName} Cannot Be Removed" });
            return Ok(new {result = $"User {User.DisplayName} is Removed Successfully"});
        }
        [HttpPost("EditUser")]
        public async Task<ActionResult> EditUser(string email , string username)
        {
            var User = await _userManager.FindByEmailAsync(email);
            if (User == null) return BadRequest(new { error = "No User With This Email" });
            var OldUsername = User.UserName;
            var Result = await _userManager.SetUserNameAsync(User, username);
            if (!Result.Succeeded) return BadRequest(new { error = $"This Username {User.UserName} Cannot Be Changed" });
            return Ok(new { result = $"User {OldUsername} is Changed Successfully to {username}" });

        }
        


    }
}
