using System.ComponentModel.DataAnnotations;

namespace Peso_Baseed_Barcode_Printing_System_API.Models.DTO
{
	public class TransitTruckViewModel
	{
		public int Id { get; set; } // Unique identifier for the transport record

		[Required]
		[StringLength(100)]
		public string TName { get; set; } // Name of the transport

		[StringLength(255)]
		public string Addr { get; set; } // Address of the transport

		public string Gstno { get; set; }
		[StringLength(100)]
		public string State { get; set; } // State of the transport

		[StringLength(100)]
		public string City { get; set; } // City of the transport

		[StringLength(100)]
		public string District { get; set; } // District of the transport

		[StringLength(100)]
		public string Tahsil { get; set; } // Tahsil of the transport

		public List<TransitMemberViewModel> Members { get; set; } // List of members associated with the transport
		public List<TransitTruckDetailViewModel> Vehicles { get; set; } // List of vehicles associated with the transport

		public TransitTruckViewModel()
		{
			Members = new List<TransitMemberViewModel>();
			Vehicles = new List<TransitTruckDetailViewModel>();
		}
	}

	public class TransitMemberViewModel
	{
		public int Id { get; set; } // Unique identifier for the member

		public int Cid { get; set; } // Customer ID

		[Required]
		[StringLength(100)]
		public string Name { get; set; } // Name of the member

		[EmailAddress]
		[StringLength(100)]
		public string Email { get; set; } // Email of the member

		[StringLength(15)]
		public string ContactNo { get; set; } // Contact number of the member
	}

	public class TransitTruckDetailViewModel
	{
		public int Id { get; set; } // Unique identifier for the vehicle

		public int Cid { get; set; } // Customer ID

		[Required]
		[StringLength(100)]
		public string VehicleNo { get; set; } // Vehicle number

		[StringLength(100)]
		public string License { get; set; } // License information

		[StringLength(100)]
		public string Validity { get; set; } // Validity of the vehicle license

		[StringLength(50)]
		public string Wt { get; set; } // Weight of the vehicle

		[StringLength(50)]
		public string Unit { get; set; } // Unit of weight measurement
	}
}

