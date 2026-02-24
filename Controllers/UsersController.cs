using Peso_Baseed_Barcode_Printing_System_API.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Peso_Baseed_Barcode_Printing_System_API.DBContext;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Models.DTO;
using Peso_Baseed_Barcode_Printing_System_API.Repositorys;
using Peso_Baseed_Barcode_Printing_System_API.Services;

namespace Peso_Baseed_Barcode_Printing_System_API.Controllers
{
	[Route("api/[controller]/[action]")]
    [ApiController]
   // [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleMasterRepository roleMasterRepository;
        private readonly IDistributedCache _distributedCache;
        private APIResponse _APIResponse;
        private string KeyName = "UserMaster";

        public UsersController(IDistributedCache distributedCache, IUserRepository userRepository, IRoleMasterRepository roleMasterRepository)
        {
            _userRepository = userRepository;
              roleMasterRepository = roleMasterRepository;
            _distributedCache = distributedCache;
            _APIResponse = new();
        }

        // GET: api/Users
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetAllUsers()
        {
            try
            {
				var users = await _userRepository.GetAllAsync();

				_APIResponse.Data = users;
				_APIResponse.Status = true;
				_APIResponse.Message = "Users retrieved successfully.";
				_APIResponse.StatusCode = HttpStatusCode.OK;
				return Ok(_APIResponse);
            }
            catch (Exception ex)
            {
                _APIResponse.Errors.Add(ex.Message);
                _APIResponse.StatusCode = HttpStatusCode.InternalServerError;
                _APIResponse.Status = false;
                return _APIResponse;
            }
        }




        // GET: api/Users/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetUserById(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid user ID.");

            try
            {
                
                User user = null;

               

                // If not found in cache, check repository
                if (user == null)
                {
                    user = await _userRepository.GetByUsersIdPermissions(id);
                    if (user == null)
                        return NotFound($"User not found with ID {id}.");
                }

                // Return user if found
                return Ok(new APIResponse { Data = user, Status = true, StatusCode = HttpStatusCode.OK });
            }
            catch (Exception ex)
            {
                _APIResponse.Errors.Add(ex.Message);
                _APIResponse.StatusCode = HttpStatusCode.InternalServerError;
                _APIResponse.Status = false;
                return _APIResponse;
            }
        }



        // GET: api/Users/5
        [HttpGet("{username}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetUserByName(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                _APIResponse.Data = $"Username cannot be null or empty.";
                _APIResponse.StatusCode = HttpStatusCode.OK;
                _APIResponse.Status = true;
                return _APIResponse;
            }

            try
            {
                UserViewModel user = null;

                // Check cache for user
                var encodedList = await _distributedCache.GetAsync(KeyName);
                if (encodedList != null)
                {
                    var cachedUsers = JsonConvert.DeserializeObject<List<UserViewModel>>(Encoding.UTF8.GetString(encodedList));
                    user = cachedUsers?.FirstOrDefault(u => u.Username == username);
                }

                // If not found in cache, query the repository
                if (user == null)
                {
                    await _distributedCache.RemoveAsync(KeyName);
                    var users = await _userRepository.GetAllUsersWithPermissions();
                    var SerializeList = JsonConvert.SerializeObject(users);
                    var EncodedList = Encoding.UTF8.GetBytes(SerializeList);
                    var option = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromDays(30)).SetAbsoluteExpiration(TimeSpan.FromDays(30));
                    _distributedCache.SetAsync(KeyName, EncodedList, option);

                    user = users.FirstOrDefault(u => u.Username == username);
                    if (user == null)
                        _APIResponse.Data = $"User not found with username '{username}'.";
                    _APIResponse.StatusCode = HttpStatusCode.OK;
                    _APIResponse.Status = false;
                    return _APIResponse;
                }

                // Return successful response
                _APIResponse.Data = user;
                _APIResponse.StatusCode = HttpStatusCode.OK;
                _APIResponse.Status = true;
                return _APIResponse;

            }
            catch (Exception ex)
            {
                _APIResponse.Errors.Add(ex.Message);
                _APIResponse.StatusCode = HttpStatusCode.InternalServerError;
                _APIResponse.Status = false;
                return _APIResponse;
            }
        }



        // POST: api/Users
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> PostUser(User userViewModel)
        {
            try
            {
				
                var existing = await _userRepository.GetByUserName(userViewModel.Username);

				if (existing != null)
				{
					_APIResponse.Errors.Add("Username already exists.");
                    _APIResponse.Message = "Username already exists.";
					_APIResponse.StatusCode = HttpStatusCode.BadRequest;
					_APIResponse.Status = false;
					return _APIResponse;
				}

				var NewUser = new User
                {
                    Username = userViewModel.Username,
                    PasswordHash = EncryptionHelper.Encrypt(userViewModel.PasswordHash),
					Company_ID = userViewModel.Company_ID,
					Role=userViewModel.Role
				};

                await _userRepository.AddAsync(NewUser);

                userViewModel.Id = NewUser.Id;

                _APIResponse.Data = userViewModel;
                _APIResponse.Status = true;
                _APIResponse.StatusCode = HttpStatusCode.OK;

                return CreatedAtAction("GetUserById", new { id = userViewModel.Id }, _APIResponse);
            }
            catch (Exception ex)
            {
                _APIResponse.Errors.Add(ex.Message);
                _APIResponse.StatusCode = HttpStatusCode.InternalServerError;
                _APIResponse.Status = false;
                return _APIResponse;
            }
        }




        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> PutUser(int id, User userViewModel)
        {
            try
            {
                // Check if the provided ID matches the ID in the view model
                if (id != userViewModel.Id)
                {
                   _APIResponse.Errors.Add("The provided ID does not match the ID in the view model.");
                    _APIResponse.Message = "The provided ID does not match the ID in the view model.";
					_APIResponse.StatusCode = HttpStatusCode.BadRequest;
					_APIResponse.Status = false;
					return _APIResponse;
                }

                // Get the user from the repository by ID
                var user = await _userRepository.GetByUsersIdPermissions(id);

                // If the user doesn't exist, return NotFound
                if (user == null)
                {
					_APIResponse.Errors.Add("User not found.");
					_APIResponse.StatusCode = HttpStatusCode.NotFound;
                    _APIResponse.Message = "User not found.";
					_APIResponse.Status = false;
					return _APIResponse;
                }


                // Update user details: username and password
                user.Username = userViewModel.Username; // Corrected: update the username from the view model
                user.PasswordHash = EncryptionHelper.Encrypt(userViewModel.PasswordHash); // Corrected: encrypt and set the password
                user.Company_ID = userViewModel.Company_ID; // Update the company ID
				user.Role = userViewModel.Role;

				// Update the user in the repository asynchronously
				await _userRepository.UpdateAsync(user); // Ensure async method is awaited

               

                _APIResponse.Message = $"Data Successfully Updated for User Id {userViewModel.Id}";
                _APIResponse.Data = userViewModel;
                _APIResponse.StatusCode = HttpStatusCode.OK;
                _APIResponse.Status = true;
                // Return a NoContent response indicating the user was updated successfully
                return Ok(_APIResponse);
            }
            catch (Exception ex)
            {
                // Handle any exceptions and return an internal server error response
                _APIResponse.Errors.Add(ex.Message);
                _APIResponse.StatusCode = HttpStatusCode.InternalServerError;
                _APIResponse.Status = false;
                return _APIResponse;
            }
        }





        // DELETE: api/Permissions/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> DeleteUser(int id)
        {
            try
            {
              

                var user = await _userRepository.GetByUsersIdPermissions(id);
                if (user == null)
                {
                    _APIResponse.Errors.Add("User not found.");
					_APIResponse.StatusCode = HttpStatusCode.NotFound;
					_APIResponse.Message = "User not found.";
					_APIResponse.Status = false;
					return _APIResponse;
				}
                    

                await _userRepository.deleteUser(user.Id);

                //delete 
                await _distributedCache.RemoveAsync(KeyName);

                _APIResponse.Data = true;
                _APIResponse.Status = true;
                _APIResponse.StatusCode = HttpStatusCode.OK;

                return Ok(_APIResponse);
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

