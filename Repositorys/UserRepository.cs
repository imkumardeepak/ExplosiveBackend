using Peso_Baseed_Barcode_Printing_System_API.Interface;
using Microsoft.EntityFrameworkCore;
using Peso_Baseed_Barcode_Printing_System_API.DBContext;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Models.DTO;
using Peso_Baseed_Barcode_Printing_System_API.Repositories;

namespace Peso_Baseed_Barcode_Printing_System_API.Repositorys
{
	public class UserRepository : GenericRepository<User>, IUserRepository
	{
		private readonly ApplicationDbContext _context;
		public UserRepository(ApplicationDbContext context) : base(context){
			_context = context;
			}

		public async Task<List<UserViewModel>> GetAllUsersWithPermissions()
		{
			var usersWithPermissions = await _context.Users
									.Select(u => new UserViewModel
									{
										Id = u.Id,
										Username = u.Username,
										Password = u.PasswordHash, // Ensure sensitive data is handled securely
										Company_ID = u.Company_ID,
										Role = _context.RoleMasters.Where(e => e.RoleName == u.Role).Include(e => e.pageAccesses).FirstOrDefault() ?? new RoleMaster(),
									}).ToListAsync();




			if (usersWithPermissions == null)
				return null;


			return usersWithPermissions;
		}

		public async Task<User> GetByUsersIdPermissions(int id)
		{
			var usersWithPermissions = await _context.Users
									.Where(u => u.Id == id)
									.FirstOrDefaultAsync();


			if (usersWithPermissions == null)
				return null;


			return usersWithPermissions;
		}

		public async Task<UserViewModel> GetByUserName(string username)
		{
			var user = _context.Users.Where(e => e.Username == username).FirstOrDefault();

			if (user == null)
				return null;

			var userViewModel = new UserViewModel()
			{
				Id = user.Id,
				Username = user.Username,
				Password = user.PasswordHash, // Ensure sensitive data is handled securely
				Company_ID = user.Company_ID,
				Role = _context.RoleMasters.Where(e => e.RoleName == user.Role).Include(e => e.pageAccesses).FirstOrDefault() ?? new RoleMaster(),
			};

			return userViewModel;
		}

	   public async Task<UserViewModel> GetUsernamePassword(string username, string password)
		{
			var user = _context.Users.Where(e => e.Username == username && e.PasswordHash == password).FirstOrDefault();

			if (user == null)
				return null;

			var userViewModel = new UserViewModel()
			{
				Id = user.Id,
				Username = user.Username,
				Password = user.PasswordHash, // Ensure sensitive data is handled securely
				Company_ID = user.Company_ID,
				Role = _context.RoleMasters.Where(e => e.RoleName == user.Role).Include(e => e.pageAccesses).FirstOrDefault() ?? new RoleMaster(),
			};
			return userViewModel;

		}

		public Task deleteUser(int id)
		{
          return _context.Users.Where(e => e.Id == id).ExecuteDeleteAsync();
		}

	}
}


