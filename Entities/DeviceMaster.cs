using System.ComponentModel.DataAnnotations;

namespace Peso_Baseed_Barcode_Printing_System_API.Entities
{
	public class DeviceMaster
	{
		public int Id { get; set; }

		[MaxLength(50)]
		public string TypeOfDevice { get; set; }

		[MaxLength(20)]
		public string OS { get; set; }

		[MaxLength(50)]
		public string DeviceNo { get; set; }

		[MaxLength(100)]
		public string Devicename { get; set; }

		[MaxLength(50)]
		public string modelno { get; set; }

		[MaxLength(20)]
		public string version { get; set; }

		[MaxLength(50)]
		public string macid { get; set; }

		public int Port { get; set; }

		[MaxLength(500)]
		public string Spe_setting { get; set; }
	}
}
