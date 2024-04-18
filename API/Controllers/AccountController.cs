
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace API.Controllers
{
    public class AccountController: BaseApiController
    {
        private readonly DataContext _context;
        public ITokenService _tokenService { get; }

        public AccountController(DataContext context, ITokenService tokenService)
        {
            _tokenService = tokenService;
            _context = context;
        }


        // For user registration coming from my class registerDto ehich has property of username and passoword
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register([FromBody]RegisterDto registerDto)
        {
                // Here I am checking if the user is registered then I am returning the username is taken string
            if(await UserExists(registerDto.Username)) return BadRequest ("Username is taken");
            using var hmac=new HMACSHA512();
            var user=new AppUser
            {
                UserName=registerDto.Username.ToLower(),
                PasswordHash=hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt=hmac.Key
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return new UserDto
            {
                Username=user.UserName,
                Token=_tokenService.CreateToken(user),
            };
        }


        // Here I am trying to log in
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {

            //Using single or default method to  returns the only element of a sequence that satisfies a specified condition (Inside parentheses our lambda method)
            //Or a default value(null) if no such element exists
            //this method throws an exception if more than one element satisfies the condition in our app this will not happen as I have made the username unique
            var user=await _context.Users.SingleOrDefaultAsync(x => x.UserName ==loginDto.Username);


            //if there is default value(null) the it returns Unauthorized 401 response
            if(user==null) return Unauthorized("Invalid Username");

                //Now doing exactly reverse of what we did in registration
            using var hmac=new HMACSHA512(user.PasswordSalt);
            var computedHash=hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            //comparing computedHash with the actual hash and returing 401 if they are not equal

        for(int i=0;i<computedHash.Length;i++)
        {
            if (computedHash[i]!=user.PasswordHash[i]) return Unauthorized("Invalid Password");
        }
            return new UserDto
            {
                Username=user.UserName,
                Token=_tokenService.CreateToken(user),
            };
        }



        // I am checking thjt to see the user is already registered or not
        private async Task<bool> UserExists(string username)
        {
            return await _context.Users.AnyAsync(x=>x.UserName==username.ToLower());
        }
    }
}