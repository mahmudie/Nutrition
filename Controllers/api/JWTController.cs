using DataSystem.Data;
using DataSystem.GLM.Dtos;
using DataSystem.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;

namespace DataSystem.Api.Controllers
{
   // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class JWTController : ControllerBase
    {
        protected readonly ApplicationDbContext _context;
        protected readonly IConfiguration _config;
        protected readonly SignInManager<ApplicationUser> _signInManager;

        public JWTController(
            ApplicationDbContext context,
            IConfiguration config,
            SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _config = config;
            _signInManager = signInManager;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApplicationUser>>> Index()
        {
            var users = await _context.Users
                .ToListAsync();

            return Ok(new
            {
                Users = users
            });
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("Authenticate")]
        public async Task<ActionResult> Authenticate([FromBody] Jwt dto)
        {
            var result = await _signInManager.PasswordSignInAsync(dto.Username, dto.Password, isPersistent: true, lockoutOnFailure: true);

            if (result.Succeeded)
            {

                // add token to the dto
                dto.Token = GenerateJSONWebToken(dto);

                // make sure password is empty
                dto.Password = null;

                return Ok(dto.Token);
            }

            if (result.IsLockedOut)
            {
                return BadRequest("This user is locked out.");
            }
            else
            {
                return BadRequest("Invalid login attempt.");
            }
        }
        private string GenerateJSONWebToken(Jwt userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Issuer"],
              null,
              expires: DateTime.Now.AddMinutes(120),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        protected string GenerateToken()
        {
            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];
            var expiry = DateTime.Now.AddMinutes(180);
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(issuer: issuer, audience: audience, expires: expiry, signingCredentials: credentials);
            var tokenHandler = new JwtSecurityTokenHandler();

            return tokenHandler.WriteToken(token);
        }
    }
}