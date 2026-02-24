using System.ComponentModel.DataAnnotations;

namespace Peso_Baseed_Barcode_Printing_System_API.Entities
{
	public class RoleMaster
	{
		public int Id { get; set; }

		[MaxLength(50)]
		public string? RoleName { get; set; }

		public List<PageAccess> pageAccesses { get; set; } = new List<PageAccess>();
	}

	public class PageAccess
	{
		public int Id { get; set; }

		public int RoleId { get; set; }

		[MaxLength(100)]
		public string? PageName { get; set; }

		public bool IsAdd { get; set; } = false;

		public bool IsEdit { get; set; } = false;

		public bool IsDelete { get; set; } = false;
	}
}
