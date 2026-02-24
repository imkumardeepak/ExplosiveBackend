using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Peso_Baseed_Barcode_Printing_System_API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AllLoadingSheet",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LoadingSheetNo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Mfgdt = table.Column<DateTime>(type: "date", nullable: false),
                    TName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    TruckNo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    TruckLic = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    LicVal = table.Column<DateTime>(type: "date", nullable: false),
                    CreationDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Month = table.Column<int>(type: "integer", nullable: true),
                    Year = table.Column<int>(type: "integer", nullable: true),
                    Quarter = table.Column<int>(type: "integer", nullable: true),
                    Compflag = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllLoadingSheet", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BarcodeData",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    L1 = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    L2 = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    L3 = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Batch = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Re2 = table.Column<bool>(type: "boolean", nullable: false),
                    Re12 = table.Column<bool>(type: "boolean", nullable: false),
                    IsFinal = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BarcodeData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BarcodeDataFinal",
                columns: table => new
                {
                    L1 = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    L2 = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    L3 = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    BrandName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    BrandId = table.Column<string>(type: "varchar(4)", maxLength: 4, nullable: false),
                    PlantName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PlantCode = table.Column<string>(type: "varchar(2)", maxLength: 2, nullable: false),
                    ProductSize = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SizeCode = table.Column<string>(type: "varchar(3)", maxLength: 3, nullable: false),
                    MfgDt = table.Column<DateTime>(type: "date", nullable: false),
                    Shift = table.Column<string>(type: "varchar(1)", maxLength: 1, nullable: false),
                    Batch = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Re2 = table.Column<int>(type: "integer", nullable: false),
                    Re12 = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "BatchDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlantCode = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true),
                    MfgDate = table.Column<DateTime>(type: "date", nullable: false),
                    BatchCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    TotalCapacity = table.Column<decimal>(type: "numeric(12,4)", nullable: false),
                    RemainingCapacity = table.Column<decimal>(type: "numeric(12,4)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatchDetails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BatchMasters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlantName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PlantCode = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                    BatchSize = table.Column<decimal>(type: "numeric(12,4)", nullable: false),
                    unit = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    BatchCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    BatchType = table.Column<char>(type: "character(1)", nullable: true),
                    ResetType = table.Column<char>(type: "character(1)", nullable: true),
                    BatchFormat = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatchMasters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BrandMaster",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    plant_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    bname = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    bid = table.Column<string>(type: "varchar(4)", maxLength: 4, nullable: false),
                    Class = table.Column<int>(type: "integer", nullable: false),
                    Division = table.Column<int>(type: "integer", nullable: false),
                    unit = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrandMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Canre11indent_prd_info",
                columns: table => new
                {
                    indentno = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    indentdt = table.Column<DateTime>(type: "date", nullable: false),
                    pid = table.Column<int>(type: "integer", nullable: false),
                    ptype = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ptypecode = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false),
                    bname = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    bid = table.Column<string>(type: "varchar(4)", maxLength: 4, nullable: false),
                    psize = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    sizecode = table.Column<string>(type: "varchar(3)", maxLength: 3, nullable: false),
                    Class = table.Column<int>(type: "integer", nullable: false),
                    div = table.Column<int>(type: "integer", nullable: false),
                    l1netwt = table.Column<decimal>(type: "numeric(12,4)", nullable: false),
                    unit = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    reqwt = table.Column<decimal>(type: "numeric(12,4)", nullable: false),
                    requnit = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    remwt = table.Column<decimal>(type: "numeric(12,4)", nullable: false),
                    remunit = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    compflag = table.Column<int>(type: "integer", nullable: false),
                    loadwt = table.Column<decimal>(type: "numeric(12,4)", nullable: false),
                    loadunit = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Canre11indent_prd_info", x => x.indentno);
                });

            migrationBuilder.CreateTable(
                name: "Canre11indentinfo",
                columns: table => new
                {
                    indentno = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    indentdt = table.Column<DateTime>(type: "date", nullable: false),
                    custname = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    conname = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    conno = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    clic = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    compflag = table.Column<int>(type: "integer", nullable: false),
                    month = table.Column<string>(type: "varchar(2)", maxLength: 2, nullable: false),
                    year = table.Column<string>(type: "varchar(4)", maxLength: 4, nullable: false),
                    pesodt = table.Column<DateTime>(type: "date", nullable: false),
                    canceldttime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    cmonth = table.Column<string>(type: "varchar(2)", maxLength: 2, nullable: false),
                    cyear = table.Column<string>(type: "varchar(4)", maxLength: 4, nullable: false),
                    reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Canre11indentinfo", x => x.indentno);
                });

            migrationBuilder.CreateTable(
                name: "CountryMaster",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    cname = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    code = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CountryMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CustomerMaster",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Cid = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    srno = table.Column<int>(type: "integer", nullable: false),
                    CName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Addr = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Gstno = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    State = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    District = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Tahsil = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeviceMaster",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TypeOfDevice = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    OS = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    DeviceNo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Devicename = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    modelno = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    version = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    macid = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Port = table.Column<int>(type: "integer", nullable: false),
                    Spe_setting = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DispatchDownload",
                columns: table => new
                {
                    Barcode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    TruckNo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    IndentNo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Mag = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Bid = table.Column<string>(type: "varchar(4)", maxLength: 4, nullable: false),
                    SizeCode = table.Column<string>(type: "varchar(3)", maxLength: 3, nullable: false),
                    ScanFlag = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DispatchDownload", x => x.Barcode);
                });

            migrationBuilder.CreateTable(
                name: "DispatchReport",
                columns: table => new
                {
                    from = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    to = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    dispatchDt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    typeofreport = table.Column<string>(type: "text", nullable: false),
                    truck = table.Column<string>(type: "text", nullable: false),
                    magname = table.Column<string>(type: "text", nullable: false),
                    customername = table.Column<string>(type: "text", nullable: false),
                    re11indentno = table.Column<string>(type: "text", nullable: false),
                    L1Barcode = table.Column<string>(type: "text", nullable: false),
                    brandname = table.Column<string>(type: "text", nullable: false),
                    productsize = table.Column<string>(type: "text", nullable: false),
                    unit = table.Column<string>(type: "text", nullable: false),
                    netqty = table.Column<string>(type: "text", nullable: false),
                    currentstkdt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Month = table.Column<string>(type: "text", nullable: false),
                    Year = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DispatchReport", x => x.from);
                });

            migrationBuilder.CreateTable(
                name: "FormRe2MagAllot",
                columns: table => new
                {
                    l1barcode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    srno = table.Column<int>(type: "integer", nullable: true),
                    plantname = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    pcode = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true),
                    bname = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    bid = table.Column<string>(type: "varchar(4)", maxLength: 4, nullable: true),
                    productsize = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    psize = table.Column<string>(type: "varchar(3)", maxLength: 3, nullable: true),
                    shift = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    l1netwt = table.Column<decimal>(type: "numeric(12,4)", nullable: true),
                    dateoftest = table.Column<DateTime>(type: "date", nullable: true),
                    magname = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    maglic = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    mfgdt = table.Column<DateTime>(type: "date", nullable: true),
                    gendt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    month = table.Column<string>(type: "varchar(2)", maxLength: 2, nullable: true),
                    quarter = table.Column<string>(type: "varchar(2)", maxLength: 2, nullable: true),
                    year = table.Column<string>(type: "varchar(4)", maxLength: 4, nullable: true),
                    re2 = table.Column<int>(type: "integer", nullable: true),
                    re12 = table.Column<int>(type: "integer", nullable: true),
                    @class = table.Column<int>(name: "class", type: "integer", nullable: true),
                    div = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormRe2MagAllot", x => x.l1barcode);
                });

            migrationBuilder.CreateTable(
                name: "HHT_prodtomagtransfer",
                columns: table => new
                {
                    tid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    pcode = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false),
                    truckno = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    l1barcode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    lul = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    transferdt = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    month = table.Column<string>(type: "varchar(2)", maxLength: 2, nullable: false),
                    year = table.Column<string>(type: "varchar(4)", maxLength: 4, nullable: false),
                    td = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    brand = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    psize = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    mfgdt = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HHT_prodtomagtransfer", x => x.tid);
                });

            migrationBuilder.CreateTable(
                name: "IntimationMaster",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntimationMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "L1Barcodegeneration",
                columns: table => new
                {
                    L1Barcode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    SrNo = table.Column<long>(type: "bigint", nullable: false),
                    Country = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CountryCode = table.Column<string>(type: "varchar(2)", maxLength: 2, nullable: false),
                    MfgName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    MfgLoc = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    MfgCode = table.Column<string>(type: "varchar(3)", maxLength: 3, nullable: false),
                    PlantName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PCode = table.Column<string>(type: "varchar(2)", maxLength: 2, nullable: false),
                    MCode = table.Column<string>(type: "varchar(1)", maxLength: 1, nullable: false),
                    Shift = table.Column<string>(type: "varchar(1)", maxLength: 1, nullable: false),
                    BrandName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    BrandId = table.Column<string>(type: "varchar(4)", maxLength: 4, nullable: false),
                    ProductSize = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PSizeCode = table.Column<string>(type: "varchar(3)", maxLength: 3, nullable: false),
                    SdCat = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    UnNoClass = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    MfgDt = table.Column<DateTime>(type: "date", nullable: false),
                    MfgTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    L1NetWt = table.Column<decimal>(type: "numeric(12,4)", nullable: false),
                    L1NetUnit = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    NoOfL2 = table.Column<int>(type: "integer", nullable: false),
                    NoOfL3 = table.Column<int>(type: "integer", nullable: false),
                    MFlag = table.Column<bool>(type: "boolean", nullable: false),
                    CheckFlag = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_L1Barcodegeneration", x => x.L1Barcode);
                });

            migrationBuilder.CreateTable(
                name: "L1BarcodeReprint",
                columns: table => new
                {
                    l1barcode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    srno = table.Column<int>(type: "integer", nullable: false),
                    country = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    countrycode = table.Column<string>(type: "varchar(2)", maxLength: 2, nullable: false),
                    mfgname = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    mfgloc = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    mfgcode = table.Column<string>(type: "varchar(3)", maxLength: 3, nullable: false),
                    plantname = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    pcode = table.Column<string>(type: "varchar(2)", maxLength: 2, nullable: false),
                    mcode = table.Column<string>(type: "varchar(1)", maxLength: 1, nullable: false),
                    shift = table.Column<string>(type: "varchar(1)", maxLength: 1, nullable: false),
                    brandname = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    brandid = table.Column<string>(type: "varchar(4)", maxLength: 4, nullable: false),
                    productsize = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    psizecode = table.Column<string>(type: "varchar(3)", maxLength: 3, nullable: false),
                    sdcat = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    unnoclass = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    mfgdt = table.Column<DateTime>(type: "date", nullable: false),
                    rp_dt_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    month = table.Column<string>(type: "varchar(2)", maxLength: 2, nullable: false),
                    genyear = table.Column<string>(type: "varchar(4)", maxLength: 4, nullable: false),
                    reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_L1BarcodeReprint", x => x.l1barcode);
                });

            migrationBuilder.CreateTable(
                name: "L1boxDeletion",
                columns: table => new
                {
                    tid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    plant = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    brand = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    psize = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    mfgdt = table.Column<DateTime>(type: "date", nullable: false),
                    l1barcode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    deldt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    month = table.Column<string>(type: "varchar(2)", maxLength: 2, nullable: false),
                    year = table.Column<string>(type: "varchar(4)", maxLength: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_L1boxDeletion", x => x.tid);
                });

            migrationBuilder.CreateTable(
                name: "L1boxDeletionReport",
                columns: table => new
                {
                    from = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    to = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    mfgDt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    typeofreport = table.Column<string>(type: "text", nullable: false),
                    plant = table.Column<string>(type: "text", nullable: false),
                    brand = table.Column<string>(type: "text", nullable: false),
                    productsize = table.Column<string>(type: "text", nullable: false),
                    l1barcode = table.Column<string>(type: "text", nullable: false),
                    deletionDt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    reason = table.Column<string>(type: "text", nullable: false),
                    currentstkdt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Month = table.Column<string>(type: "text", nullable: false),
                    Year = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_L1boxDeletionReport", x => x.from);
                });

            migrationBuilder.CreateTable(
                name: "L1ReprintReport",
                columns: table => new
                {
                    from = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    to = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    typeofreport = table.Column<string>(type: "text", nullable: false),
                    plant = table.Column<string>(type: "text", nullable: false),
                    brandname = table.Column<string>(type: "text", nullable: false),
                    productsize = table.Column<string>(type: "text", nullable: false),
                    L1Barcode = table.Column<string>(type: "text", nullable: false),
                    reprintDt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    reason = table.Column<string>(type: "text", nullable: false),
                    time = table.Column<string>(type: "text", nullable: false),
                    currentstkdt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Month = table.Column<string>(type: "text", nullable: false),
                    Year = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_L1ReprintReport", x => x.from);
                });

            migrationBuilder.CreateTable(
                name: "L2ReprintReport",
                columns: table => new
                {
                    from = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    to = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    typeofreport = table.Column<string>(type: "text", nullable: false),
                    plant = table.Column<string>(type: "text", nullable: false),
                    plantcode = table.Column<string>(type: "text", nullable: false),
                    L2Barcode = table.Column<string>(type: "text", nullable: false),
                    reprintDt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    reason = table.Column<string>(type: "text", nullable: false),
                    time = table.Column<string>(type: "text", nullable: false),
                    currentstkdt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Month = table.Column<string>(type: "text", nullable: false),
                    Year = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_L2ReprintReport", x => x.from);
                });

            migrationBuilder.CreateTable(
                name: "MachineCodeMaster",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    pname = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    pcode = table.Column<string>(type: "varchar(2)", maxLength: 2, nullable: false),
                    mcode = table.Column<string>(type: "varchar(1)", maxLength: 1, nullable: false),
                    Company_ID = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MachineCodeMaster", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "MagzineMasters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    mfgloc = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    mfgloccode = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false),
                    magname = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    mcode = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false),
                    licno = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    issuedate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    validitydt = table.Column<DateTime>(type: "date", nullable: false),
                    Totalwt = table.Column<decimal>(type: "numeric(12,4)", nullable: false),
                    margin = table.Column<int>(type: "integer", nullable: false),
                    autoallot_flag = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MagzineMasters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MfgLocationMaster",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    mfgname = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    mfgcode = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false),
                    mfgloc = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    mfgloccode = table.Column<string>(type: "varchar(1)", maxLength: 1, nullable: false),
                    maincode = table.Column<string>(type: "varchar(3)", maxLength: 3, nullable: false),
                    Company_ID = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MfgLocationMaster", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "MfgMaster",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    mfgname = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Company_ID = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MfgMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlantMasters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    plant_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PCode = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false),
                    License = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Company_ID = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    issue_dt = table.Column<DateTime>(type: "date", nullable: false),
                    validity_dt = table.Column<DateTime>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlantMasters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlantTypeMaster",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    plant_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Company_ID = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlantTypeMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductionMagzineAllocations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TransferId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    TruckNo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Plant = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PlantCode = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true),
                    BrandName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    BrandId = table.Column<string>(type: "varchar(4)", maxLength: 4, nullable: true),
                    ProductSize = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ProductSizecCode = table.Column<string>(type: "varchar(3)", maxLength: 3, nullable: true),
                    MagazineName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CaseQuantity = table.Column<int>(type: "integer", nullable: false),
                    Totalwt = table.Column<decimal>(type: "numeric(12,4)", nullable: true),
                    ReadFlag = table.Column<int>(type: "integer", nullable: true),
                    TransferDate = table.Column<DateTime>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionMagzineAllocations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductionPlan",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductionPlanNo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ProductionType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    MfgDt = table.Column<DateTime>(type: "Date", nullable: false),
                    PlantCode = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    BrandId = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: false),
                    PSizeCode = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    TotalWeight = table.Column<decimal>(type: "numeric", nullable: false),
                    NoOfbox = table.Column<int>(type: "integer", nullable: false),
                    NoOfstickers = table.Column<int>(type: "integer", nullable: false),
                    MachineCode = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: false),
                    Shift = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: false),
                    CratedDate = table.Column<DateTime>(type: "Date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionPlan", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductionReport",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    from = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    to = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    shift = table.Column<string>(type: "text", nullable: false),
                    mfgdt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    plantname = table.Column<string>(type: "text", nullable: false),
                    brandname = table.Column<string>(type: "text", nullable: false),
                    productsize = table.Column<string>(type: "text", nullable: false),
                    re2 = table.Column<string>(type: "text", nullable: false),
                    typeofreport = table.Column<string>(type: "text", nullable: false),
                    l1barcode = table.Column<string>(type: "text", nullable: false),
                    l1netqty = table.Column<double>(type: "double precision", nullable: false),
                    l1netunit = table.Column<string>(type: "text", nullable: false),
                    boxcount = table.Column<string>(type: "text", nullable: false),
                    Qty = table.Column<string>(type: "text", nullable: false),
                    unit = table.Column<string>(type: "text", nullable: false),
                    currentdate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    time = table.Column<string>(type: "text", nullable: false),
                    month = table.Column<string>(type: "text", nullable: false),
                    year = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionReport", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ProductionTransfers",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    trans_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    months = table.Column<int>(type: "integer", nullable: true),
                    years = table.Column<int>(type: "integer", nullable: true),
                    truck_no = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Allotflag = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionTransfers", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ProductMaster",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    bname = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    bid = table.Column<string>(type: "varchar(4)", maxLength: 4, nullable: false),
                    ptype = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ptypecode = table.Column<string>(type: "varchar(2)", maxLength: 2, nullable: false),
                    Class = table.Column<int>(type: "integer", nullable: false),
                    Division = table.Column<int>(type: "integer", nullable: false),
                    unit = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    psize = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    psizecode = table.Column<string>(type: "varchar(3)", maxLength: 3, nullable: false),
                    dimnesion = table.Column<int>(type: "integer", nullable: false),
                    dimensionunit = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    dimunitwt = table.Column<decimal>(type: "numeric(12,4)", nullable: false),
                    wtunit = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    l1netwt = table.Column<decimal>(type: "numeric(12,4)", nullable: false),
                    noofl2 = table.Column<int>(type: "integer", nullable: false),
                    noofl3perl2 = table.Column<int>(type: "integer", nullable: false),
                    noofl3perl1 = table.Column<int>(type: "integer", nullable: false),
                    bpl1 = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    bpl2 = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    bpl3 = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    sdcat = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    unnoclass = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    act = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Re11IndentInfo",
                columns: table => new
                {
                    IndentNo = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    IndentDt = table.Column<DateTime>(type: "date", nullable: false),
                    PesoDt = table.Column<DateTime>(type: "date", nullable: false),
                    CustName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ConName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ConNo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Clic = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Month = table.Column<string>(type: "varchar(2)", maxLength: 2, nullable: false),
                    Year = table.Column<string>(type: "varchar(4)", maxLength: 4, nullable: false),
                    CompletedIndent = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Re11IndentInfo", x => x.IndentNo);
                });

            migrationBuilder.CreateTable(
                name: "RE11StatusReport",
                columns: table => new
                {
                    from = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    to = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    customer = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    re11indentno = table.Column<string>(type: "text", nullable: false),
                    indentDt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    pesore11Dt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Ptype = table.Column<string>(type: "text", nullable: false),
                    brand = table.Column<string>(type: "text", nullable: false),
                    productsize = table.Column<string>(type: "text", nullable: false),
                    reqqty = table.Column<string>(type: "text", nullable: false),
                    requnit = table.Column<string>(type: "text", nullable: false),
                    remqty = table.Column<string>(type: "text", nullable: false),
                    remunit = table.Column<string>(type: "text", nullable: false),
                    currentdt = table.Column<string>(type: "text", nullable: false),
                    month = table.Column<string>(type: "text", nullable: false),
                    year = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RE11StatusReport", x => x.from);
                });

            migrationBuilder.CreateTable(
                name: "RE2StatusReport",
                columns: table => new
                {
                    from = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    to = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    typeofreport = table.Column<string>(type: "text", nullable: false),
                    re2status = table.Column<int>(type: "integer", nullable: false),
                    brandname = table.Column<string>(type: "text", nullable: false),
                    productsize = table.Column<string>(type: "text", nullable: false),
                    magname = table.Column<string>(type: "text", nullable: false),
                    unloadDt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    l1barcode = table.Column<string>(type: "text", nullable: false),
                    currentdt = table.Column<string>(type: "text", nullable: false),
                    month = table.Column<string>(type: "text", nullable: false),
                    year = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RE2StatusReport", x => x.from);
                });

            migrationBuilder.CreateTable(
                name: "Re4Dispatch",
                columns: table => new
                {
                    dispdt = table.Column<string>(type: "text", nullable: true),
                    l1barcode = table.Column<string>(type: "text", nullable: true),
                    bname = table.Column<string>(type: "text", nullable: true),
                    brandid = table.Column<string>(type: "text", nullable: true),
                    productsize = table.Column<string>(type: "text", nullable: true),
                    psizecode = table.Column<string>(type: "text", nullable: true),
                    @class = table.Column<string>(name: "class", type: "text", nullable: true),
                    div = table.Column<string>(type: "text", nullable: true),
                    l1netwt = table.Column<string>(type: "text", nullable: true),
                    cname = table.Column<string>(type: "text", nullable: true),
                    clic = table.Column<string>(type: "text", nullable: true),
                    truckno = table.Column<string>(type: "text", nullable: true),
                    trucklic = table.Column<string>(type: "text", nullable: true),
                    magname = table.Column<string>(type: "text", nullable: true),
                    maglic = table.Column<string>(type: "text", nullable: true),
                    re12 = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "RE6Generation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    re12no = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    consigneename = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    consigneeid = table.Column<int>(type: "integer", nullable: false),
                    consigneeaddress = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    licno = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    transname = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    transid = table.Column<int>(type: "integer", nullable: false),
                    vehicleno = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    vehiclelic = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    vehiclevalue = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    re6date = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    re6time = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    re6month = table.Column<string>(type: "varchar(2)", maxLength: 2, nullable: false),
                    re6year = table.Column<string>(type: "varchar(4)", maxLength: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RE6Generation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Reprint_L2barcode",
                columns: table => new
                {
                    l2barcode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    srno = table.Column<int>(type: "integer", nullable: false),
                    plantcode = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false),
                    rptdt = table.Column<DateTime>(type: "date", nullable: false),
                    rpttime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    month = table.Column<string>(type: "varchar(2)", maxLength: 2, nullable: false),
                    year = table.Column<string>(type: "varchar(4)", maxLength: 4, nullable: false),
                    reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reprint_L2barcode", x => x.l2barcode);
                });

            migrationBuilder.CreateTable(
                name: "ResetTypeMaster",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    resettype = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    yeartype = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResetTypeMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoleMasters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleMasters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RouteMaster",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    cname = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    startpoint = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    destpoint = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Locations = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouteMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Shiftmanagement",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    pname = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    pcode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    shift = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: false),
                    activef = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shiftmanagement", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShiftMaster",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    shift = table.Column<string>(type: "text", nullable: false),
                    fromtime = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    totime = table.Column<TimeOnly>(type: "time without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShiftMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StateMaster",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    State = table.Column<string>(type: "text", nullable: false),
                    St_code = table.Column<string>(type: "text", nullable: false),
                    District = table.Column<string>(type: "text", nullable: false),
                    City = table.Column<string>(type: "text", nullable: false),
                    Tahsil = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StateMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StockSummary",
                columns: table => new
                {
                    mfgdt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    bname = table.Column<string>(type: "text", nullable: false),
                    bid = table.Column<string>(type: "text", nullable: false),
                    productsize = table.Column<string>(type: "text", nullable: false),
                    psizecode = table.Column<string>(type: "text", nullable: false),
                    @class = table.Column<string>(name: "class", type: "text", nullable: false),
                    div = table.Column<string>(type: "text", nullable: false),
                    magname = table.Column<string>(type: "text", nullable: false),
                    maglic = table.Column<string>(type: "text", nullable: false),
                    opening = table.Column<double>(type: "double precision", nullable: false),
                    recbname = table.Column<string>(type: "text", nullable: false),
                    recproductsize = table.Column<string>(type: "text", nullable: false),
                    quantity = table.Column<double>(type: "double precision", nullable: false),
                    batch = table.Column<string>(type: "text", nullable: false),
                    licence = table.Column<string>(type: "text", nullable: false),
                    transport = table.Column<string>(type: "text", nullable: false),
                    passno = table.Column<string>(type: "text", nullable: false),
                    production = table.Column<double>(type: "double precision", nullable: false),
                    closing = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "StorageMagazineReport",
                columns: table => new
                {
                    L1Barcode = table.Column<string>(type: "text", nullable: false),
                    license = table.Column<string>(type: "text", nullable: false),
                    magname = table.Column<string>(type: "text", nullable: false),
                    typeofreport = table.Column<string>(type: "text", nullable: false),
                    stockfrom = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    stockto = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    brandname = table.Column<string>(type: "text", nullable: false),
                    productsize = table.Column<string>(type: "text", nullable: false),
                    unit = table.Column<string>(type: "text", nullable: false),
                    netqty = table.Column<string>(type: "text", nullable: false),
                    currentstkdt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Month = table.Column<string>(type: "text", nullable: false),
                    Year = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StorageMagazineReport", x => x.L1Barcode);
                });

            migrationBuilder.CreateTable(
                name: "TransitTruckMaster",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Addr = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Gstno = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    State = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    District = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Tahsil = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransitTruckMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransportMaster",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Addr = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Gstno = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    State = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    District = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Tahsil = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransportMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TruckToMAGBarcode",
                columns: table => new
                {
                    Barcode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    TruckNo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    MagName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ScanFlag = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TruckToMAGBarcode", x => x.Barcode);
                });

            migrationBuilder.CreateTable(
                name: "UomMaster",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    uom = table.Column<string>(type: "text", nullable: false),
                    uomcode = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UomMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Company_ID = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Role = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Username);
                });

            migrationBuilder.CreateTable(
                name: "AllLoadingIndentDeatils",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LoadingSheetId = table.Column<int>(type: "integer", nullable: false),
                    IndentNo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    IndentDt = table.Column<DateTime>(type: "date", nullable: false),
                    Ptype = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PtypeCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Bname = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Bid = table.Column<string>(type: "varchar(4)", maxLength: 4, nullable: false),
                    Psize = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SizeCode = table.Column<string>(type: "varchar(3)", maxLength: 3, nullable: false),
                    Class = table.Column<int>(type: "integer", nullable: false),
                    Div = table.Column<int>(type: "integer", nullable: false),
                    L1NetWt = table.Column<decimal>(type: "numeric(12,4)", nullable: false),
                    Unit = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    LoadWt = table.Column<decimal>(type: "numeric(12,4)", nullable: true),
                    Loadcase = table.Column<int>(type: "integer", nullable: true),
                    TypeOfDispatch = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    mag = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    iscompleted = table.Column<int>(type: "integer", nullable: true),
                    Batch = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllLoadingIndentDeatils", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AllLoadingIndentDeatils_AllLoadingSheet_LoadingSheetId",
                        column: x => x.LoadingSheetId,
                        principalTable: "AllLoadingSheet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustMagazineDetail",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Cid = table.Column<int>(type: "integer", nullable: false),
                    Magazine = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    License = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Validity = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Wt = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustMagazineDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustMagazineDetail_CustomerMaster_Cid",
                        column: x => x.Cid,
                        principalTable: "CustomerMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustMemberDetail",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Cid = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ContactNo = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustMemberDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustMemberDetail_CustomerMaster_Cid",
                        column: x => x.Cid,
                        principalTable: "CustomerMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "L2Barcodegeneration",
                columns: table => new
                {
                    L2Barcode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    SrNo = table.Column<long>(type: "bigint", nullable: false),
                    L1Barcode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    MfgDt = table.Column<DateTime>(type: "date", nullable: false),
                    MfgTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_L2Barcodegeneration", x => x.L2Barcode);
                    table.ForeignKey(
                        name: "FK_L2Barcodegeneration_L1Barcodegeneration_L1Barcode",
                        column: x => x.L1Barcode,
                        principalTable: "L1Barcodegeneration",
                        principalColumn: "L1Barcode");
                });

            migrationBuilder.CreateTable(
                name: "Magzine_Stock",
                columns: table => new
                {
                    L1Barcode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    MagName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    BrandName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    BrandId = table.Column<string>(type: "varchar(4)", maxLength: 4, nullable: false),
                    PrdSize = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PSizeCode = table.Column<string>(type: "varchar(3)", maxLength: 3, nullable: false),
                    Stock = table.Column<int>(type: "integer", nullable: false),
                    StkDt = table.Column<DateTime>(type: "date", nullable: false),
                    Re2 = table.Column<bool>(type: "boolean", nullable: false),
                    Re12 = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Magzine_Stock", x => x.L1Barcode);
                    table.ForeignKey(
                        name: "FK_Magzine_Stock_L1Barcodegeneration_L1Barcode",
                        column: x => x.L1Barcode,
                        principalTable: "L1Barcodegeneration",
                        principalColumn: "L1Barcode");
                });

            migrationBuilder.CreateTable(
                name: "MagzineMasterDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    magzineid = table.Column<int>(type: "integer", nullable: false),
                    Class = table.Column<int>(type: "integer", nullable: false),
                    division = table.Column<int>(type: "integer", nullable: false),
                    Product = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    wt = table.Column<decimal>(type: "numeric(12,4)", nullable: false),
                    margin = table.Column<int>(type: "integer", nullable: false),
                    units = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    free_space = table.Column<decimal>(type: "numeric(12,4)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MagzineMasterDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MagzineMasterDetails_MagzineMasters_magzineid",
                        column: x => x.magzineid,
                        principalTable: "MagzineMasters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductionMagzineAllocationCases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TransferToMazgnieId = table.Column<int>(type: "integer", nullable: false),
                    L1Scanned = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionMagzineAllocationCases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductionMagzineAllocationCases_ProductionMagzineAllocatio~",
                        column: x => x.TransferToMazgnieId,
                        principalTable: "ProductionMagzineAllocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductionTransferCases",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    production_transfer_id = table.Column<int>(type: "integer", nullable: false),
                    l1barcode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionTransferCases", x => x.id);
                    table.ForeignKey(
                        name: "FK_ProductionTransferCases_ProductionTransfers_production_tran~",
                        column: x => x.production_transfer_id,
                        principalTable: "ProductionTransfers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DispatchTransaction",
                columns: table => new
                {
                    Tid = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IndentNo = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    L1Barcode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Brand = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Bid = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false),
                    PSize = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PSizeCode = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false),
                    TruckNo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    MagName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DispDt = table.Column<DateTime>(type: "date", nullable: false),
                    Re12 = table.Column<bool>(type: "boolean", nullable: false),
                    L1NetWt = table.Column<decimal>(type: "numeric(12,4)", nullable: false),
                    L1NetUnit = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    L1barcodegenerationL1Barcode = table.Column<string>(type: "varchar(50)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DispatchTransaction", x => x.Tid);
                    table.ForeignKey(
                        name: "FK_DispatchTransaction_L1Barcodegeneration_L1barcodegeneration~",
                        column: x => x.L1barcodegenerationL1Barcode,
                        principalTable: "L1Barcodegeneration",
                        principalColumn: "L1Barcode");
                    table.ForeignKey(
                        name: "FK_DispatchTransaction_Re11IndentInfo_IndentNo",
                        column: x => x.IndentNo,
                        principalTable: "Re11IndentInfo",
                        principalColumn: "IndentNo");
                });

            migrationBuilder.CreateTable(
                name: "Re11IndentPrdInfo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IndentNo = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    IndentDt = table.Column<DateTime>(type: "date", nullable: false),
                    Ptype = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PtypeCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Bname = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Bid = table.Column<string>(type: "varchar(4)", maxLength: 4, nullable: false),
                    Psize = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SizeCode = table.Column<string>(type: "varchar(3)", maxLength: 3, nullable: false),
                    Class = table.Column<int>(type: "integer", nullable: false),
                    Div = table.Column<int>(type: "integer", nullable: false),
                    L1NetWt = table.Column<decimal>(type: "numeric(12,4)", nullable: false),
                    Unit = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ReqWt = table.Column<decimal>(type: "numeric(12,4)", nullable: false),
                    ReqCase = table.Column<int>(type: "integer", nullable: false),
                    RemWt = table.Column<decimal>(type: "numeric(12,4)", nullable: true),
                    Remcase = table.Column<int>(type: "integer", nullable: true),
                    LoadWt = table.Column<decimal>(type: "numeric(12,4)", nullable: true),
                    Loadcase = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Re11IndentPrdInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Re11IndentPrdInfo_Re11IndentInfo_IndentNo",
                        column: x => x.IndentNo,
                        principalTable: "Re11IndentInfo",
                        principalColumn: "IndentNo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PageAccess",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<int>(type: "integer", nullable: false),
                    PageName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IsAdd = table.Column<bool>(type: "boolean", nullable: false),
                    IsEdit = table.Column<bool>(type: "boolean", nullable: false),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageAccess", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PageAccess_RoleMasters_RoleId",
                        column: x => x.RoleId,
                        principalTable: "RoleMasters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TransitMemberDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Cid = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ContactNo = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    TransitTruckMasterId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransitMemberDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransitMemberDetails_TransitTruckMaster_TransitTruckMasterId",
                        column: x => x.TransitTruckMasterId,
                        principalTable: "TransitTruckMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TransitTruckDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Cid = table.Column<int>(type: "integer", nullable: false),
                    VehicleNo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    License = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Validity = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Wt = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TransitTruckMasterId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransitTruckDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransitTruckDetails_TransitTruckMaster_TransitTruckMasterId",
                        column: x => x.TransitTruckMasterId,
                        principalTable: "TransitTruckMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TransMemberDetail",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Cid = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ContactNo = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransMemberDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransMemberDetail_TransportMaster_Cid",
                        column: x => x.Cid,
                        principalTable: "TransportMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TransVehicleDetail",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Cid = table.Column<int>(type: "integer", nullable: false),
                    VehicleNo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    License = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Validity = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Wt = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransVehicleDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransVehicleDetail_TransportMaster_Cid",
                        column: x => x.Cid,
                        principalTable: "TransportMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "L3Barcodegeneration",
                columns: table => new
                {
                    L3Barcode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    SrNo = table.Column<long>(type: "bigint", nullable: false),
                    L1Barcode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    L2Barcode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    MfgDt = table.Column<DateTime>(type: "date", nullable: false),
                    MfgTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_L3Barcodegeneration", x => x.L3Barcode);
                    table.ForeignKey(
                        name: "FK_L3Barcodegeneration_L1Barcodegeneration_L1Barcode",
                        column: x => x.L1Barcode,
                        principalTable: "L1Barcodegeneration",
                        principalColumn: "L1Barcode");
                    table.ForeignKey(
                        name: "FK_L3Barcodegeneration_L2Barcodegeneration_L2Barcode",
                        column: x => x.L2Barcode,
                        principalTable: "L2Barcodegeneration",
                        principalColumn: "L2Barcode");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AllLoadingIndentDeatils_LoadingSheetId",
                table: "AllLoadingIndentDeatils",
                column: "LoadingSheetId");

            migrationBuilder.CreateIndex(
                name: "IX_BarcodeData_Batch_L1",
                table: "BarcodeData",
                columns: new[] { "Batch", "L1" });

            migrationBuilder.CreateIndex(
                name: "IX_BarcodeData_L1",
                table: "BarcodeData",
                column: "L1");

            migrationBuilder.CreateIndex(
                name: "IX_BarcodeDataFinal_L1",
                table: "BarcodeDataFinal",
                column: "L1");

            migrationBuilder.CreateIndex(
                name: "IX_PCode_MCode_BrandId_PSizeCode_batch_MfgDt_L1",
                table: "BarcodeDataFinal",
                columns: new[] { "PlantCode", "BrandId", "SizeCode", "Batch", "MfgDt", "L1" });

            migrationBuilder.CreateIndex(
                name: "IX_BatchCode_PlantCode_MfgDate",
                table: "BatchDetails",
                columns: new[] { "BatchCode", "PlantCode", "MfgDate" });

            migrationBuilder.CreateIndex(
                name: "IX_BatchCode_PlantCode_BatchType",
                table: "BatchMasters",
                columns: new[] { "BatchCode", "PlantCode", "BatchType" });

            migrationBuilder.CreateIndex(
                name: "IX_CustMagazineDetail_Cid",
                table: "CustMagazineDetail",
                column: "Cid");

            migrationBuilder.CreateIndex(
                name: "IX_CustMemberDetail_Cid",
                table: "CustMemberDetail",
                column: "Cid");

            migrationBuilder.CreateIndex(
                name: "IX_DispatchTransaction_DispDt",
                table: "DispatchTransaction",
                column: "DispDt");

            migrationBuilder.CreateIndex(
                name: "IX_DispatchTransaction_IndentNo",
                table: "DispatchTransaction",
                column: "IndentNo");

            migrationBuilder.CreateIndex(
                name: "IX_DispatchTransaction_IndentNo_Re12",
                table: "DispatchTransaction",
                columns: new[] { "IndentNo", "Re12" });

            migrationBuilder.CreateIndex(
                name: "IX_DispatchTransaction_L1barcodegenerationL1Barcode",
                table: "DispatchTransaction",
                column: "L1barcodegenerationL1Barcode");

            migrationBuilder.CreateIndex(
                name: "IX_DispatchTransaction_L1BidPSizeCode",
                table: "DispatchTransaction",
                columns: new[] { "L1Barcode", "Bid", "PSizeCode" });

            migrationBuilder.CreateIndex(
                name: "IX_DispatchTransaction_MagNamere12",
                table: "DispatchTransaction",
                columns: new[] { "MagName", "Re12" });

            migrationBuilder.CreateIndex(
                name: "IX_L1Barcodegeneration_MfgDt",
                table: "L1Barcodegeneration",
                column: "MfgDt");

            migrationBuilder.CreateIndex(
                name: "IX_L1Barcodegeneration_Production",
                table: "L1Barcodegeneration",
                columns: new[] { "MfgDt", "PCode", "BrandId", "PSizeCode" });

            migrationBuilder.CreateIndex(
                name: "IX_L2Barcodegeneration_L1Barcode",
                table: "L2Barcodegeneration",
                column: "L1Barcode");

            migrationBuilder.CreateIndex(
                name: "IX_L3Barcodegeneration_L1Barcode",
                table: "L3Barcodegeneration",
                column: "L1Barcode");

            migrationBuilder.CreateIndex(
                name: "IX_L3Barcodegeneration_L2Barcode",
                table: "L3Barcodegeneration",
                column: "L2Barcode");

            migrationBuilder.CreateIndex(
                name: "IX_BrandId_PSizeCode_L1Barcode",
                table: "Magzine_Stock",
                columns: new[] { "BrandId", "PSizeCode", "L1Barcode" });

            migrationBuilder.CreateIndex(
                name: "IX_MagazineStock_L1Barcode",
                table: "Magzine_Stock",
                column: "L1Barcode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MagazineStock_MagName_Re2_Re12",
                table: "Magzine_Stock",
                columns: new[] { "MagName", "Re2", "Re12" });

            migrationBuilder.CreateIndex(
                name: "IX_MagzineMasterDetails_magzineid",
                table: "MagzineMasterDetails",
                column: "magzineid");

            migrationBuilder.CreateIndex(
                name: "IX_PageAccess_RoleId",
                table: "PageAccess",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionMagzineAllocationCases_TransferToMazgnieId",
                table: "ProductionMagzineAllocationCases",
                column: "TransferToMazgnieId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionPlan_MfgDt_PlantCode_BrandId_PSizeCode",
                table: "ProductionPlan",
                columns: new[] { "MfgDt", "PlantCode", "BrandId", "PSizeCode" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductionTransferCases_l1barcode",
                table: "ProductionTransferCases",
                column: "l1barcode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductionTransferCases_production_transfer_id",
                table: "ProductionTransferCases",
                column: "production_transfer_id");

            migrationBuilder.CreateIndex(
                name: "IX_Re11IndentInfo_IndentDt",
                table: "Re11IndentInfo",
                column: "IndentDt");

            migrationBuilder.CreateIndex(
                name: "IX_Re11IndentInfo_IndentNo",
                table: "Re11IndentInfo",
                column: "IndentNo");

            migrationBuilder.CreateIndex(
                name: "IX_Re11IndentPrdInfo_IndentNo",
                table: "Re11IndentPrdInfo",
                column: "IndentNo");

            migrationBuilder.CreateIndex(
                name: "IX_RoleMaster_Page",
                table: "RoleMasters",
                column: "RoleName");

            migrationBuilder.CreateIndex(
                name: "IX_TransitMemberDetails_TransitTruckMasterId",
                table: "TransitMemberDetails",
                column: "TransitTruckMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_TransitTruckDetails_TransitTruckMasterId",
                table: "TransitTruckDetails",
                column: "TransitTruckMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_TransMemberDetail_Cid",
                table: "TransMemberDetail",
                column: "Cid");

            migrationBuilder.CreateIndex(
                name: "IX_TransVehicleDetail_Cid",
                table: "TransVehicleDetail",
                column: "Cid");

            migrationBuilder.CreateIndex(
                name: "IX_User_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AllLoadingIndentDeatils");

            migrationBuilder.DropTable(
                name: "BarcodeData");

            migrationBuilder.DropTable(
                name: "BarcodeDataFinal");

            migrationBuilder.DropTable(
                name: "BatchDetails");

            migrationBuilder.DropTable(
                name: "BatchMasters");

            migrationBuilder.DropTable(
                name: "BrandMaster");

            migrationBuilder.DropTable(
                name: "Canre11indent_prd_info");

            migrationBuilder.DropTable(
                name: "Canre11indentinfo");

            migrationBuilder.DropTable(
                name: "CountryMaster");

            migrationBuilder.DropTable(
                name: "CustMagazineDetail");

            migrationBuilder.DropTable(
                name: "CustMemberDetail");

            migrationBuilder.DropTable(
                name: "DeviceMaster");

            migrationBuilder.DropTable(
                name: "DispatchDownload");

            migrationBuilder.DropTable(
                name: "DispatchReport");

            migrationBuilder.DropTable(
                name: "DispatchTransaction");

            migrationBuilder.DropTable(
                name: "FormRe2MagAllot");

            migrationBuilder.DropTable(
                name: "HHT_prodtomagtransfer");

            migrationBuilder.DropTable(
                name: "IntimationMaster");

            migrationBuilder.DropTable(
                name: "L1BarcodeReprint");

            migrationBuilder.DropTable(
                name: "L1boxDeletion");

            migrationBuilder.DropTable(
                name: "L1boxDeletionReport");

            migrationBuilder.DropTable(
                name: "L1ReprintReport");

            migrationBuilder.DropTable(
                name: "L2ReprintReport");

            migrationBuilder.DropTable(
                name: "L3Barcodegeneration");

            migrationBuilder.DropTable(
                name: "MachineCodeMaster");

            migrationBuilder.DropTable(
                name: "Magzine_Stock");

            migrationBuilder.DropTable(
                name: "MagzineMasterDetails");

            migrationBuilder.DropTable(
                name: "MfgLocationMaster");

            migrationBuilder.DropTable(
                name: "MfgMaster");

            migrationBuilder.DropTable(
                name: "PageAccess");

            migrationBuilder.DropTable(
                name: "PlantMasters");

            migrationBuilder.DropTable(
                name: "PlantTypeMaster");

            migrationBuilder.DropTable(
                name: "ProductionMagzineAllocationCases");

            migrationBuilder.DropTable(
                name: "ProductionPlan");

            migrationBuilder.DropTable(
                name: "ProductionReport");

            migrationBuilder.DropTable(
                name: "ProductionTransferCases");

            migrationBuilder.DropTable(
                name: "ProductMaster");

            migrationBuilder.DropTable(
                name: "Re11IndentPrdInfo");

            migrationBuilder.DropTable(
                name: "RE11StatusReport");

            migrationBuilder.DropTable(
                name: "RE2StatusReport");

            migrationBuilder.DropTable(
                name: "Re4Dispatch");

            migrationBuilder.DropTable(
                name: "RE6Generation");

            migrationBuilder.DropTable(
                name: "Reprint_L2barcode");

            migrationBuilder.DropTable(
                name: "ResetTypeMaster");

            migrationBuilder.DropTable(
                name: "RouteMaster");

            migrationBuilder.DropTable(
                name: "Shiftmanagement");

            migrationBuilder.DropTable(
                name: "ShiftMaster");

            migrationBuilder.DropTable(
                name: "StateMaster");

            migrationBuilder.DropTable(
                name: "StockSummary");

            migrationBuilder.DropTable(
                name: "StorageMagazineReport");

            migrationBuilder.DropTable(
                name: "TransitMemberDetails");

            migrationBuilder.DropTable(
                name: "TransitTruckDetails");

            migrationBuilder.DropTable(
                name: "TransMemberDetail");

            migrationBuilder.DropTable(
                name: "TransVehicleDetail");

            migrationBuilder.DropTable(
                name: "TruckToMAGBarcode");

            migrationBuilder.DropTable(
                name: "UomMaster");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "AllLoadingSheet");

            migrationBuilder.DropTable(
                name: "CustomerMaster");

            migrationBuilder.DropTable(
                name: "L2Barcodegeneration");

            migrationBuilder.DropTable(
                name: "MagzineMasters");

            migrationBuilder.DropTable(
                name: "RoleMasters");

            migrationBuilder.DropTable(
                name: "ProductionMagzineAllocations");

            migrationBuilder.DropTable(
                name: "ProductionTransfers");

            migrationBuilder.DropTable(
                name: "Re11IndentInfo");

            migrationBuilder.DropTable(
                name: "TransitTruckMaster");

            migrationBuilder.DropTable(
                name: "TransportMaster");

            migrationBuilder.DropTable(
                name: "L1Barcodegeneration");
        }
    }
}
