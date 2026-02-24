using Microsoft.Build.Framework;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Models;
using User = Peso_Baseed_Barcode_Printing_System_API.Entities.User;

namespace Peso_Baseed_Barcode_Printing_System_API.Interface
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<List<UserViewModel>> GetAllUsersWithPermissions();

        Task <User> GetByUsersIdPermissions(int id);

        Task<UserViewModel> GetByUserName(string username);

        Task<UserViewModel> GetUsernamePassword(string email, string password);

        Task deleteUser(int id);


    }
}
