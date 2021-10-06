using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using Microsoft.AspNetCore.Mvc;
using API.Entities;
using System.Security.Cryptography;
using System.Text;
using API.DTOs;
using Microsoft.EntityFrameworkCore;
using API.Interfaces;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
                private readonly ITokenService _tokenservice;

        public AccountController(DataContext context , ITokenService ser)
        {
            _context = context;
            _tokenservice = ser;

        }

        [HttpPost("register")]
        public async Task<ActionResult<Userdto>> Register(RegisterDto RegDto )
        {
            if ( await IsUserExist(RegDto.Username)) return BadRequest("Username Already exist!");
            using var hmac = new HMACSHA512();
            
            var user = new AppUser{
            UserName=RegDto.Username.ToLower() ,
            PasswordHash= hmac.ComputeHash(Encoding.UTF8.GetBytes(RegDto.Password)) ,
            PasswordSalt = hmac.Key
            } ;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return new Userdto
            {
                UserName = user.UserName,
                Token = _tokenservice.CreateToken(user)
            };
            
        }
        [HttpPost("Login")]
        public async Task<ActionResult<Userdto>> Login(LoginDto loginDto)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName.ToLower() == loginDto.username.ToLower());
            if(user == null) return Unauthorized("Invalid Username");

            using var hmac = new HMACSHA512(user.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
            //Check the computedHash and hash store in databas is identical or not
            for (int i=0 ; i<computedHash.Length ; i++)
            {
                if(computedHash[i] != user.PasswordHash[i])
                {
                    return Unauthorized("invalid Password");
                }
            }
            return  new Userdto
            {
                UserName = user.UserName,
                Token = _tokenservice.CreateToken(user)
            };
        }
        private async Task<bool> IsUserExist(string username)
        {
            return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
        }

    }
}