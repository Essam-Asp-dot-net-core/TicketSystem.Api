using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Ticket.Core.Entity;
using Ticket.Repository.Data.AppDbContext;
using TicketSystem.Api.DTOs;

namespace TicketSystem.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly ApplicationDbContext _dbContext;
		private readonly UserManager<User> _userManager;
		private readonly SignInManager<User> _signInManager;
		private readonly IConfiguration _configuration;

		public AuthController(ApplicationDbContext dbContext , UserManager<User> userManager , SignInManager<User> signInManager , IConfiguration configuration)
		{
			_dbContext = dbContext;
			_userManager = userManager;
			_signInManager = signInManager;
			_configuration = configuration;
		}

		[HttpPost("register")]
		public async Task<IActionResult>Register(RegisterDTO model)
		{
			var exist = await _userManager.FindByNameAsync(model.Username);
			if (exist != null)
				return BadRequest("Your Name Is Exist");

			var user = new User
			{
				FullName = model.Username,
				Role = model.Role,
			};
			var result = await _userManager.CreateAsync(user , model.Password);
			if (!result.Succeeded)
				return BadRequest(result.Errors);
			return Ok("Register Successfully");
		}

		[HttpPost("login")]
		public async Task<IActionResult>Login(LoginDTO dto)
		{
			var user =await _userManager.FindByNameAsync(dto.Username);
			if(user == null)
				return Unauthorized("This Name Is InValid");
			var UserLogin = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
			if (!UserLogin.Succeeded)
				return Unauthorized("Password is InCorrect");
			var Claims = new List<Claim>
			{
				new Claim(ClaimTypes.GivenName, user.FullName),
				new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
				new Claim(ClaimTypes.Role, user.Role)

			};
			var Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
			var credential = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256Signature);

			var token = new JwtSecurityToken(

				issuer: _configuration["Jwt:Issure"],
				audience: _configuration["Jwt:Audience"],
				claims:Claims,
				expires: DateTime.Now.AddDays(double.Parse(_configuration["Jwt:Expire"])),
				signingCredentials:credential
				);

			return Ok(new {token = new JwtSecurityTokenHandler().WriteToken(token)});


		}
	}
}
