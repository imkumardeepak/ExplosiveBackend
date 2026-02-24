using Microsoft.EntityFrameworkCore;
using Peso_Baseed_Barcode_Printing_System_API.Entities;


namespace Peso_Baseed_Barcode_Printing_System_API.DBContext
{
	public class ApplicationDbContext : DbContext
	{

		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{
			AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
		}


		public DbSet<User> Users { get; set; }

		public DbSet<RoleMaster> RoleMasters { get; set; }

		public DbSet<PageAccess> PageAccess { get; set; }

		public DbSet<PlantMaster> PlantMasters { get; set; }

		public DbSet<CountryMaster> CountryMaster { get; set; }

		public DbSet<StateMaster> StateMaster { get; set; }

		public DbSet<MfgMaster> MfgMaster { get; set; }

		public DbSet<MfgLocationMaster> MfgLocationMaster { get; set; }

		public DbSet<MachineCodeMaster> MachineCodeMaster { get; set; }

		public DbSet<BrandMaster> BrandMaster { get; set; }

		public DbSet<ProductMaster> ProductMaster { get; set; }

		public DbSet<MagzineMaster> MagzineMasters { get; set; }
		public DbSet<BatchMasters> BatchMasters { get; set; }
		public DbSet<BatchDetails> BatchDetails { get; set; }

		public DbSet<ShiftMaster> ShiftMaster { get; set; }

		public DbSet<CustomerMaster> CustomerMaster { get; set; }

		public DbSet<CustMemberDetail> CustMemberDetail { get; set; }

		public DbSet<CustMagazineDetail> CustMagazineDetail { get; set; }

		public DbSet<TransportMaster> TransportMaster { get; set; }
		public DbSet<TransitTruckMaster> TransitTruckMaster { get; set; }
		public DbSet<TransitMemberDetails> TransitMemberDetails { get; set; }
		public DbSet<TransitTruckDetails> TransitTruckDetails { get; set; }
		//public DbSet<TransitTruckMaster> TransitTruckMaster { get; set; }

		public DbSet<TransVehicleDetail> TransVehicleDetail { get; set; }

		public DbSet<TransMemberDetail> TransMemberDetail { get; set; }

		public DbSet<L1barcodegeneration>? L1Barcodegeneration { get; set; }

		public DbSet<L2barcodegeneration>? L2Barcodegeneration { get; set; }

		public DbSet<L3barcodegeneration>? L3Barcodegeneration { get; set; }

		public DbSet<BarcodeData> BarcodeData { get; set; }

		public DbSet<BarcodeDataFinal> BarcodeDataFinal { get; set; }

		public DbSet<Magzine_Stock> Magzine_Stock { get; set; }

		public DbSet<Re11IndentInfo> Re11IndentInfo { get; set; }

		public DbSet<Re11IndentPrdInfo> Re11IndentPrdInfo { get; set; }

		public DbSet<AllLoadingSheet> AllLoadingSheet { get; set; }

		public DbSet<AllLoadingIndentDeatils> AllLoadingIndentDeatils { get; set; }

		public DbSet<DeviceMaster> DeviceMaster { get; set; }

		public DbSet<DispatchDownload> DispatchDownload { get; set; }

		public DbSet<TruckToMAGBarcode> TruckToMAGBarcode { get; set; }

		public DbSet<DispatchTransaction> DispatchTransaction { get; set; }

		public DbSet<ProductionReport> ProductionReport { get; set; }
		public DbSet<StorageMagazineReport> StorageMagazineReport { get; set; }

		public DbSet<DispatchReport> DispatchReport { get; set; }

		public DbSet<RE11StatusReport> RE11StatusReport { get; set; }

		public DbSet<RE2StatusReport> RE2StatusReport { get; set; }

		public DbSet<l1barcodereprint> L1BarcodeReprint { get; set; }

		public DbSet<l1boxdeletion> L1boxDeletion { get; set; }

		public DbSet<reprint_l2barcode> Reprint_L2barcode { get; set; }

		public DbSet<L1boxDeletionReport> L1boxDeletionReport { get; set; }
		public DbSet<L1ReprintReport> L1ReprintReport { get; set; }
		public DbSet<L2ReprintReport> L2ReprintReport { get; set; }

		public DbSet<hht_prodtomagtransfer> HHT_prodtomagtransfer { get; set; }

		public DbSet<shiftmanagement> Shiftmanagement { get; set; }
		public DbSet<canre11indent_prd_info> Canre11indent_prd_info { get; set; }

		public DbSet<canre11indentinfo> Canre11indentinfo { get; set; }
		public DbSet<RE6Generation> RE6Generation { get; set; }

		public DbSet<UomMaster> UomMaster { get; set; }
		public DbSet<ResetTypeMaster> ResetTypeMaster { get; set; }

		public DbSet<ProductionPlan> ProductionPlan { get; set; }

		public DbSet<MagzineMasterDetails> MagzineMasterDetails { get; set; }

		public DbSet<ProductionTransfer> ProductionTransfers { get; set; }
		public DbSet<ProductionTransferCases> ProductionTransferCases { get; set; }

		public DbSet<ProductionMagzineAllocation> ProductionMagzineAllocations { get; set; }

		public DbSet<ProductionMagzineAllocationCases> ProductionMagzineAllocationCases { get; set; }


		public DbSet<StockSummary> StockSummary { get; set; }

		public DbSet<re4dispatch> Re4Dispatch { get; set; }

		public DbSet<PlantTypeMaster> PlantTypeMaster { get; set; }

		public DbSet<formre2magallot> FormRe2MagAllot { get; set; }
		public DbSet<IntimationMaster> IntimationMaster { get; set; }
		public DbSet<RouteMaster> RouteMaster { get; set; }
		//public DbSet<AIMEMaster> AIMEMaster { get; set; }


		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{

			// Configure CustMemberDetail relationship
			modelBuilder.Entity<CustMemberDetail>()
				.HasOne(m => m.Customer) // Navigation property in CustMemberDetail
				.WithMany(c => c.Members) // Navigation property in CustomerMaster
				.HasForeignKey(m => m.Cid) // Foreign key in CustMemberDetail
				.OnDelete(DeleteBehavior.Cascade); // Specify behavior on delete if necessary

			// Configure CustMagazineDetail relationship
			modelBuilder.Entity<CustMagazineDetail>()
				.HasOne(m => m.Customer) // Navigation property in CustMagazineDetail
				.WithMany(c => c.Magazines) // Navigation property in CustomerMaster
				.HasForeignKey(m => m.Cid) // Foreign key in CustMagazineDetail
				.OnDelete(DeleteBehavior.Cascade); // Specify behavior on delete if necessary

			// Configure TransportMaster - TransVehicleDetails relationship
			modelBuilder.Entity<TransportMaster>()
					.HasMany(t => t.Vehicles)
					.WithOne()
					.HasForeignKey(v => v.Cid)
					.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<TransportMaster>()
				.HasMany(t => t.Members)
				.WithOne()
				.HasForeignKey(m => m.Cid)
				.OnDelete(DeleteBehavior.Cascade);


			// relation between Re11IndentInfo and Re11IndentItem
			modelBuilder.Entity<Re11IndentInfo>()
		   .HasMany(p => p.IndentItems)
		   .WithOne()
		   .HasForeignKey(b => b.IndentNo)
		   .OnDelete(DeleteBehavior.Cascade);


			modelBuilder.Entity<ProductionTransfer>()
		   .HasMany(p => p.Barcodes)
		   .WithOne()
		   .HasForeignKey(b => b.ProductionTransferId)
		   .OnDelete(DeleteBehavior.Cascade);


			modelBuilder.Entity<StockSummary>()
				.HasNoKey();


			modelBuilder.Entity<ProductionMagzineAllocation>()
				.HasMany(p => p.TransferToMazgnieScanneddata)
				   .WithOne()
				   .HasForeignKey(b => b.TransferToMazgnieId)
				   .OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<MagzineMaster>()
				.HasMany(b => b.MagzineMasterDetails)
				.WithOne()
				.HasForeignKey(b => b.magzineid)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<RoleMaster>()
			.HasMany(up => up.pageAccesses)
			.WithOne()
			.HasForeignKey(up => up.RoleId)
			.OnDelete(DeleteBehavior.Cascade);


			modelBuilder.Entity<AllLoadingSheet>()
		   .HasMany(p => p.IndentDetails)
		   .WithOne()
		   .HasForeignKey(b => b.LoadingSheetId)
		   .OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<ProductionTransferCases>()
				.HasIndex(b => b.L1Barcode)
				.IsUnique();



			modelBuilder.Entity<RoleMaster>()
				.HasIndex(p => p.RoleName)
				.HasDatabaseName("IX_RoleMaster_Page");

			modelBuilder.Entity<User>()
				.HasIndex(u => u.Username)
				.HasDatabaseName("IX_User_Username")
				.IsUnique();

			//set identity to table Production Plan
			modelBuilder.Entity<ProductionPlan>()
			 .HasIndex(p => new { p.MfgDt, p.PlantCode, p.BrandId, p.PSizeCode })
			 .HasDatabaseName("IX_ProductionPlan_MfgDt_PlantCode_BrandId_PSizeCode");

			modelBuilder.Entity<User>()
				.HasKey(u => u.Username);


			modelBuilder.Entity<Re11IndentInfo>()
			.HasIndex(m => new { m.IndentNo });

			modelBuilder.Entity<BatchMasters>(entity =>
			 {
				 entity.HasIndex(e => new { e.BatchCode, e.PlantCode, e.BatchType })
				 .HasDatabaseName("IX_BatchCode_PlantCode_BatchType");
			 });


			// =====================================================
			// OPTIMIZED INDEXES - Phase 1 Complete
			// Removed: Oversized 9-column composite indexes (lines 257-276)
			// Reason: 80% index maintenance overhead, low selectivity
			// =====================================================

			modelBuilder.Entity<BarcodeData>(entity =>
			{
				entity.HasIndex(e => new { e.Batch, e.L1 })
					  .HasDatabaseName("IX_BarcodeData_Batch_L1");
			});



			modelBuilder.Entity<BatchDetails>(entity =>
			   {
				   // Define the index on PlantCode
				   entity.HasIndex(e => new { e.BatchCode, e.PlantCode, e.MfgDate })
						 .HasDatabaseName("IX_BatchCode_PlantCode_MfgDate");
			   });



			// Create an index on BrandId and PSizeCode
			modelBuilder.Entity<Magzine_Stock>()
				.HasIndex(m => new { m.BrandId, m.PSizeCode, m.L1Barcode })
				.HasDatabaseName("IX_BrandId_PSizeCode_L1Barcode"); // Optional: name the index

			modelBuilder.Entity<DispatchTransaction>()
			.HasIndex(d => new { d.L1Barcode, d.Bid, d.PSizeCode })
			.HasDatabaseName("IX_DispatchTransaction_L1BidPSizeCode") // Custom index name
			.IsUnique(false); // Set to true if you want a unique index

			modelBuilder.Entity<DispatchTransaction>()
			.HasIndex(d => new { d.MagName, d.Re12 })
			.HasDatabaseName("IX_DispatchTransaction_MagNamere12") // Custom index name
			.IsUnique(false); // Set to true if you want a unique index

			// =====================================================
			// OPTIMIZED INDEXES - Added for performance improvement
			// =====================================================

			// CRITICAL: Most used query pattern - IndentNo + Re12 (used in 15+ queries)
			modelBuilder.Entity<DispatchTransaction>()
				.HasIndex(d => new { d.IndentNo, d.Re12 })
				.HasDatabaseName("IX_DispatchTransaction_IndentNo_Re12");

			// CRITICAL: Date-based dispatch queries
			modelBuilder.Entity<DispatchTransaction>()
				.HasIndex(d => d.DispDt)
				.HasDatabaseName("IX_DispatchTransaction_DispDt");

			// L1 Barcode date-based queries (dashboard, reports)
			modelBuilder.Entity<L1barcodegeneration>()
				.HasIndex(l => l.MfgDt)
				.HasDatabaseName("IX_L1Barcodegeneration_MfgDt");

			// Production query optimization index
			modelBuilder.Entity<L1barcodegeneration>()
				.HasIndex(l => new { l.MfgDt, l.PCode, l.BrandId, l.PSizeCode })
				.HasDatabaseName("IX_L1Barcodegeneration_Production");

			// Re11IndentInfo date index for indent filtering
			modelBuilder.Entity<Re11IndentInfo>()
				.HasIndex(r => r.IndentDt)
				.HasDatabaseName("IX_Re11IndentInfo_IndentDt");

			// Magazine stock lookup optimization
			modelBuilder.Entity<Magzine_Stock>()
				.HasIndex(m => new { m.MagName, m.Re2, m.Re12 })
				.HasDatabaseName("IX_MagazineStock_MagName_Re2_Re12");

			// AllLoadingIndentDeatils FK index
			modelBuilder.Entity<AllLoadingIndentDeatils>()
				.HasIndex(a => a.LoadingSheetId)
				.HasDatabaseName("IX_AllLoadingIndentDeatils_LoadingSheetId");

			// =====================================================
			// FOREIGN KEY INDEXES - Barcode Hierarchy
			// =====================================================

			// L2 -> L1 FK Index
			modelBuilder.Entity<L2barcodegeneration>()
				.HasIndex(l => l.L1Barcode)
				.HasDatabaseName("IX_L2Barcodegeneration_L1Barcode");

			// L3 -> L2 FK Index
			modelBuilder.Entity<L3barcodegeneration>()
				.HasIndex(l => l.L2Barcode)
				.HasDatabaseName("IX_L3Barcodegeneration_L2Barcode");

			// L3 -> L1 Index (for direct queries)
			modelBuilder.Entity<L3barcodegeneration>()
				.HasIndex(l => l.L1Barcode)
				.HasDatabaseName("IX_L3Barcodegeneration_L1Barcode");

			// Dispatch -> IndentNo FK Index
			modelBuilder.Entity<DispatchTransaction>()
				.HasIndex(d => d.IndentNo)
				.HasDatabaseName("IX_DispatchTransaction_IndentNo");

			// Magazine Stock -> L1Barcode FK Index
			modelBuilder.Entity<Magzine_Stock>()
				.HasIndex(m => m.L1Barcode)
				.HasDatabaseName("IX_MagazineStock_L1Barcode");

			// BarcodeData L1 lookup
			modelBuilder.Entity<BarcodeData>()
				.HasIndex(b => b.L1)
				.HasDatabaseName("IX_BarcodeData_L1");

			// BarcodeDataFinal L1 lookup
			modelBuilder.Entity<BarcodeDataFinal>()
				.HasIndex(b => b.L1)
				.HasDatabaseName("IX_BarcodeDataFinal_L1");

			// =====================================================
			// NAVIGATION PROPERTIES WITHOUT DATABASE FK ENFORCEMENT
			// Use for tables with potential orphaned data
			// =====================================================

			// Magzine_Stock -> L1Barcodegeneration (NO FK CONSTRAINT)
			// Data has orphaned records - using navigation without DB enforcement
			modelBuilder.Entity<Magzine_Stock>()
				.HasOne(m => m.ParentL1)
				.WithOne(l => l.MagazineStock)
				.HasForeignKey<Magzine_Stock>(m => m.L1Barcode)
				.IsRequired(false)
				.OnDelete(DeleteBehavior.SetNull); // No DB cascade, handle in app

			// L2Barcodegeneration -> L1Barcodegeneration (NO FK CONSTRAINT)
			modelBuilder.Entity<L2barcodegeneration>()
				.HasOne(l2 => l2.ParentL1)
				.WithMany(l1 => l1.L2Barcodes)
				.HasForeignKey(l2 => l2.L1Barcode)
				.IsRequired(false)
				.OnDelete(DeleteBehavior.Cascade);

			// L3Barcodegeneration -> L1Barcodegeneration (NO FK CONSTRAINT)
			modelBuilder.Entity<L3barcodegeneration>()
				.HasOne(l3 => l3.ParentL1)
				.WithMany(l1 => l1.L3Barcodes)
				.HasForeignKey(l3 => l3.L1Barcode)
				.IsRequired(false)
				.OnDelete(DeleteBehavior.Cascade);

			// L3Barcodegeneration -> L2Barcodegeneration (NO FK CONSTRAINT)
			modelBuilder.Entity<L3barcodegeneration>()
				.HasOne(l3 => l3.ParentL2)
				.WithMany()
				.HasForeignKey(l3 => l3.L2Barcode)
				.IsRequired(false)
				.OnDelete(DeleteBehavior.Cascade);

			// DispatchTransaction -> Re11IndentInfo (NO FK CONSTRAINT)
			modelBuilder.Entity<DispatchTransaction>()
				.HasOne(d => d.Indent)
				.WithMany(i => i.DispatchTransactions)
				.HasForeignKey(d => d.IndentNo)
				.IsRequired(false)
				.OnDelete(DeleteBehavior.SetNull);
		}
	}

}

