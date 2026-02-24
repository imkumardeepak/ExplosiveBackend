using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OfficeOpenXml;
using Peso_Baseed_Barcode_Printing_System_API.Configurations;
using Peso_Baseed_Barcode_Printing_System_API.DBContext;
using Peso_Baseed_Barcode_Printing_System_API.Interface;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Models.DTO;
using Peso_Baseed_Barcode_Printing_System_API.Repositories;
using Peso_Baseed_Barcode_Printing_System_API.Repositorys;
using Peso_Baseed_Barcode_Printing_System_API.Services;
using Peso_Based_Barcode_Printing_System_API.Services;
using StackExchange.Redis;
using System.Text;

namespace Peso_Baseed_Barcode_Printing_System_API.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Excel License
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // Controllers
            services.AddControllers();
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });

            // Database
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
            });

            // Redis Cache
            services.AddDistributedRedisCache(options =>
            {
                options.Configuration = "localhost:6379";
            });

            // Redis Connection
            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("localhost:6379"));

            // WebSocket Service
            services.AddSingleton<WebSocketService>();

            // Health Checks
            services.AddHealthChecks();

            // CORS
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                });

                options.AddPolicy("AllowOnlyLocalhost", builder =>
                {
                    builder.WithOrigins("https://localhost:7256").AllowAnyHeader().AllowAnyMethod();
                });
            });

            // JWT Authentication
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration.GetValue<string>("JWTSecret"))),
                    ValidateIssuer = true,
                    ValidIssuer = configuration.GetValue<string>("JWT:Issuer"),
                    ValidateAudience = true,
                    ValidAudience = configuration.GetValue<string>("JWT:Audience")
                };
            });

            // AutoMapper
            services.AddAutoMapper(typeof(AutoMapperConfig));

            // Password Hasher
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

            // Swagger
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization Header using the bearer scheme. Enter Bearer [space] add your token in the text input",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Scheme = "Bearer"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = "Bearer",
                                Type = ReferenceType.SecurityScheme
                            },
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });
            });

            // Repositories
            AddRepositories(services);

            // Services
            AddCustomServices(services);

            return services;
        }

        private static void AddRepositories(IServiceCollection services)
        {
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            services.AddScoped<IRoleMasterRepository, RoleMasterRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPlantMasterRepository, PlantMasterRepository>();
            services.AddScoped<ICountryMasterRepository, CountryMasterRepository>();
            services.AddScoped<IStateMasterRepository, StateMasterRepository>();
            services.AddScoped<IMfgMasterRepository, MfgMasterRepository>();
            services.AddScoped<IMfgLocationMasterRepository, MfgLocationMasterRepository>();
            services.AddScoped<IMachineCodeMasterRepository, MachineCodeMasterRepository>();
            services.AddScoped<IBrandMasterRepository, BrandMasterRepository>();
            services.AddScoped<IProductMasterRepository, ProductMasterRepository>();
            services.AddScoped<IMagzineMasterRepository, MagzineMasterRepository>();
            services.AddScoped<IShiftMasterRepository, ShiftMasterRepository>();
            services.AddScoped<ICustomerMastersRepository, CustomerMastersRepository>();
            services.AddScoped<IBatchMasterRepository, BatchMasterRepository>();
            services.AddScoped<ICustMagazineDetailsRepository, CustMagazineDetailsRepository>();
            services.AddScoped<ICustMemberDetailsRepository, CustMemberDetailsRepository>();
            services.AddScoped<ITransportMasterRepository, TransportMasterRepository>();
            services.AddScoped<ITransMemberDetailRepository, TransMemberDetailRepository>();
            services.AddScoped<ITransVehicleDetailRepository, TransVehicleDetailRepository>();
            services.AddScoped<IL1barcodegenerationRepository, L1barcodegenerationRepository>();
            services.AddScoped<IL2barcodegenerationRepository, L2barcodegenerationRepository>();
            services.AddScoped<IL3barcodegenerationRepository, L3barcodegenerationRepository>();
            services.AddScoped<IBarcodeDataRepository, BarcodeDataRepository>();
            services.AddScoped<IBarcodeDataFinalRepository, BarcodeDataFinalRepository>();
            services.AddScoped<IMagzine_StockRepository, Magzine_StockRepository>();
            services.AddScoped<IRe11IndentInfoRepository, Re11IndentInfoRepository>();
            services.AddScoped<IRe11IndentPrdInfoRepository, Re11IndentPrdInfoRepository>();
            services.AddScoped<IAllLoadingSheetRepository, AllLoadingSheetRepository>();
            services.AddScoped<IDeviceMasterRepository, DeviceMasterRepository>();
            services.AddScoped<IDispatchDownloadRepository, DispatchDownloadRepository>();
            services.AddScoped<ITruckToMAGBarcodeRepository, TruckToMAGBarcodeRepository>();
            services.AddScoped<IDispatchTransactionRepository, DispatchTransactionRepository>();
            services.AddScoped<Il1boxdeletionRepository, l1boxdeletionRepository>();
            services.AddScoped<IProductReportRepository, ProductReportRepository>();
            services.AddScoped<IRe12Repository, Re12Repository>();
            services.AddScoped<APIResponse>();
            services.AddScoped<Il1barcodereprintRepository, l1barcodereprintRepository>();
            services.AddScoped<Ireprint_l2barcodeRepository, reprint_l2barcodeRepository>();
            services.AddScoped<Ihht_prodtomagtransferRepository, hht_prodtomagtransferRepository>();
            services.AddScoped<Iformre2magallotRepository, formre2magallotRepository>();
            services.AddScoped<IshiftmanagementRepository, shiftmanagementRepository>();
            services.AddScoped<Icanre11indent_prd_infoRepository, canre11indent_prd_infoRepository>();
            services.AddScoped<Icanre11indentinfoRepository, canre11indentinfoRepository>();
            services.AddScoped<IRE6GenerationRepository, RE6GenerationRepository>();
            services.AddScoped<IUomMasterRepository, UomMasterRepository>();
            services.AddScoped<ITransitTruckMasterRepository, TransitTruckMasterRepository>();
            services.AddScoped<ITransitMemberDetailsRepository, TransitMemberDetailsRepository>();
            services.AddScoped<ITransitTruckDetailsRepository, TransitTruckDetailsRepository>();
            services.AddScoped<IResetTypeMasterRepository, ResetTypeMasterRepository>();
            services.AddScoped<IProductionPlanRepository, ProductionPlanRepository>();
            services.AddScoped<IMagzineviewmodelRepository, MagzineviewmodelRepository>();
            services.AddScoped<IProductionMagzineAllocationRepository, ProductionMagzineAllocationRepository>();
            services.AddScoped<IPlantTypeMasterRepository, PlantTypeMasterRepository>();
            services.AddScoped<IIntimationMasterRepository, IntimationMasterRepository>();
            services.AddScoped<IRouteMasterRepository, RouteMasterRepository>();
            services.AddScoped<IBatchDetailsRepository, BatchDetailsRepository>();
        }

        private static void AddCustomServices(IServiceCollection services)
        {
            services.AddScoped<IProductionPlanService, ProductionPlanService>();
            services.AddScoped<PdfReaderService>();
            services.AddScoped<L1DetailsService>();
        }
    }
}
