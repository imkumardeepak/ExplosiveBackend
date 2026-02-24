using Peso_Baseed_Barcode_Printing_System_API.Interface;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Peso_Baseed_Barcode_Printing_System_API.DBContext;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Models.DTO;
using Peso_Baseed_Barcode_Printing_System_API.Repositories;

namespace Peso_Baseed_Barcode_Printing_System_API.Repositorys
{

	public class TransitTruckMasterRepository : GenericRepository<TransitTruckMaster>, ITransitTruckMasterRepository
    {

        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;
        public TransitTruckMasterRepository(IMapper mapper, ApplicationDbContext context, ApplicationDbContext context1, ApplicationDbContext context2, ApplicationDbContext context3, ApplicationDbContext context4, ApplicationDbContext context5, ApplicationDbContext context6, ApplicationDbContext context7) : base(context, context1, context2, context3, context4, context5, context6, context7)
        {
            _mapper = mapper;
            _context = context;
            }



        public async Task<TransitTruckViewModel> GetTransportMasterViewModelByIdAsync(int transportMasterId)
        {
            var data = await _context.TransitTruckMaster
                .Where(t => t.Id == transportMasterId)
                .Select(t => new TransitTruckViewModel
                {
                    Id = t.Id,
                    TName = t.TName,
                    Addr = t.Addr,
                    Gstno = t.Gstno,
                    State = t.State,
                    City = t.City,
                    District = t.District,
                    Tahsil = t.Tahsil,
                    Members = t.Members.Select(m => new TransitMemberViewModel
                    {
                        Id = m.Id,
                        Cid = m.Id,
                        Name = m.Name,
                        Email = m.Email,
                        ContactNo = m.ContactNo
                    }).ToList(),
                    Vehicles = t.Vehicles.Select(v => new TransitTruckDetailViewModel
                    {
                        Id = v.Id,
                        Cid = v.Id,
                        VehicleNo = v.VehicleNo,
                        License = v.License,
                        Validity = v.Validity,
                        Wt = v.Wt,
                        Unit = v.Unit
                    }).ToList()
                })
                .FirstOrDefaultAsync();
            return data;
        }

        public async Task InsertAndRemoveMembersAsync(IEnumerable<TransitMemberViewModel> memberViewModels, int transportMasterId)
        {
            // Fetch the existing members for the transport master
            var existingMembers = await _context.TransitMemberDetails
                .Where(m => m.Id == transportMasterId)
                .ToListAsync();

            // Find members to remove
            var membersToRemove = existingMembers
                .Where(m => !memberViewModels.Any(vm => vm.Id == m.Id))
                .ToList();

            // Remove members
            _context.TransitMemberDetails.RemoveRange(membersToRemove);

            // Upsert members
            foreach (var memberViewModel in memberViewModels)
            {
                var existingMember = await _context.TransitMemberDetails
                    .FirstOrDefaultAsync(m => m.Id == memberViewModel.Id && m.Id == transportMasterId);

                if (existingMember != null)
                {
                    // Update existing member
                    _context.Entry(existingMember).CurrentValues.SetValues(memberViewModel);
                    _context.Entry(existingMember).State = EntityState.Modified;
                }
                else
                {
                    // Add new member
                    var newMember = _mapper.Map<TransitMemberDetails>(memberViewModel);
                    newMember.Cid = transportMasterId; // Set foreign key
                    await _context.TransitMemberDetails.AddAsync(newMember);
                }
            }

            // Save changes
            await _context.SaveChangesAsync();
        }

        public async Task InsertAndRemoveVehiclesAsync(IEnumerable<TransitTruckDetailViewModel> vehicleViewModels, int transportMasterId)
        {
            // Fetch the existing vehicles for the transport master
            var existingVehicles = await _context.TransitTruckDetails
                .Where(v => v.Id == transportMasterId)
                .ToListAsync();

            // Find vehicles to remove
            var vehiclesToRemove = existingVehicles
                .Where(v => !vehicleViewModels.Any(vm => vm.Id == v.Id))
                .ToList();

            // Remove vehicles
            _context.TransitTruckDetails.RemoveRange(vehiclesToRemove);

            // Upsert vehicles
            foreach (var vehicleViewModel in vehicleViewModels)
            {
                var existingVehicle = await _context.TransitTruckDetails
                    .FirstOrDefaultAsync(v => v.Id == vehicleViewModel.Id && v.Id == transportMasterId);

                if (existingVehicle != null)
                {
                    // Update existing vehicle
                    _mapper.Map(vehicleViewModel, existingVehicle);
                    _context.Entry(existingVehicle).State = EntityState.Modified;
                }
                else
                {
                    // Add new vehicle
                    var newVehicle = _mapper.Map<TransitTruckDetails>(vehicleViewModel);
                    newVehicle.Id = transportMasterId; // Set foreign key
                    await _context.TransitTruckDetails.AddAsync(newVehicle);
                }
            }

            // Save changes
            await _context.SaveChangesAsync();
        }


        public async Task<List<TransitTruckDetails>> GetTransittruckDetailsAsync(string transporterName)
        {
            var transportMasters = await _context.Set<TransitTruckMaster>()
                                                 .Where(tm => tm.TName == transporterName)
                                                 .Include(tm => tm.Vehicles) // Ensure Vehicles are loaded
                                                 .Select(tm => new TransitTruckDetails
                                                 {
                                                     VehicleNo = tm.Vehicles.ToString(),

                                                 })
                                                 .ToListAsync();

            return transportMasters;
        }


      


    }
}


