using Peso_Baseed_Barcode_Printing_System_API.Interface;
using AutoMapper;
using Azure;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office2013.Drawing.ChartStyle;
using DocumentFormat.OpenXml.VariantTypes;
using DocumentFormat.OpenXml.Wordprocessing;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.Extensions.Caching.Distributed;
using NetTopologySuite.Algorithm;
using Npgsql;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Models.DTO;
using Peso_Baseed_Barcode_Printing_System_API.Repositorys;
using System;
using System.Data;
using System.Net;
using static System.Runtime.InteropServices.JavaScript.JSType;
using DataTable = System.Data.DataTable;



namespace Peso_Baseed_Barcode_Printing_System_API.Controllers
{

    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class FormRE4Controller : ControllerBase
    {

        private readonly Iformre2magallotRepository _formre2magallotRepository;
        private readonly IL1barcodegenerationRepository _L1barcodegenerationRepository;
        private readonly IPlantMasterRepository _plantMasterRepository;
        private readonly IMagzineMasterRepository _magzineMasterRepository;
        private readonly IBrandMasterRepository _brandMasterRepository;
        private readonly IDistributedCache _distributedCache;
        private APIResponse _APIResponse;
        private readonly IMapper _mapper;
        private string KeyName = "formre2magallot";
        private readonly IConfiguration _configuration;


        public FormRE4Controller(Iformre2magallotRepository formre2magallotRepository, IDistributedCache distributedCache, IMapper mapper, IL1barcodegenerationRepository L1barcodegenerationRepository, IMagzineMasterRepository magzineMasterRepository, IPlantMasterRepository plantMasterRepository, IConfiguration configuration, IBrandMasterRepository brandMasterRepository)
        {
            _formre2magallotRepository = formre2magallotRepository;
            _APIResponse = new APIResponse();
            _distributedCache = distributedCache;
            _mapper = mapper;
            _L1barcodegenerationRepository = L1barcodegenerationRepository;
            _plantMasterRepository = plantMasterRepository;
            _mapper = mapper;
            _magzineMasterRepository = magzineMasterRepository;
            _configuration = configuration;
            _brandMasterRepository = brandMasterRepository;
        }

        [HttpGet("{brandName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetIdByBrandName(string brandName)
        {
            try
            {
                var brandId = await _brandMasterRepository.GetIdByBrandNameAsync(brandName);
                if (brandId == null)
                {
                    _APIResponse.Status = false;
                    _APIResponse.StatusCode = HttpStatusCode.NotFound;
                    _APIResponse.Errors.Add("Brand not found.");
                    return NotFound(_APIResponse);
                }

                _APIResponse.Data = brandId;
                _APIResponse.Status = true;
                _APIResponse.StatusCode = HttpStatusCode.OK;
                return Ok(_APIResponse);
            }
            catch (Exception ex)
            {
                _APIResponse.Errors.Add(ex.Message);
                _APIResponse.Status = false;
                _APIResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode((int)HttpStatusCode.InternalServerError, _APIResponse);
            }
        }


        [HttpGet("{indentno}/{despDate}/{brand}/{brandid}/{magname}/{truckno}/{re12}")]
        public async Task<ActionResult<APIResponse>> GetRe4TableDate(string indentno, string despDate, string brand, string brandid, string magname, string truckno, string re12)
        {
            indentno = indentno.Replace(".", "/");
            despDate = despDate.Replace("-", "/");

            var response = new APIResponse();
            var result = new Dictionary<string, DataTable>();

            var dt = new DataTable();
            dt.Columns.Add("dispdt", typeof(string));
            dt.Columns.Add("l1barcode", typeof(string));
            dt.Columns.Add("brandname", typeof(string));
            dt.Columns.Add("brandid", typeof(string));
            dt.Columns.Add("productsize", typeof(string));
            dt.Columns.Add("psizecode", typeof(string));
            dt.Columns.Add("l1netwt", typeof(string));
            dt.Columns.Add("custname", typeof(string));
            dt.Columns.Add("clic", typeof(string));
            dt.Columns.Add("truckno", typeof(string));

            try
            {
                string strcon = _configuration["ConnectionStrings:DefaultConnection"];
                await using var con = new NpgsqlConnection(strcon);
                await con.OpenAsync();

                DateTime fromDateTime = DateTime.Parse(despDate);

                string query = $@"SELECT DISTINCT dp.""DispDt"", dp.""L1Barcode"", pi.""BrandName"", pi.""BrandId"", pi.""ProductSize"", pi.""PSizeCode"", pi.""L1NetWt"", ri.""CustName"", ri.""Clic"", dp.""TruckNo"" FROM ""DispatchTransaction"" dp JOIN ""L1Barcodegeneration"" pi ON dp.""L1Barcode"" = pi.""L1Barcode"" JOIN ""Re11IndentInfo"" ri ON dp.""IndentNo"" = ri.""IndentNo"" WHERE dp.""IndentNo"" = @indentno AND dp.""DispDt"" = @fromDate AND dp.""Brand"" = @brand AND pi.""BrandId"" = @brandid AND dp.""MagName"" = @magname AND dp.""TruckNo"" = @truckno AND NOT EXISTS ( SELECT 1 FROM re4dispatch rt WHERE rt.l1barcode = dp.""L1Barcode"" )";

                await using var cmd = new NpgsqlCommand(query, con);
                cmd.Parameters.AddWithValue("@fromDate", fromDateTime);
                cmd.Parameters.AddWithValue("@indentno", indentno);
                cmd.Parameters.AddWithValue("@brand", brand);
                cmd.Parameters.AddWithValue("@brandid", brandid);
                cmd.Parameters.AddWithValue("@magname", magname);
                cmd.Parameters.AddWithValue("@truckno", truckno);

                await using var reader = await cmd.ExecuteReaderAsync();
                dt.Load(reader);
                await reader.CloseAsync();

                result["DispatchData"] = dt;
                response.Data = result;
                response.Status = true;
                response.StatusCode = HttpStatusCode.OK;

                // If re12 is not provided, return bad request
                if (string.IsNullOrWhiteSpace(re12))
                {
                    return BadRequest(new APIResponse
                    {
                        Status = false,
                        StatusCode = HttpStatusCode.BadRequest,
                        Errors = new List<string> { "Please enter RE-12!" }
                    });
                }

                foreach (DataRow item in dt.Rows)
                {
                    string class1 = "", div = "";

                    string productQuery = @"SELECT ""Class"", ""Division"" FROM ""ProductMaster"" WHERE bid = @bid AND psizecode = @psizecode";

                    await using var cmdProduct = new NpgsqlCommand(productQuery, con);
                    cmdProduct.Parameters.AddWithValue("@bid", item["brandid"].ToString());
                    cmdProduct.Parameters.AddWithValue("@psizecode", item["psizecode"].ToString());

                    await using var productReader = await cmdProduct.ExecuteReaderAsync();
                    if (await productReader.ReadAsync())
                    {
                        class1 = productReader["Class"].ToString();
                        div = productReader["Division"].ToString();
                    }
                    await productReader.CloseAsync();

                    string insertQuery = @"INSERT INTO re4dispatch (dispdt, l1barcode, bname, brandid, productsize, psizecode, class, div, l1netwt, cname, clic, truckno, trucklic, magname, maglic, re12) VALUES (@dispdt, @l1barcode, @bname, @brandid, @productsize, @psizecode, @class, @division, @l1netwt, @cname, @clic, @truckno, @trucklic, @magname, @maglic, @re12)";

                    await using var insertCmd = new NpgsqlCommand(insertQuery, con);
                    insertCmd.Parameters.AddWithValue("@dispdt", fromDateTime);
                    insertCmd.Parameters.AddWithValue("@l1barcode", item["l1barcode"].ToString());
                    insertCmd.Parameters.AddWithValue("@bname", item["brandname"].ToString());
                    insertCmd.Parameters.AddWithValue("@brandid", item["brandid"].ToString());
                    insertCmd.Parameters.AddWithValue("@productsize", item["productsize"].ToString());
                    insertCmd.Parameters.AddWithValue("@psizecode", item["psizecode"].ToString());
                    insertCmd.Parameters.AddWithValue("@class", class1);
                    insertCmd.Parameters.AddWithValue("@division", div);
                    insertCmd.Parameters.AddWithValue("@l1netwt", item["l1netwt"].ToString());
                    insertCmd.Parameters.AddWithValue("@cname", item["custname"].ToString());
                    insertCmd.Parameters.AddWithValue("@clic", item["clic"].ToString());
                    insertCmd.Parameters.AddWithValue("@truckno", item["truckno"].ToString());
                    insertCmd.Parameters.AddWithValue("@trucklic", string.Empty); // Replace if needed
                    insertCmd.Parameters.AddWithValue("@magname", magname);
                    insertCmd.Parameters.AddWithValue("@maglic", string.Empty); // Replace if needed
                    insertCmd.Parameters.AddWithValue("@re12", re12);

                    await insertCmd.ExecuteNonQueryAsync();
                }

                return Ok(new APIResponse
                {
                    Data = "Data successfully added in RE-4 Dispatch!",
                    Status = true,
                    StatusCode = HttpStatusCode.Created
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new APIResponse
                {
                    Status = false,
                    StatusCode = HttpStatusCode.InternalServerError,
                    Errors = new List<string> { ex.Message }
                });
            }
        }



        //[HttpPost()]
        //[ProducesResponseType(StatusCodes.Status201Created)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //public async Task<ActionResult<APIResponse>> SaveRE4Data(FormRE4 request)
        //{
        //    try
        //    {
        //        if (string.IsNullOrWhiteSpace(request.re12))
        //        {
        //            return BadRequest(new APIResponse
        //            {
        //                Status = false,
        //                StatusCode = HttpStatusCode.BadRequest,
        //                Errors = new List<string> { "Please enter RE-12!" }
        //            });
        //        }

        //        if (request.re4dispatch == null || !request.re4dispatch.Any())
        //        {
        //            return BadRequest(new APIResponse
        //            {
        //                Status = false,
        //                StatusCode = HttpStatusCode.BadRequest,
        //                Errors = new List<string> { "No data found to dispatch." }
        //            });
        //        }
        //        string strcon = _configuration["ConnectionStrings:DefaultConnection"];

        //        using var con = new NpgsqlConnection(strcon);
        //        await con.OpenAsync();

        //        foreach (var item in request.re4dispatch)
        //        {
        //            string class1 = "", div = "";

        //            string query = @"SELECT ""Class"", ""Division"" FROM ""ProductMaster"" WHERE bid = '" + item.brandid + "' AND psizecode = '" + item.psizecode + "'";

        //            using var cmd = new NpgsqlCommand(query, con);
        //            using var reader = await cmd.ExecuteReaderAsync();

        //            if (await reader.ReadAsync())
        //            {
        //                class1 = reader["class"].ToString();
        //                div = reader["division"].ToString();
        //            }

        //            reader.CloseAsync();

        //            // Insert into re4dispatch
        //            string query1 = @"INSERT INTO re4dispatch (dispdt, l1barcode, bname, brandid, productsize, psizecode, class, div, l1netwt, cname, clic, truckno, trucklic, magname, maglic, re12) VALUES (@dispdt, @l1barcode, @bname, @brandid, @productsize, @psizecode, @class, @division, @l1netwt, @cname, @clic, @truckno, @trucklic, @magname, @maglic, @re12)";


        //            DateTime dispDate = DateTime.Parse(request.despdate);


        //            using var cmd1 = new NpgsqlCommand(query1, con);
        //            cmd1.Parameters.AddWithValue("@dispdt", dispDate);
        //            cmd1.Parameters.AddWithValue("@l1barcode", item.l1barcode ?? string.Empty);
        //            cmd1.Parameters.AddWithValue("@bname", item.bname ?? string.Empty);
        //            cmd1.Parameters.AddWithValue("@brandid", item.brandid ?? string.Empty);
        //            cmd1.Parameters.AddWithValue("@productsize", item.productsize ?? string.Empty);
        //            cmd1.Parameters.AddWithValue("@psizecode", item.psizecode ?? string.Empty);
        //            cmd1.Parameters.AddWithValue("@class", class1);
        //            cmd1.Parameters.AddWithValue("@division", div);
        //            cmd1.Parameters.AddWithValue("@l1netwt", item.l1netwt ?? string.Empty);
        //            cmd1.Parameters.AddWithValue("@cname", item.cname ?? string.Empty);
        //            cmd1.Parameters.AddWithValue("@clic", item.clic ?? string.Empty);
        //            cmd1.Parameters.AddWithValue("@truckno", request.truckNo ?? string.Empty);
        //            cmd1.Parameters.AddWithValue("@trucklic", request.trucklic ?? string.Empty);
        //            cmd1.Parameters.AddWithValue("@magname", request.magname ?? string.Empty);
        //            cmd1.Parameters.AddWithValue("@maglic", request.maglic ?? string.Empty);
        //            cmd1.Parameters.AddWithValue("@re12", request.re12 ?? string.Empty);

        //            using var reader1 = await cmd1.ExecuteReaderAsync();

        //        }

        //        return Ok(new APIResponse
        //        {
        //            Data = "Data successfully added in RE-4 Dispatch!",
        //            Status = true,
        //            StatusCode = HttpStatusCode.Created
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new APIResponse
        //        {
        //            Status = false,
        //            StatusCode = HttpStatusCode.InternalServerError,
        //            Errors = new List<string> { ex.Message }
        //        });
        //    }
        //}



        [HttpGet("{fromDate}/{toDate}/{magname}")]
        public async Task<ActionResult<APIResponse>> FormRE4(string fromDate, string toDate, string magname)
        {
            fromDate = fromDate.Replace(".", "/");
            toDate = toDate.Replace(".", "/");

            var response = new APIResponse();
            var result = new Dictionary<string, DataTable>();

            var dt3 = new DataTable();

            // Define the columns (must match the model properties and order)
            dt3.Columns.Add("mfgdt", typeof(string));
            dt3.Columns.Add("bname", typeof(string));
            dt3.Columns.Add("productsize", typeof(string));
            dt3.Columns.Add("class", typeof(string));
            dt3.Columns.Add("div", typeof(string));
            dt3.Columns.Add("opening", typeof(int));
            dt3.Columns.Add("recbname", typeof(string));
            dt3.Columns.Add("recproductsize", typeof(string));
            dt3.Columns.Add("quantity", typeof(int));
            dt3.Columns.Add("batch", typeof(string));
            dt3.Columns.Add("cname", typeof(string));
            dt3.Columns.Add("licence", typeof(string));
            dt3.Columns.Add("transport", typeof(string));
            dt3.Columns.Add("passno", typeof(string));
            dt3.Columns.Add("closing", typeof(double));
            dt3.Columns.Add("remarks", typeof(string));

            var stockSoldSummaryList = new List<StockSummary>();


            try
            {
                string strcon = _configuration["ConnectionStrings:DefaultConnection"];

                using var con = new NpgsqlConnection(strcon);
                await con.OpenAsync();

                DateTime fromDateTime = DateTime.Parse(fromDate);
                DateTime toDateTime = DateTime.Parse(toDate);

                try
                {
                    string deleteQuery = @"DELETE FROM ""StockSummary""";
                    using var deleteCmd = new NpgsqlCommand(deleteQuery, con);
                    await deleteCmd.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    response.Errors.Add($"Delete Error: {ex.Message}");
                    response.Status = false;
                    response.StatusCode = HttpStatusCode.InternalServerError;
                    return StatusCode(StatusCodes.Status500InternalServerError, response);
                }

                await GenStockSummaryAsync(con, fromDateTime, toDateTime);


                var dt1 = await SelectStockSummaryDateRangeAsync(con, fromDateTime, toDateTime, magname);
                result["stockSummary"] = dt1;

                foreach (DataRow row in dt1.Rows)
                {
                    string? mfgdt = row["mfgdt"]?.ToString();
                    string? bname = row["bname"]?.ToString();
                    string? bid = row["bid"]?.ToString() ?? ""; // default blank if not present
                    string? productsize = row["productsize"]?.ToString();
                    string? psize = row["psizecode"]?.ToString() ?? "";
                    string? class1 = row["class"]?.ToString();
                    string? div = row["div"]?.ToString();
                    string? magazinname = row["magname"]?.ToString();
                    string? maglic = row["maglic"]?.ToString();
                    string? opening = row["opening"]?.ToString();
                    string? production = row["production"]?.ToString();
                    double? closing = Convert.ToDouble(row["closing"]);
                    string? plic = "-";


                    var tempdt4 = await StockSoldSummaryAsync(con, Convert.ToDateTime(mfgdt), bid, psize, magname, maglic);

                    List<StockSummaryRowre4> stockSummary = new List<StockSummaryRowre4>();

                    double stock = 0;

                    if (tempdt4.Rows.Count > 0)
                    {
                        try
                        {
                            foreach (DataRow row3 in tempdt4.Rows)
                            {
                                var row1 = new StockSummaryRowre4
                                {
                                    mfgdt = mfgdt,
                                    bname = bname,
                                    productsize = productsize,
                                    @class = class1,
                                    div = div
                                };

                                if (stock == 0)
                                {
                                    row1.opening = Convert.ToInt32(opening);
                                    stock = Convert.ToDouble(opening) + Convert.ToDouble(production);
                                }
                                else
                                {
                                    row1.opening = Convert.ToInt32(stock);
                                }

                                row1.recbname = bname;
                                row1.recproductsize = productsize;
                                row1.quantity = Convert.ToDouble(row3["netsold"]);
                                row1.batch = "As Per Annexure";
                                row1.cname = row3["cname"].ToString();
                                row1.licence = row3["clic"].ToString();
                                row1.transport = "Eicher / Explosives Van";
                                row1.passno = row3["re12"].ToString();
                                row1.remarks = row3["truckno"].ToString();

                                double soldSum = Convert.ToDouble(row3["netsold"]);
                                row1.closing = stock - soldSum;
                                stock -= soldSum;

                                stockSummary.Add(row1);
                            }
                        }
                        catch (Exception ex)
                        {
                            response.Errors.Add($"Delete Error: {ex.Message}");
                            response.Status = false;
                            response.StatusCode = HttpStatusCode.InternalServerError;
                            return StatusCode(StatusCodes.Status500InternalServerError, response);
                        }


                    }
                    else
                    {
                        try
                        {
                            stock = Convert.ToDouble(opening) + Convert.ToDouble(production);

                            if (Convert.ToDouble(opening) == closing)
                                stock = Convert.ToDouble(opening);

                            if (stock > 0 || (stock == 0 && Convert.ToDouble(production) > 0))
                            {
                                var row1 = new StockSummaryRowre4
                                {
                                    mfgdt = mfgdt,
                                    bname = bname,
                                    productsize = productsize,
                                    @class = class1,
                                    div = div,
                                    opening = Convert.ToInt32(opening),
                                    closing = stock,
                                    passno = "-"
                                };

                                if (Convert.ToDouble(production) == 0)
                                {
                                    row1.recbname = "-";
                                    row1.recproductsize = "-";
                                    row1.quantity = 0;
                                    row1.batch = "-";
                                    row1.licence = "-";
                                    row1.transport = "-";
                                }
                                else
                                {
                                    row1.recbname = "-";
                                    row1.recproductsize = productsize;
                                    row1.quantity = 0;
                                    row1.batch = "As Per Annexure";
                                    row1.licence = plic;
                                    row1.transport = "Eicher / Explosives Van";
                                }

                                row1.remarks = "-";
                                stockSummary.Add(row1);
                            }
                        }
                        catch (Exception ex)
                        {
                            response.Errors.Add($"Delete Error: {ex.Message}");
                            response.Status = false;
                            response.StatusCode = HttpStatusCode.InternalServerError;
                            return StatusCode(StatusCodes.Status500InternalServerError, response);
                        }


                    }

                    // Final population to DataTable
                    foreach (var item in stockSummary)
                    {
                        try
                        {
                            dt3.Rows.Add(
                            item.mfgdt, item.bname, item.productsize, item.@class, item.div, item.opening, item.recbname, item.recproductsize, item.quantity, item.batch, item.cname, item.licence, item.transport, item.passno, Convert.ToDouble(item.closing), item.remarks.ToString());
                        }
                        catch (Exception ex)
                        {

                            string ed = ex.Message;
                        }

                    }


                    result["StockSoldSummary"] = dt3;
                    //List<StockSummaryRow> stockSoldSummaryList1 = stockSoldSummaryList;
                    //result["StockSoldSummary"] = stockSoldSummaryList1;


                    try
                    {
                        //string query = @"SELECT licno FROM ""MagzineMaster"" WHERE mcode ILIKE @magname";
                        string query = @"select plm.""License"" from ""ProductMaster"" pm,""PlantMasters"" plm where pm.ptypecode=plm.""PCode"" and pm.bname=@bname and pm.psize=@productsize";

                        using var cmd = new NpgsqlCommand(query, con);
                        //cmd.Parameters.AddWithValue("@magname", $" %{magname}% ");
                        cmd.Parameters.AddWithValue("@bname", bname);
                        cmd.Parameters.AddWithValue("@productsize", productsize);

                        using var reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            plic = reader["license"].ToString();
                        }
                    }
                    catch (Exception ex)
                    {
                        // Optional: log the exception
                    }
                }


                // Create and structure the mlic_for_head table
                var mlicTable = new DataTable();
                mlicTable.Columns.Add("mlic_for_head", typeof(string));
                var mlicRow = mlicTable.NewRow();

                string mlic_for_head = "";

                try
                {
                    string query = @"SELECT licno FROM ""MagzineMaster"" WHERE mcode ILIKE @magname";

                    using var cmd = new NpgsqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@magname", $"%{magname}%");

                    using var reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        mlic_for_head = reader["licno"].ToString();
                    }
                }
                catch (Exception ex)
                {
                    // Optional: log the exception
                }

                // Set the value into the DataTable
                mlicRow["mlic_for_head"] = mlic_for_head;
                mlicTable.Rows.Add(mlicRow);

                // Add to result dictionary
                result.Add("mlic_for_head", mlicTable);



                response.Data = result;
                response.Status = true;
                response.StatusCode = HttpStatusCode.OK;

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Errors.Add($"General Error: {ex.Message}");
                response.Status = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }

        }


        private async Task<DataTable> GenStockSummaryAsync(NpgsqlConnection con, DateTime fromDate, DateTime toDate)
        {
            try
            {
                string insertQuery = @"INSERT INTO public.""StockSummary"" ( mfgdt, bname, bid, productsize, psizecode, ""class"", div, magname, maglic, opening, production, closing)SELECT dates.date, fr2ma.bname, fr2ma.bid, fr2ma.productsize, fr2ma.psize, fr2ma.""class"", fr2ma.div, fr2ma.magname, fr2ma.maglic, COALESCE(SUM(fr2ma.l1netwt) FILTER (WHERE fr2ma.mfgdt < dates.date), 0) AS opening, COALESCE(SUM(fr2ma.l1netwt) FILTER (WHERE fr2ma.mfgdt = dates.date), 0) AS production, COALESCE(SUM(fr2ma.l1netwt) FILTER (WHERE fr2ma.mfgdt <= dates.date), 0) AS closing FROM generate_series( @StartDate::date, @EndDate::date, interval '1 day' ) AS dates(date)CROSS JOIN public.""FormRe2MagAllot"" fr2ma WHERE fr2ma.mfgdt <= dates.date GROUP BY dates.date, fr2ma.bname, fr2ma.bid, fr2ma.productsize, fr2ma.psize, fr2ma.""class"", fr2ma.div, fr2ma.magname, fr2ma.maglic";

                using var insertCmd = new NpgsqlCommand(insertQuery, con);
                insertCmd.Parameters.AddWithValue("@StartDate", fromDate); // DateTime value
                insertCmd.Parameters.AddWithValue("@EndDate", toDate);     // DateTime value
                insertCmd.CommandTimeout = 0;
                await insertCmd.ExecuteNonQueryAsync();


                string UpdateQuery = @"UPDATE public.""StockSummary"" SET opening = COALESCE(""StockSummary"".opening, 0) - COALESCE(subquery.opening, 0), closing = ( COALESCE(""StockSummary"".opening, 0) - COALESCE(subquery.opening, 0) ) + COALESCE(""StockSummary"".production, 0) - COALESCE(subquery.closing, 0) FROM ( SELECT dates.date, bname, brandid AS bid, productsize, psizecode, class, div, magname, maglic, COALESCE(SUM(CAST(l1netwt AS double precision)) FILTER (WHERE dispdt::date < dates.date), 0) AS opening, COALESCE(SUM(CAST(l1netwt AS double precision)) FILTER (WHERE dispdt::date <= dates.date), 0) AS closing FROM generate_series( @StartDate , @EndDate, INTERVAL '1 day' ) AS dates(date) CROSS JOIN public.""Re4Dispatch"" WHERE dispdt IS NOT NULL GROUP BY dates.date, bname, brandid, productsize, psizecode, class, div, magname, maglic) AS subquery WHERE ""StockSummary"".mfgdt = subquery.date AND ""StockSummary"".bname = subquery.bname AND ""StockSummary"".bid = subquery.bid    AND ""StockSummary"".productsize = subquery.productsize    AND ""StockSummary"".psizecode = subquery.psizecode AND ""StockSummary"".""class"" = subquery.class AND ""StockSummary"".div = subquery.div AND ""StockSummary"".magname = subquery.magname AND ""StockSummary"".maglic = subquery.maglic";

                using var updateCmd = new NpgsqlCommand(UpdateQuery, con);
                updateCmd.Parameters.AddWithValue("@StartDate", NpgsqlTypes.NpgsqlDbType.Date, fromDate.Date); // DateTime or DateOnly
                updateCmd.Parameters.AddWithValue("@EndDate", NpgsqlTypes.NpgsqlDbType.Date, toDate.Date);
                updateCmd.CommandTimeout = 0;
                await updateCmd.ExecuteNonQueryAsync();


                string UpdateQuery1 = @"UPDATE public.""StockSummary"" SET closing = CASE WHEN opening = 0 THEN closing ELSE opening + production END";

                using var updateCmd1 = new NpgsqlCommand(UpdateQuery1, con);
                updateCmd1.CommandTimeout = 0;
                await updateCmd1.ExecuteNonQueryAsync();


                return new DataTable(); // Nothing to return yet
            }
            catch (Exception ex)
            {
                // Ideally log this
                throw new Exception($"GenStockSummaryAsync failed: {ex.Message}", ex);
            }
        }

        private async Task<DataTable> SelectStockSummaryDateRangeAsync(NpgsqlConnection con, DateTime fromDate, DateTime toDate, string magname)
        {
            var dt = new DataTable();
            try
            {
                string query = @"SELECT * FROM ""StockSummary"" WHERE mfgdt BETWEEN @fromDate AND @toDate AND magname = @magname ORDER BY mfgdt";



                using var cmd = new NpgsqlCommand(query, con);
                //cmd.Parameters.AddWithValue("@fromDate", fromDate);
                //cmd.Parameters.AddWithValue("@toDate", toDate);
                //cmd.Parameters.AddWithValue("@magname", magname);
                cmd.Parameters.AddWithValue("@fromDate", NpgsqlTypes.NpgsqlDbType.Date, fromDate.Date);
                cmd.Parameters.AddWithValue("@toDate", NpgsqlTypes.NpgsqlDbType.Date, toDate.Date);
                cmd.Parameters.AddWithValue("@magname", magname);

                using var reader = await cmd.ExecuteReaderAsync();
                dt.Load(reader);
            }
            catch (Exception ex)
            {
                // Ideally log this
                throw new Exception($"SelectStockSummaryDateRangeAsync failed: {ex.Message}", ex);
            }



            //NpgsqlDataAdapter sda = new NpgsqlDataAdapter(query, con);
            //sda.Fill(dt);

            return dt;
        }

        private async Task<DataTable> StockSoldSummaryAsync(NpgsqlConnection con, DateTime mfgdt, string bid, string psizecode, string magname, string maglic)
        {
            var dt = new DataTable();

            var newDt = mfgdt.ToString("yyyy-MM-dd");

            try
            {
                string query = @"SELECT SUM(l1netwt::double precision) AS netsold, cname, clic, truckno, trucklic, re12 FROM ""Re4Dispatch"" WHERE dispdt = '" + newDt + "' AND brandid = '" + bid + "' AND psizecode = '" + psizecode + "' AND magname = '" + magname + "' GROUP BY cname, clic, truckno, trucklic, re12";

                using var cmd = new NpgsqlCommand(query, con);

                using var reader = await cmd.ExecuteReaderAsync();
                //var dt = new DataTable();
                dt.Load(reader);
            }
            catch (Exception ex)
            {
                // Ideally log this
                throw new Exception($"StockSoldSummaryAsync failed: {ex.Message}", ex);
            }


            return dt;
        }

    }

    public class StockSummaryRowre4
    {
        public string mfgdt { get; set; }
        public string bname { get; set; }
        public string productsize { get; set; }
        public string @class { get; set; }
        public string cname { get; set; }
        public string div { get; set; }
        public int opening { get; set; }
        public string recbname { get; set; }
        public string recproductsize { get; set; }
        public double quantity { get; set; }
        public string batch { get; set; }
        public string licence { get; set; }
        public string transport { get; set; }
        public string passno { get; set; }
        public string remarks { get; set; }
        public double closing { get; set; }
    }
}


