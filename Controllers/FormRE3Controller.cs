using Peso_Baseed_Barcode_Printing_System_API.Interface;
using AutoMapper;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office2013.Drawing.ChartStyle;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.Extensions.Caching.Distributed;
using NetTopologySuite.Algorithm;
using Npgsql;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Models.DTO;
using Peso_Baseed_Barcode_Printing_System_API.Repositorys;
using System.Data;
using System.Globalization;
using System.Net;
using static System.Runtime.InteropServices.JavaScript.JSType;
using DataTable = System.Data.DataTable;



namespace Peso_Baseed_Barcode_Printing_System_API.Controllers
{

    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class FormRE3Controller : ControllerBase
    {

        private readonly Iformre2magallotRepository _formre2magallotRepository;
        private readonly IL1barcodegenerationRepository _L1barcodegenerationRepository;
        private readonly IPlantMasterRepository _plantMasterRepository;
        private readonly IMagzineMasterRepository _magzineMasterRepository;
        private readonly IDistributedCache _distributedCache;
        private APIResponse _APIResponse;
        private readonly IMapper _mapper;
        private string KeyName = "formre2magallot";
        private readonly IConfiguration _configuration;


        public FormRE3Controller(Iformre2magallotRepository formre2magallotRepository, IDistributedCache distributedCache, IMapper mapper, IL1barcodegenerationRepository L1barcodegenerationRepository, IMagzineMasterRepository magzineMasterRepository, IPlantMasterRepository plantMasterRepository, IConfiguration configuration)
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

        }


        [HttpGet("{fromDate}/{toDate}/{magname}")]
        public async Task<ActionResult<APIResponse>> FormRE3(string fromDate, string toDate, string magname)
        {
            fromDate = fromDate.Replace("-", "/");
            toDate = toDate.Replace("-", "/");

            var response = new APIResponse();
            var result = new Dictionary<string, object>();
            var stockSoldSummaryList = new List<StockSummary>();

            string strcon = _configuration["ConnectionStrings:DefaultConnection"];

            try
            {
                await using var con = new NpgsqlConnection(strcon);
                await con.OpenAsync();

                DateTime fromDateTime = DateTime.Parse(fromDate);
                DateTime toDateTime = DateTime.Parse(toDate);


                try
                {
                    string deleteQuery = @"DELETE FROM ""StockSummary""";
                    await using var deleteCmd = new NpgsqlCommand(deleteQuery, con);
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

                // Load data into strongly-typed lists
                var dt1 = await SelectStockSummaryDateRangeAsync(con, fromDateTime, toDateTime, magname);
                result["stockSummary"] = dt1;

                foreach (var item in dt1)
                {
                    var dt2 = await SelectFromPostgresByDateRangeAsync(con, fromDateTime, toDateTime, item.bname, item.productsize, item.maglic);
                    string sroseries = "-";
                    string plic = "-";

                    foreach (var row2 in dt2)
                    {
                        try
                        {
                            string gendt = row2.gendt.ToString();
                            if (row2.bname == item.bname && row2.productsize == item.productsize && gendt == item.mfgdt.ToString())
                            {
                                string str = row2.srno_ranges.TrimEnd(',');
                                if (str.EndsWith(" to "))
                                {
                                    var parts = str.Split(',');
                                    string firstValue = parts.Length > 0 ? parts[0] : "";
                                    string lastValue = parts.Length > 0 ? parts[^1] : "";
                                    if (parts.Length > 1)
                                    {
                                        parts = parts.Skip(1).Take(parts.Length - 2).ToArray();
                                    }
                                    row2.srno_ranges = lastValue + firstValue + " , " + string.Join(" , ", parts);
                                }
                                if (sroseries == "-")
                                {
                                    sroseries = row2.srno_ranges;
                                }
                                else
                                {
                                    sroseries += " , " + row2.srno_ranges;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            response.Errors.Add($"Error processing srno_ranges: {ex.Message}");
                            response.Status = false;
                            response.StatusCode = HttpStatusCode.InternalServerError;
                            return StatusCode(StatusCodes.Status500InternalServerError, response);
                        }
                    }

                    try
                    {
                        string Query = $@"SELECT plm.""License"" FROM ""ProductMaster"" pm, ""PlantMasters"" plm 
                                  WHERE pm.ptypecode = plm.""PCode"" AND pm.bname = '{item.bname}' AND pm.psize = '{item.productsize}'";

                        await using var cmd = new NpgsqlCommand(Query, con);
                        cmd.CommandTimeout = 0;
                        await using var reader = await cmd.ExecuteReaderAsync();

                        while (await reader.ReadAsync())
                        {
                            string license = reader.GetString(0);
                            if (plic == "-")
                            {
                                plic = license;
                            }
                            else
                            {
                                plic += " , " + license;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        response.Errors.Add($"License fetch error: {ex.Message}");
                        response.Status = false;
                        response.StatusCode = HttpStatusCode.InternalServerError;
                        return StatusCode(StatusCodes.Status500InternalServerError, response);
                    }

                    var tempdt4 = await StockSoldSummaryAsync(con, item.mfgdt, item.bid, item.psizecode, magname, item.maglic);
                    double stock = 0;
                    var row1 = new StockSummary
                    {
                        mfgdt = Convert.ToDateTime(item.mfgdt),
                        bname = item.bname,
                        productsize = item.productsize,
                        @class = item.@class,
                        div = item.div
                    };

                    if (tempdt4.Count > 0)
                    {
                        foreach (var row3 in tempdt4)
                        {
                            int qty = 0;
                            try
                            {
                                await using var conn = new NpgsqlConnection(strcon);
                                await conn.OpenAsync();
                                string q = @"SELECT COUNT(*) FROM ""DispatchTransaction"" 
                                     WHERE ""IndentNo"" = @indentno AND ""Brand"" = @brand AND ""PSize"" = @psize";
                                await using var cmd = new NpgsqlCommand(q, conn);
                                cmd.Parameters.AddWithValue("@indentno", row3.re12);
                                cmd.Parameters.AddWithValue("@brand", item.bname);
                                cmd.Parameters.AddWithValue("@psize", item.productsize);
                                qty = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                            }
                            catch { /* handle if needed */ }

                            if (stock == 0)
                            {
                                row1.opening = item.opening;
                                stock = item.opening + item.production;
                            }

                            if (stock > 0)
                            {
                                if (item.production == 0)
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
                                    row1.recbname = item.bname;
                                    row1.recproductsize = item.productsize;
                                    row1.quantity = item.production;
                                    row1.batch = sroseries;
                                    row1.licence = plic;
                                    row1.transport = "Eicher / Explosives Van";
                                }
                                row1.passno = "";

                                double soldSum = row3.netsold;
                                row1.closing = stock - soldSum;
                                stock -= soldSum;
                            }
                        }
                    }
                    else
                    {
                        stock = item.opening + item.production;
                        if (item.opening == item.closing) stock = item.opening;

                        if (stock > 0 || (stock == 0 && item.production > 0))
                        {
                            row1.opening = stock != 0 ? item.opening : 0;
                            row1.passno = "-";

                            if (item.production == 0)
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
                                row1.recbname = item.bname;
                                row1.recproductsize = item.productsize;
                                row1.quantity = item.production;
                                row1.batch = sroseries;
                                row1.licence = plic;
                                row1.transport = "Eicher / Explosives Van";
                            }

                            row1.closing = stock;
                        }
                    }

                    if (row1.closing != 0 || row1.quantity != 0)
                    {
                        stockSoldSummaryList.Add(row1);
                    }
                }

                result["StockSoldSummary"] = stockSoldSummaryList;

                var mlicTable = new DataTable();
                mlicTable.Columns.Add("mlic_for_head", typeof(string));
                var mlicRow = mlicTable.NewRow();
                string mlic_for_head = "";

                try
                {
                    string query = @"select licno from ""MagzineMasters"" WHERE mcode ILIKE @magname";
                    await using var cmd = new NpgsqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@magname", $"%{magname}%");
                    await using var reader = await cmd.ExecuteReaderAsync();
                    if (await reader.ReadAsync())
                    {
                        mlic_for_head = reader["licno"].ToString();
                    }
                }
                catch (Exception ex)
                {
                    response.Errors.Add($"General Error: {ex.Message}");
                    response.Status = false;
                    response.StatusCode = HttpStatusCode.InternalServerError;
                    return StatusCode(StatusCodes.Status500InternalServerError, response);
                }

                mlicRow["mlic_for_head"] = mlic_for_head;
                mlicTable.Rows.Add(mlicRow);
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
                string insertQuery = @"INSERT INTO public.""StockSummary"" ( mfgdt, bname, bid, productsize, psizecode, ""class"", div, magname, maglic, opening, production, closing)SELECT dates.date, fr2ma.bname, fr2ma.bid, fr2ma.productsize, fr2ma.psize, fr2ma.""class"", fr2ma.div, fr2ma.magname, fr2ma.maglic, COALESCE(SUM(fr2ma.l1netwt) FILTER (WHERE fr2ma.mfgdt < dates.date), 0) AS opening, COALESCE(SUM(fr2ma.l1netwt) FILTER (WHERE fr2ma.mfgdt = dates.date), 0) AS production, COALESCE(SUM(fr2ma.l1netwt) FILTER (WHERE fr2ma.mfgdt <= dates.date), 0) AS closing FROM generate_series( @StartDate::date, @EndDate::date, interval '1 day' ) AS dates(date)CROSS JOIN public.""FormRe2MagAllot"" fr2ma WHERE fr2ma.mfgdt <= dates.date GROUP BY dates.date, fr2ma.bname, fr2ma.bid, fr2ma.productsize, fr2ma.psize, fr2ma.""class"", fr2ma.div, fr2ma.magname, fr2ma.maglic;";

                using var insertCmd = new NpgsqlCommand(insertQuery, con);
                insertCmd.Parameters.AddWithValue("@StartDate", NpgsqlTypes.NpgsqlDbType.Date, fromDate.Date); // DateTime or DateOnly
                insertCmd.Parameters.AddWithValue("@EndDate", NpgsqlTypes.NpgsqlDbType.Date, toDate.Date);
                insertCmd.CommandTimeout = 0;
                await insertCmd.ExecuteNonQueryAsync();



                // string UpdateQuery = @"UPDATE public.""StockSummary"" SET opening = COALESCE(""StockSummary"".opening, 0) - COALESCE(subquery.opening, 0), closing = (COALESCE(""StockSummary"".opening, 0) - COALESCE(subquery.opening, 0)) + COALESCE(""StockSummary"".production, 0) - COALESCE(subquery.closing, 0) FROM (SELECT dates.date, bname, brandid AS bid, productsize, psizecode, class, div, magname, maglic, COALESCE(SUM(CAST(l1netwt AS double precision)) FILTER (WHERE dispdt < dates.date), 0) AS opening, COALESCE(SUM(CAST(l1netwt AS double precision)) FILTER (WHERE dispdt <= dates.date), 0) AS closing FROM generate_series((@StartDate), (@EndDate), interval '1 day') dates(date) CROSS JOIN public.re4dispatch WHERE dispdt IS NOT NULL GROUP BY dates.date, bname, brandid, productsize, psizecode, class, div, magname, maglic) AS subquery WHERE stocksummary.mfgdt = subquery.date AND stocksummary.bname = subquery.bname AND stocksummary.bid = subquery.bid AND stocksummary.productsize = subquery.productsize AND stocksummary.psizecode = subquery.psizecode AND stocksummary.class = subquery.class AND stocksummary.div = subquery.div AND stocksummary.magname = subquery.magname AND stocksummary.maglic = subquery.maglic";


                string UpdateQuery = @"UPDATE public.""StockSummary"" SET opening = COALESCE(""StockSummary"".opening, 0) - COALESCE(subquery.opening, 0), closing = ( COALESCE(""StockSummary"".opening, 0) - COALESCE(subquery.opening, 0) ) + COALESCE(""StockSummary"".production, 0) - COALESCE(subquery.closing, 0) FROM ( SELECT         dates.date, bname, brandid AS bid, productsize, psizecode, class, div, magname, maglic, COALESCE(SUM(CAST(l1netwt AS double precision)) FILTER (WHERE dispdt::date < dates.date), 0) AS opening, COALESCE(SUM(CAST(l1netwt AS double precision)) FILTER (WHERE dispdt::date <= dates.date), 0) AS closing FROM generate_series( @StartDate , @EndDate, INTERVAL '1 day' ) AS dates(date) CROSS JOIN public.""Re4Dispatch"" WHERE dispdt IS NOT NULL GROUP BY dates.date, bname,        brandid, productsize, psizecode, class, div, magname, maglic) AS subquery WHERE ""StockSummary"".mfgdt = subquery.date AND ""StockSummary"".bname = subquery.bname AND ""StockSummary"".bid = subquery.bid    AND ""StockSummary"".productsize = subquery.productsize    AND ""StockSummary"".psizecode = subquery.psizecode AND ""StockSummary"".""class"" = subquery.class AND ""StockSummary"".div = subquery.div AND ""StockSummary"".magname = subquery.magname AND ""StockSummary"".maglic = subquery.maglic;";



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

        private async Task<List<StockSummaryDateRangeItem>> SelectStockSummaryDateRangeAsync(NpgsqlConnection con, DateTime fromDate, DateTime toDate, string magname)
        {
            var items = new List<StockSummaryDateRangeItem>();
            try
            {
                string query = @"SELECT  mfgdt, bname, bid, productsize, psizecode, class, div, magname, maglic, opening, production, closing FROM ""StockSummary"" WHERE magname = @magname AND mfgdt BETWEEN @fromDate AND @toDate";

                await using var cmd = new NpgsqlCommand(query, con);
                cmd.Parameters.AddWithValue("magname", magname);
                cmd.Parameters.AddWithValue("fromDate", fromDate);
                cmd.Parameters.AddWithValue("toDate", toDate);

                await using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    items.Add(new StockSummaryDateRangeItem
                    {
                        mfgdt = reader.GetDateTime(reader.GetOrdinal("mfgdt")),
                        bname = reader.GetString(reader.GetOrdinal("bname")),
                        bid = reader.IsDBNull(reader.GetOrdinal("bid")) ? "" : reader.GetString(reader.GetOrdinal("bid")),
                        productsize = reader.GetString(reader.GetOrdinal("productsize")),
                        psizecode = reader.GetString(reader.GetOrdinal("psizecode")),
                        @class = reader.GetString(reader.GetOrdinal("class")),
                        div = reader.GetString(reader.GetOrdinal("div")),
                        magname = reader.GetString(reader.GetOrdinal("magname")),
                        maglic = reader.GetString(reader.GetOrdinal("maglic")),
                        opening = reader.GetDouble(reader.GetOrdinal("opening")),
                        production = reader.GetDouble(reader.GetOrdinal("production")),
                        closing = reader.GetDouble(reader.GetOrdinal("closing"))

                    });
                }

            }
            catch (Exception ex)
            {
                // Ideally log this
                throw new Exception($"SelectStockSummaryDateRangeAsync failed: {ex.Message}", ex);
            }

            return items;
        }

        private async Task<List<SelectFromPostgresItem>> SelectFromPostgresByDateRangeAsync(NpgsqlConnection con, DateTime fromDate, DateTime toDate, string bname, string productsize, string maglic)
        {
            var list = new List<SelectFromPostgresItem>();

            try
            {
                string query = @"WITH srno_ranges AS (
                        SELECT pcode, shift, bname, productsize, maglic,
                               MIN(srno) AS first_srno, MAX(srno) AS last_srno
                        FROM (
                            SELECT srno, shift, pcode, bname, productsize, maglic,
                                   LAG(srno) OVER (PARTITION BY magname, shift ORDER BY srno) AS prev_srno,
                                   LEAD(srno) OVER (PARTITION BY magname, shift ORDER BY srno) AS next_srno
                            FROM ""FormRe2MagAllot""
                            WHERE mfgdt BETWEEN @fromDate AND @toDate
                              AND bname = @bname
                              AND productsize = @productsize
                              AND maglic = @maglic
                        ) AS cte
                        GROUP BY pcode, shift, bname, productsize, maglic
                     ),
                     aggregated_data AS (
                         SELECT f.gendt, f.shift, f.""class"", f.div, f.bname, f.productsize, f.maglic,
                                f.pcode, SUM(f.l1netwt) AS netwt, f.dateoftest, COUNT(f.srno) AS srno_count
                         FROM ""FormRe2MagAllot"" f
                         WHERE f.gendt BETWEEN @fromDate AND @toDate
                           AND f.bname = @bname
                           AND f.productsize = @productsize
                           AND f.maglic = @maglic
                         GROUP BY f.gendt, f.shift, f.""class"", f.div, f.bname, f.productsize, f.maglic, f.pcode, f.dateoftest
                     )
                     SELECT TO_CHAR(ad.gendt, 'DD/MM/YYYY') as gendt, ad.shift, ad.""class"", ad.div, ad.bname,
                            ad.productsize, ad.maglic, ad.pcode, ad.netwt, ad.dateoftest, ad.srno_count,
                            STRING_AGG(
                                CASE
                                    WHEN sr.first_srno IS NOT NULL AND sr.last_srno IS NOT NULL THEN CONCAT(sr.first_srno::TEXT, ' to ', sr.last_srno::TEXT)
                                    WHEN sr.first_srno IS NOT NULL THEN sr.first_srno::TEXT
                                    WHEN sr.last_srno IS NOT NULL THEN sr.last_srno::TEXT
                                    ELSE ''
                                END, ', '
                            ) AS srno_ranges
                     FROM aggregated_data ad
                     LEFT JOIN srno_ranges sr ON ad.pcode = sr.pcode AND ad.shift = sr.shift AND ad.bname = sr.bname
                                             AND ad.productsize = sr.productsize AND ad.maglic = sr.maglic
                     GROUP BY ad.gendt, ad.shift, ad.""class"", ad.div, ad.bname, ad.productsize, ad.maglic,
                              ad.pcode, ad.netwt, ad.dateoftest, ad.srno_count
                     ORDER BY ad.gendt, ad.shift, ad.dateoftest";

                using var cmd = new NpgsqlCommand(query, con);
                cmd.Parameters.AddWithValue("@fromDate", fromDate);
                cmd.Parameters.AddWithValue("@toDate", toDate);
                cmd.Parameters.AddWithValue("@bname", bname);
                cmd.Parameters.AddWithValue("@productsize", productsize);
                cmd.Parameters.AddWithValue("@maglic", maglic);

                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        Console.WriteLine($"{reader.GetName(i)}: {reader.GetFieldType(i).Name}");
                    }


                    list.Add(new SelectFromPostgresItem
                    {
                        gendt = reader.GetString(reader.GetOrdinal("gendt")),
                        shift = reader.IsDBNull(reader.GetOrdinal("shift")) ? "" : reader.GetString(reader.GetOrdinal("shift")),
                        @class = reader.IsDBNull(reader.GetOrdinal("class")) ? 0 : reader.GetInt32(reader.GetOrdinal("class")),
                        div = reader.IsDBNull(reader.GetOrdinal("div")) ? 0 : reader.GetInt32(reader.GetOrdinal("div")),
                        bname = reader.IsDBNull(reader.GetOrdinal("bname")) ? "" : reader.GetString(reader.GetOrdinal("bname")),
                        productsize = reader.IsDBNull(reader.GetOrdinal("productsize")) ? "" : reader.GetString(reader.GetOrdinal("productsize")),
                        maglic = reader.IsDBNull(reader.GetOrdinal("maglic")) ? "" : reader.GetString(reader.GetOrdinal("maglic")),
                        pcode = reader.IsDBNull(reader.GetOrdinal("pcode")) ? "" : reader.GetString(reader.GetOrdinal("pcode")),
                        netwt = reader.IsDBNull(reader.GetOrdinal("netwt")) ? 0.0 : reader.GetDouble(reader.GetOrdinal("netwt")),
                        dateoftest = reader.IsDBNull(reader.GetOrdinal("dateoftest")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("dateoftest")),
                        srno_count = reader.IsDBNull(reader.GetOrdinal("srno_count")) ? 0L : reader.GetInt64(reader.GetOrdinal("srno_count")),
                        srno_ranges = reader.IsDBNull(reader.GetOrdinal("srno_ranges")) ? "" : reader.GetString(reader.GetOrdinal("srno_ranges"))
                    });


                }

            }
            catch (Exception ex)
            {
                // Ideally log this
                throw new Exception($"SelectFromPostgresByDateRangeAsync failed: {ex.Message}", ex);
            }


            return list;
        }

        private async Task<List<StockSoldSummaryItem>> StockSoldSummaryAsync(NpgsqlConnection con, DateTime mfgdt, string bid, string psizecode, string magname, string maglic)
        {
            var list = new List<StockSoldSummaryItem>();

            try
            {


                string query = @"SELECT SUM(l1netwt::double precision) AS netsold, cname, clic, truckno, trucklic, re12
                     FROM ""Re4Dispatch""
                     WHERE dispdt = @mfgdt
                       AND brandid = @bid
                       AND psizecode = @psizecode
                       AND magname = @magname
                     GROUP BY cname, clic, truckno, trucklic, re12";

                using var cmd = new NpgsqlCommand(query, con);
                //cmd.Parameters.AddWithValue("@mfgdt", mfgdt);
                cmd.Parameters.AddWithValue("@mfgdt", NpgsqlTypes.NpgsqlDbType.Date, mfgdt.Date);
                cmd.Parameters.AddWithValue("@bid", bid);
                cmd.Parameters.AddWithValue("@psizecode", psizecode);
                cmd.Parameters.AddWithValue("@magname", magname);
                cmd.Parameters.AddWithValue("@maglic", maglic);

                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    list.Add(new StockSoldSummaryItem
                    {
                        netsold = reader.GetDouble(reader.GetOrdinal("netsold")),
                        cname = reader.IsDBNull(reader.GetOrdinal("cname")) ? "" : reader.GetString(reader.GetOrdinal("cname")),
                        clic = reader.IsDBNull(reader.GetOrdinal("clic")) ? "" : reader.GetString(reader.GetOrdinal("clic")),
                        truckno = reader.IsDBNull(reader.GetOrdinal("truckno")) ? "" : reader.GetString(reader.GetOrdinal("truckno")),
                        trucklic = reader.IsDBNull(reader.GetOrdinal("trucklic")) ? "" : reader.GetString(reader.GetOrdinal("trucklic")),
                        re12 = reader.IsDBNull(reader.GetOrdinal("re12")) ? "" : reader.GetString(reader.GetOrdinal("re12"))
                    });
                }
            }
            catch (Exception ex)
            {
                // Ideally log this
                throw new Exception($"StockSoldSummaryAsync failed: {ex.Message}", ex);
            }



            return list;
        }
    }

    public class StockSummaryDateRangeItem
    {
        public DateTime mfgdt { get; set; }
        public string bname { get; set; } = "";
        public string bid { get; set; } = "";
        public string productsize { get; set; } = "";
        public string psizecode { get; set; } = "";
        public string @class { get; set; } = "";
        public string div { get; set; } = "";
        public string magname { get; set; } = "";
        public string maglic { get; set; } = "";
        public double opening { get; set; }
        public double production { get; set; }
        public double closing { get; set; }

    }

    public class SelectFromPostgresItem
    {
        public string gendt { get; set; } = "";
        public string shift { get; set; } = "";
        public int @class { get; set; }
        public int div { get; set; }
        public string bname { get; set; } = "";
        public string productsize { get; set; } = "";
        public string maglic { get; set; } = "";
        public string pcode { get; set; } = "";
        public double netwt { get; set; }
        public DateTime dateoftest { get; set; }
        public long srno_count { get; set; }
        public string srno_ranges { get; set; } = "";
    }


    public class StockSoldSummaryItem
    {
        public double netsold { get; set; }
        public string cname { get; set; } = "";
        public string clic { get; set; } = "";
        public string truckno { get; set; } = "";
        public string trucklic { get; set; } = "";
        public string re12 { get; set; } = "";
    }

}
