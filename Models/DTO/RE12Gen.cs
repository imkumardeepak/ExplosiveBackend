using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Peso_Baseed_Barcode_Printing_System_API.Models.DTO
{
	public class RE12Gen
	{
		public string LoadingSheet { get; set; }
		public string IndentNo { get; set; }

		public string TruckNo { get; set; }

		public string BrandName { get; set; }


		public string BrandId { get; set; }


		public string ProductSize { get; set; }



		public string PSizeCode { get; set; }

		public string Magname { get; set; }

		public int loadcase { get; set; }


	}

}
