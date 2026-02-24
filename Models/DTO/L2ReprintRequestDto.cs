using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Peso_Baseed_Barcode_Printing_System_API.Entities;

namespace Peso_Based_Barcode_Printing_System_APIBased.Models.DTO
{
	public class L2ReprintRequestDto
	{
		public List<L2barcodegeneration>? ReprintData { get; set; }

		// Reason for reprint
		public string? Reason { get; set; }

		public int? NoOfCopies { get; set; } = 1;
	}



}
