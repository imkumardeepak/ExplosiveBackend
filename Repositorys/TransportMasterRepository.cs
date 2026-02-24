using Peso_Baseed_Barcode_Printing_System_API.Interface;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Peso_Baseed_Barcode_Printing_System_API.DBContext;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Models.DTO;
using Peso_Baseed_Barcode_Printing_System_API.Repositories;

namespace Peso_Baseed_Barcode_Printing_System_API.Repositorys
{
	public class TransportMasterRepository : GenericRepository<TransportMaster>, ITransportMasterRepository
	{

		private readonly IMapper _mapper;
		private readonly ApplicationDbContext _context;
		public TransportMasterRepository(IMapper mapper, ApplicationDbContext context, ApplicationDbContext context1, ApplicationDbContext context2, ApplicationDbContext context3, ApplicationDbContext context4, ApplicationDbContext context5, ApplicationDbContext context6, ApplicationDbContext context7) : base(context, context1, context2, context3, context4, context5, context6, context7)
		{
			_mapper = mapper;
			_context = context;
			}



		public async Task<TransportViewModel> GetTransportMasterViewModelByIdAsync(int transportMasterId)
		{
			return await _context.TransportMaster
				.Where(t => t.Id == transportMasterId)
				.Select(t => new TransportViewModel
				{
					Id = t.Id,
					TName = t.TName,
					Addr = t.Addr,
					Gstno = t.Gstno,
					State = t.State,
					City = t.City,
					District = t.District,
					Tahsil = t.Tahsil,
					Members = t.Members.Select(m => new TransportMemberViewModel
					{
						Id = m.Id,
						Cid = m.Cid,
						Name = m.Name,
						Email = m.Email,
						ContactNo = m.ContactNo
					}).ToList(),
					Vehicles = t.Vehicles.Select(v => new TransportVehicleViewModel
					{
						Id = v.Id,
						Cid = v.Cid,
						VehicleNo = v.VehicleNo,
						License = v.License,
						Validity = v.Validity,
						Wt = v.Wt,
						Unit = v.Unit
					}).ToList()
				})
				.FirstOrDefaultAsync();
		}

		public async Task<List<TransportMaster>> GetAllTransportMasterWithDetails()
		{
			return await _context.TransportMaster
				.Include(t => t.Members)
				.Include(t => t.Vehicles)
				.ToListAsync();
		}
		public async Task UpsertAndRemoveMembersAsync(IEnumerable<TransportMemberViewModel> memberViewModels, int transportMasterId)
		{
			// Fetch the existing members for the transport master
			var existingMembers = await _context.TransMemberDetail
				.Where(m => m.Cid == transportMasterId)
				.ToListAsync();

			// Find members to remove
			var membersToRemove = existingMembers
				.Where(m => !memberViewModels.Any(vm => vm.Id == m.Id))
				.ToList();

			// Remove members
			_context.TransMemberDetail.RemoveRange(membersToRemove);

			// Upsert members
			foreach (var memberViewModel in memberViewModels)
			{
				var existingMember = await _context.TransMemberDetail
					.FirstOrDefaultAsync(m => m.Id == memberViewModel.Id && m.Cid == transportMasterId);

				if (existingMember != null)
				{
					// Update existing member
					_context.Entry(existingMember).CurrentValues.SetValues(memberViewModel);
					_context.Entry(existingMember).State = EntityState.Modified;
				}
				else
				{
					// Add new member
					var newMember = _mapper.Map<TransMemberDetail>(memberViewModel);
					newMember.Cid = transportMasterId; // Set foreign key
					await _context.TransMemberDetail.AddAsync(newMember);
				}
			}

			// Save changes
			await _context.SaveChangesAsync();
		}

		public async Task UpsertAndRemoveVehiclesAsync(IEnumerable<TransportVehicleViewModel> vehicleViewModels, int transportMasterId)
		{
			// Fetch the existing vehicles for the transport master
			var existingVehicles = await _context.TransVehicleDetail
				.Where(v => v.Cid == transportMasterId)
				.ToListAsync();

			// Find vehicles to remove
			var vehiclesToRemove = existingVehicles
				.Where(v => !vehicleViewModels.Any(vm => vm.Id == v.Id))
				.ToList();

			// Remove vehicles
			_context.TransVehicleDetail.RemoveRange(vehiclesToRemove);

			// Upsert vehicles
			foreach (var vehicleViewModel in vehicleViewModels)
			{
				var existingVehicle = await _context.TransVehicleDetail
					.FirstOrDefaultAsync(v => v.Id == vehicleViewModel.Id && v.Cid == transportMasterId);

				if (existingVehicle != null)
				{
					// Update existing vehicle
					_context.Entry(existingVehicle).CurrentValues.SetValues(vehicleViewModel);
					_context.Entry(existingVehicle).State = EntityState.Modified;
				}
				else
				{
					// Add new vehicle
					var newVehicle = _mapper.Map<TransVehicleDetail>(vehicleViewModel);
					newVehicle.Cid = transportMasterId; // Set foreign key
					await _context.TransVehicleDetail.AddAsync(newVehicle);
				}
			}

			// Save changes
			await _context.SaveChangesAsync();
		}


		public async Task<List<TransVehicleDetail>> GetTransportVehicleDetailsByTransporterNameAsync(string transporterName)
		{
			var transportMasters = await _context.Set<TransportMaster>()
												 .Where(tm => tm.TName == transporterName)
												 .Include(tm => tm.Vehicles) // Ensure Vehicles are loaded
												 .SelectMany(tm => tm.Vehicles)
												 .ToListAsync();

			return transportMasters;
		}


	}
}


