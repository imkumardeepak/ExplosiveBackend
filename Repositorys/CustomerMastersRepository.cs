using Peso_Baseed_Barcode_Printing_System_API.Interface;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Peso_Baseed_Barcode_Printing_System_API.DBContext;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Models.DTO;
using Peso_Baseed_Barcode_Printing_System_API.Repositories;
using System.Linq.Expressions;

namespace Peso_Baseed_Barcode_Printing_System_API.Repositorys
{
	public class CustomerMastersRepository : GenericRepository<CustomerMaster>, ICustomerMastersRepository
	{

		private readonly IMapper _mapper1;
		private readonly ApplicationDbContext _context;
		public CustomerMastersRepository(IMapper mapper, ApplicationDbContext context, ApplicationDbContext context1, ApplicationDbContext context2, ApplicationDbContext context3, ApplicationDbContext context4, ApplicationDbContext context5, ApplicationDbContext context6, ApplicationDbContext context7) : base(context, context1, context2, context3, context4, context5, context6, context7)
		{
			_mapper1 = mapper;
			_context = context;
			}


		public async Task<CustomerViewModel> GetCustomerViewModelByIdAsync(int customerId)
		{
			return await _context.CustomerMaster
				.Where(c => c.Id == customerId)
				.Select(c => new CustomerViewModel
				{
					Id = c.Id,
					srno = c.srno,
					Cid = c.Cid,
					Cname = c.CName,
					Addr = c.Addr,
					Gstno = c.Gstno,
					State = c.State,
					City = c.City,
					District = c.District,
					Tahsil = c.Tahsil,
					Members = c.Members.Select(m => new CustMemberViewModel
					{
						Id = m.Id,
						Cid = m.Cid,
						Name = m.Name,
						Email = m.Email,
						Contactno = m.ContactNo
					}).ToList(),
					Magazines = c.Magazines.Select(m => new CustMagazineViewModel
					{
						Id = m.Id,
						Cid = m.Cid,
						Magazine = m.Magazine,
						License = m.License,
						Validity = m.Validity,
						Wt = m.Wt,
						Unit = m.Unit
					}).ToList()
				})
				.FirstOrDefaultAsync();
		}

		public async Task UpsertAndRemoveMembersAsync(IEnumerable<CustMemberViewModel> memberViewModels, int customerId)
		{
			// Fetch the existing members from the database for the given customer ID
			var existingMembers = await _context.CustMemberDetail
				.Where(m => m.Cid == customerId)
				.ToListAsync();

			// Find members to remove (those that are not in the updated member list)
			var membersToRemove = existingMembers
				.Where(m => !memberViewModels.Any(vm => vm.Id == m.Id))
				.ToList();

			// Remove the members that no longer exist in the updated data
			_context.CustMemberDetail.RemoveRange(membersToRemove);

			// Upsert the members (add/update)
			foreach (var memberViewModel in memberViewModels)
			{
				var existingMember = await _context.CustMemberDetail
					.FirstOrDefaultAsync(m => m.Id == memberViewModel.Id && m.Cid == customerId);

				if (existingMember != null)
				{
					// If the member exists, update the existing entity with the new values
					_context.Entry(existingMember).CurrentValues.SetValues(memberViewModel);
					_context.Entry(existingMember).State = EntityState.Modified;
				}
				else
				{
					// If the member does not exist, add a new member with the correct foreign key (Cid)
					var newMember = _mapper1.Map<CustMemberDetail>(memberViewModel);
					newMember.Cid = customerId; // Set the foreign key (Customer ID)
					await _context.CustMemberDetail.AddAsync(newMember);
				}
			}

			// Save changes to the database
			await _context.SaveChangesAsync();
		}

		public async Task UpsertAndRemoveMagazinesAsync(IEnumerable<CustMagazineViewModel> magazineViewModels, int customerId)
		{
			// Fetch the existing magazines from the database for the given customer ID
			var existingMagazines = await _context.CustMagazineDetail
				.Where(m => m.Cid == customerId)
				.ToListAsync();

			// Find magazines to remove (those that are not in the updated magazine list)
			var magazinesToRemove = existingMagazines
				.Where(m => !magazineViewModels.Any(vm => vm.Id == m.Id))
				.ToList();

			// Remove the magazines that no longer exist in the updated data
			_context.CustMagazineDetail.RemoveRange(magazinesToRemove);

			// Upsert the magazines (add/update)
			foreach (var magazineViewModel in magazineViewModels)
			{
				var existingMagazine = await _context.CustMagazineDetail
					.FirstOrDefaultAsync(m => m.Id == magazineViewModel.Id && m.Cid == customerId);

				if (existingMagazine != null)
				{
					// If the magazine exists, update the existing entity with the new values
					_context.Entry(existingMagazine).CurrentValues.SetValues(magazineViewModel);
					_context.Entry(existingMagazine).State = EntityState.Modified;
				}
				else
				{
					// If the magazine does not exist, add a new magazine with the correct foreign key (Cid)
					var newMagazine = _mapper1.Map<CustMagazineDetail>(magazineViewModel);
					newMagazine.Cid = customerId; // Set the foreign key (Customer ID)
					await _context.CustMagazineDetail.AddAsync(newMagazine);
				}
			}

			// Save changes to the database
			await _context.SaveChangesAsync();
		}


		public async Task<CustomerMaster> GetCustomerMasterByCname(string customerName)
		{
			return await _context.CustomerMaster
				.Include(cm => cm.Members)  // Load related Members
				.Include(cm => cm.Magazines) // Load related Magazines
				.AsSplitQuery()
				.FirstOrDefaultAsync(cm => cm.CName.ToUpper() == customerName.ToUpper());
		}

		public async Task<List<CustomerMaster>> GetCustomerDetailsWithAllDetails()
		{
			return await _context.CustomerMaster
				.Include(cm => cm.Members)  // Load related Members
				.Include(cm => cm.Magazines) // Load related Magazines
				.AsSplitQuery()
				.ToListAsync();
		}


		public async Task<List<string>> GetcustoNamesAsync()
		{
			return await _context.CustomerMaster
				.Select(p => p.CName)
				.ToListAsync();
		}


		public int GetMaxSrno()
		{
			return _context.CustomerMaster.Max(c => (int?)c.srno) ?? 0;
		}

	}
}


