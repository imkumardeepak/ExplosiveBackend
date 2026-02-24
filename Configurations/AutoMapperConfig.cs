using AutoMapper;
using NetTopologySuite.Mathematics;
using Newtonsoft.Json;
using Peso_Based_Barcode_Printing_System_APIBased.Models;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Models.DTO;
using System.ComponentModel.DataAnnotations;

namespace Peso_Baseed_Barcode_Printing_System_API.Configurations
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<ProductMaster, ProductMaster>();
            CreateMap<BrandMaster, BrandMaster>();
            CreateMap<CountryMaster, CountryMaster>();
            CreateMap<IntimationMaster, IntimationMaster>();
            CreateMap<MachineCodeMaster, MachineCodeMaster>();
            CreateMap<MfgLocationMaster, MfgLocationMaster>();
            CreateMap<MfgMaster, MfgMaster>();
            CreateMap<PlantMaster, PlantMaster>();
            CreateMap<StateMaster, StateMaster>();
            CreateMap<ProductionPlan, ProductionPlan>();
            CreateMap<FormRE2Manallot, formre2magallot>();
            CreateMap<MagzineMaster, MagzineMaster>();
            CreateMap<PlantTypeMaster, PlantTypeMaster>().ReverseMap();
            CreateMap<ProductionPlan, ProductionPlanDto>().ReverseMap();
            CreateMap<ProductionPlan, CreateProductionPlanRequestDto>().ReverseMap();
            CreateMap<ProductionPlan, UpdateProductionPlanRequestDto>().ReverseMap();
            CreateMap<ProductionPlan, SearchProductionPlanRequestDto>().ReverseMap();



            CreateMap<CustomerViewModel, CustomerMaster>()
                .ForMember(dest => dest.Magazines, opt => opt.MapFrom(src =>
                    src.Magazines.Select(m => new CustMagazineDetail
                    {
                        // Map properties and set Cid
                        Id = m.Id,
                        Cid = src.Id,
                        Magazine = m.Magazine,
                        License = m.License,
                        Validity = m.Validity,
                        Wt = m.Wt,
                        Unit = m.Unit
                    })))
                .ForMember(dest => dest.Members, opt => opt.MapFrom(src =>
                    src.Members.Select(m => new CustMemberDetail
                    {
                        // Map properties and set Cid
                        Id = m.Id,
                        Cid = src.Id,
                        Name = m.Name,
                        Email = m.Email,
                        ContactNo = m.Contactno
                    })));

            CreateMap<CustMemberDetail, CustMemberViewModel>().ReverseMap();
            CreateMap<CustMagazineDetail, CustMagazineViewModel>().ReverseMap();

            CreateMap<TransportViewModel, TransportMaster>()
                .ForMember(dest => dest.Vehicles, opt => opt.MapFrom(src =>
                src.Vehicles.Select(v => new TransVehicleDetail
                {
                    // Map properties and set Cid
                    Id = v.Id,
                    Cid = src.Id,
                    VehicleNo = v.VehicleNo,
                    License = v.License,
                    Validity = v.Validity,
                    Wt = v.Wt,
                    Unit = v.Unit
                })))
            .ForMember(dest => dest.Members, opt => opt.MapFrom(src =>
                src.Members.Select(m => new TransMemberDetail
                {
                    // Map properties and set Cid
                    Id = m.Id,
                    Cid = src.Id,
                    Name = m.Name,
                    Email = m.Email,
                    ContactNo = m.ContactNo
                })));

            CreateMap<TransMemberDetail, TransportMemberViewModel>().ReverseMap();
            CreateMap<TransVehicleDetail, TransportVehicleViewModel>().ReverseMap();

            CreateMap<TransitTruckViewModel, TransitTruckMaster>()
                    .ForMember(dest => dest.Vehicles, opt => opt.MapFrom(src =>
                        src.Vehicles.Select(v => new TransitTruckDetails
                        {
                            Id = v.Id,
                            Cid = src.Id,
                            VehicleNo = v.VehicleNo,
                            License = v.License,
                            Validity = v.Validity,
                            Wt = v.Wt,
                            Unit = v.Unit
                        })))
                    .ForMember(dest => dest.Members, opt => opt.MapFrom(src =>
                        src.Members.Select(m => new TransitMemberDetails
                        {
                            Id = m.Id,
                            Cid = src.Id,
                            Name = m.Name,
                            Email = m.Email,
                            ContactNo = m.ContactNo
                        })));
            CreateMap<TransitTruckDetails, TransitTruckDetailViewModel>().ReverseMap();
            CreateMap<TransitMemberDetails, TransitMemberViewModel>().ReverseMap();

            //CreateMap<TransitTruckViewModel, TransitTruckMaster>()
            //.ForMember(dest => dest.Vehicles, opt => opt.MapFrom(src => src.Vehicles))
            //.ForMember(dest => dest.Members, opt => opt.MapFrom(src => src.Members));

            //CreateMap<TransitTruckDetailViewModel, TransitTruckDetails>();
            //CreateMap<TransMemberDetail, TransitMemberDetails>();

            //CreateMap<TransitTruckDetails, TransitTruckDetailViewModel>().ReverseMap();
            //CreateMap<TransitMemberDetails, TransitMemberViewModel>().ReverseMap();



            // Map TransitTruckDetails to TransitTruckDetailViewModel and vice versa




            CreateMap<ManualMagAllot, List<Magzine_Stock>>()
            .ConvertUsing(src => src.SmallModels.Select(v => new Magzine_Stock
            {
                L1Barcode = v.L1Barcode,
                MagName = src.Magname,
                BrandName = src.BrandName,
                BrandId = src.BrandId,
                PrdSize = src.ProductSize,
                PSizeCode = src.PSizeCode,
                Stock = 1,
                StkDt = DateTime.Now.Date,
                Re2 = false,
                Re12 = false
            }).ToList());

            CreateMap<formre2magallot, List<formre2magallot>>()
            .ConvertUsing(src =>
                src.SmallModels
                    .Select((sm, index) => new formre2magallot
                    {
                        srno = index + 1,
                        l1barcode = sm.L1Barcode,
                        l1netwt = (decimal?)sm.L1NetWt,
                        plantname = src.plantname,
                        pcode = src.pcode,
                        bname = src.bname,
                        bid = src.bid,
                        productsize = src.productsize,
                        psize = src.psize,
                        shift = src.shift,
                        //dateoftest = src.dateoftest,
                        magname = src.magname,
                        maglic = src.maglic,
                        mfgdt = src.mfgdt,
                        gendt = src.gendt,
                        month = src.month,
                        quarter = src.quarter,
                        year = src.year,
                        re2 = 0,
                        re12 = 0,
                        @class = src.@class,
                        div = src.div
                    }).ToList()
            );


            CreateMap<RE2DDAllot, List<Magzine_Stock>>()
            .ConvertUsing(src => src.L1Data.Select(v => new Magzine_Stock
            {
                L1Barcode = v.L1barcode,
                MagName = src.Magname,
                BrandName = src.BrandName,
                BrandId = src.BrandId,
                PrdSize = src.ProductSize,
                PSizeCode = src.PSizeCode,
                Stock = 1,
                StkDt = DateTime.Now.Date,
                Re2 = false,
                Re12 = false
            }).ToList());



            // Map ViewModel to Entity
            CreateMap<Re11IndentInfoViewModel, Re11IndentInfo>()
                .ForMember(dest => dest.IndentItems, opt => opt.MapFrom(src => src.PrdInfoList));

            CreateMap<Re11IndentPrdInfoViewModel, Re11IndentPrdInfo>();

            CreateMap<Re11IndentInfoViewModel, List<Re11IndentPrdInfo>>()
            .ConvertUsing(src => src.PrdInfoList.Select(v => new Re11IndentPrdInfo
            {
                IndentNo = src.IndentNo,
                IndentDt = src.IndentDt,
                Ptype = v.Ptype,
                PtypeCode = v.PtypeCode,
                Bname = v.Bname,
                Bid = v.Bid,
                Psize = v.Psize,
                SizeCode = v.SizeCode,
                Class = (int)v.Class,
                Div = (int)v.Div,
                L1NetWt = (decimal)v.L1NetWt,
                Unit = v.LoadUnit,
                RemWt = (decimal?)v.RemWt,
                ReqWt = (decimal)v.ReqWt,
                LoadWt = (decimal?)v.LoadWt,
                Remcase = v.remCase,
                ReqCase = v.reqCase,
                Loadcase = v.LoadCase
            }).ToList());


            // Reverse Mapping (if needed)
            CreateMap<Re11IndentInfo, Re11IndentInfoViewModel>()
                .ForMember(dest => dest.PrdInfoList, opt => opt.MapFrom(src => src.IndentItems));

            CreateMap<Re11IndentPrdInfo, Re11IndentPrdInfoViewModel>();


            CreateMap<DD11ViewModel, List<DispatchDownload>>()
            .ConvertUsing(src => src.DDTransList.Select(v => new DispatchDownload
            {
                Barcode = v.Barcode,
                TruckNo = v.TruckNo,
                IndentNo = v.IndentNo,
                Mag = v.Mag,
                Bid = v.Bid,
                SizeCode = v.SizeCode,
                ScanFlag = v.ScanFlag,
            }).ToList());

            CreateMap<TruckToMAGBarcodeViewModel, List<TruckToMAGBarcode>>()
            .ConvertUsing(src => src.TruckToMAGBarcodes.Select(v => new TruckToMAGBarcode
            {
                Barcode = v.Barcode,
                TruckNo = v.TruckNo,
                MagName = v.MagName,
                ScanFlag = v.ScanFlag
            }).ToList());
        }
    }
}

