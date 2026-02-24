using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Peso_Baseed_Barcode_Printing_System_API.Entities;

namespace Peso_Baseed_Barcode_Printing_System_API.Models
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Company_ID { get; set; }
        public RoleMaster Role { get; set; }
        [ValidateNever]
        public string BearerToken { get; set; }
    }
}
