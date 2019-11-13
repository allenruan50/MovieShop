using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MovieShop.API.DTO;
using MovieShop.Entities;
using MovieShop.Services;

namespace MovieShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _config;
        public UserController(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _config = configuration;
        }
        [HttpPost]
        public async Task<IActionResult> CreateUserAsync([FromBody]CreateUserDTO createUserDTO)
        {
            if(createUserDTO == null || string.IsNullOrEmpty(createUserDTO.Email) || string.IsNullOrEmpty(createUserDTO.Password))
            {
                return BadRequest();
            }
            var user = await _userService.CreateUser(createUserDTO.Email, createUserDTO.Password, createUserDTO.FirstName, createUserDTO.LastName);
            if(user == null)
            {
                return BadRequest("Email already exists");
            }
            //return Created(); go back when we have the get newly created user method
            return Ok("User created successful!");
        }
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> LoginAsync([FromBody]ValidateUserDTO validateUserDTO)
        {
            if(validateUserDTO == null || string.IsNullOrEmpty(validateUserDTO.Email) || string.IsNullOrEmpty(validateUserDTO.Password))
            {
                return BadRequest("Either Email or Password is missing.");
            }
            var user = await _userService.ValidateUser(validateUserDTO.Email, validateUserDTO.Password);
            if (user == null || user.Id ==0)
            {
                return Unauthorized("Password is wrong.");
            }
            return Ok(new { 
                token = GenerateToken(user)
            });
        }

        [Authorize]
        [HttpGet]
        [Route("{id}/purchases")]
        public async Task<IActionResult> GetUserPurchasedMovies(int Id)
        {
            var usermovies = await _userService.GetPurchases(Id);
            return Ok(usermovies);
        }

        private string GenerateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName),
                new Claim("alias", user.FirstName[0] + user.LastName[0].ToString()),
                new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName)
            };
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["TokenSettings:PrivateKey"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(_config["TokenSettings:ExpirationDays"]));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expires,
                SigningCredentials = credentials,
                Issuer = _config["TokenSettings:Issuer"],
                Audience = _config["TokenSettings:Audience"]
            };


            var encodedJwt = new JwtSecurityTokenHandler().CreateToken(tokenDescriptor);
            return new JwtSecurityTokenHandler().WriteToken(encodedJwt);
        }
    }
}