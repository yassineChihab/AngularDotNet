using API.Data;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace API.Controllers
{
    public class AccountController : BaseController
    {
        private readonly DataContext context;
        private readonly ITokenService tokenService;

        public AccountController(DataContext context,ITokenService tokenService)
        {
            this.context = context;
            this.tokenService = tokenService;
        }
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> register(RegisterDto registerDto)
        {

            if(await userExist(registerDto.username.ToLower()))
            {
                return BadRequest("username is takin");
            }
            using var hmac = new HMACSHA512();

            var user = new AppUser
            {
                UserName = registerDto.username,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.password)),
                PasswordSalt = hmac.Key
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();
            return Ok(new UserDto
            {
                Username=user.UserName,
                Token=tokenService.GetToken(user)
            });

        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> login(LoginDto loginDto)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.UserName == loginDto.username);

            if (user == null) { return Unauthorized("invalid username"); };
            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash=hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.password));
            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i] ) { return Unauthorized("Invalid Password"); };
            }

            return Ok(new UserDto
            {
                Username = user.UserName,
                Token = tokenService.GetToken(user)
            });

        }



        private async Task<bool> userExist(string username)
        {
            return await context.Users.AnyAsync(u => u.UserName == username.ToLower()); 
        }
    }
}
