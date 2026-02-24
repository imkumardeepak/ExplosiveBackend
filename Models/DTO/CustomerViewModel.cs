using System.ComponentModel.DataAnnotations.Schema;

namespace Peso_Baseed_Barcode_Printing_System_API.Models.DTO
{
	[NotMapped]
	public class CustomerViewModel
	{
		public int Id { get; set; }
		public int srno { get; set; }
		public string Cid { get; set; }
		public string Gstno { get; set; }
		public string Cname { get; set; }
		public string Addr { get; set; }
		public string State { get; set; }
		public string City { get; set; }
		public string District { get; set; }
		public string Tahsil { get; set; }
		public List<CustMemberViewModel> Members { get; set; }
		public List<CustMagazineViewModel> Magazines { get; set; }

		public CustomerViewModel()
		{
			Members = new List<CustMemberViewModel>();
			Magazines = new List<CustMagazineViewModel>();
		}
	}

	public class CustMemberViewModel
	{
		public int Id { get; set; } // Add Id for CustMember
		public int Cid { get; set; } // This should match the Customer Id
		public string Name { get; set; }
		public string Email { get; set; }
		public string Contactno { get; set; }
	}

	public class CustMagazineViewModel
	{
		public int Id { get; set; } // Add Id for CustMagazine
		public int Cid { get; set; } // This should match the Customer Id
		public string Magazine { get; set; }
		public string License { get; set; }
		public string Validity { get; set; }
		public string Wt { get; set; }
		public string Unit { get; set; }
	}
}
