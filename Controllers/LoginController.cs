using Peso_Baseed_Barcode_Printing_System_API.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Models.DTO;
using Peso_Baseed_Barcode_Printing_System_API.Repositorys;
using Peso_Baseed_Barcode_Printing_System_API.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace Peso_Baseed_Barcode_Printing_System_API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[AllowAnonymous]
	public class LoginController : ControllerBase
	{
		private readonly IConfiguration _configuration;
		private readonly IDistributedCache _distributedCache;
		private readonly IUserRepository _userRepository;
		private APIResponse _APIResponse;
		private string KeyName = "UserMaster";


		public LoginController(IConfiguration configuration, IDistributedCache distributedCache, IUserRepository userRepository)
		{
			_configuration = configuration;
			_distributedCache = distributedCache;
			_APIResponse = new();
			_userRepository = userRepository;
		}

		[HttpPost("Login")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]

		public async Task<ActionResult<APIResponse>> Login([FromBody] LoginDTO model)
		{

			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest("Please Provide Username and Password !");
				}

				var password = EncryptionHelper.Encrypt(model.Password);

				var users = await _userRepository.GetUsernamePassword(model.Username, password);
				if (users != null)
				{
					var key = Encoding.ASCII.GetBytes(_configuration.GetValue<string>("JWTSecret"));
					var tokenHandler = new JwtSecurityTokenHandler();
					var tokenDescriptor = new SecurityTokenDescriptor()
					{
						Subject = new System.Security.Claims.ClaimsIdentity(new Claim[]
						{
								//username
								new Claim(ClaimTypes.Name,model.Username),
								new Claim("Id",users.Id.ToString()),
								new Claim("CompanyId",users.Company_ID),
								new Claim(ClaimTypes.Role,users.Role.RoleName ?? "Admin"),
						}),
						Expires = DateTime.UtcNow.AddMinutes(40),
						SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
					};

					var token = tokenHandler.CreateToken(tokenDescriptor);

					var userdata = new LoginResponseDTO()
					{
						user = users,
						token = tokenHandler.WriteToken(token)
					};

					_APIResponse.Data = userdata;
					_APIResponse.Status = true;
					_APIResponse.StatusCode = HttpStatusCode.OK;
					return Ok(_APIResponse);
				}
				else
				{
					_APIResponse.Errors.Add("Invalid Username or Password");
					_APIResponse.Message = "Invalid Username or Password";
					_APIResponse.StatusCode = HttpStatusCode.NotFound;
					_APIResponse.Status = false;
					return NotFound(_APIResponse);
				}
			}
			catch (Exception ex)
			{
				_APIResponse.Errors.Add(ex.Message);
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				_APIResponse.Status = false;
				return _APIResponse;
			}

		}


	}
}


