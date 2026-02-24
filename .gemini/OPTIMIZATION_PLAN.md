# Complete Project Optimization Plan

## Phase 1: Model Data Type Optimization (All Models)

### Core Models - Data Type & StringLength Updates

| Model | Changes |
|-------|---------|
| L1barcodegeneration | Add `[StringLength]` to all unbounded strings, add `[MaxLength(50)]` for L1Barcode PK, fix `double → decimal` for L1NetWt |
| L2barcodegeneration | Add `[StringLength]` to all unbounded strings, add `[MaxLength(50)]` for L2Barcode PK |
| L3barcodegeneration | Add `[StringLength]` to all unbounded strings, add `[MaxLength(50)]` for L3Barcode PK |
| DispatchTransaction | Already has StringLength - good |
| Magzine_Stock | Add `[MaxLength]` to MagName, BrandName, PrdSize |
| ProductMaster | Add `[MaxLength]` to all unbounded strings, fix `double → decimal` |
| BrandMaster | Add `[MaxLength]` to unit |
| PlantMaster | Add `[MaxLength]` to all unbounded strings |
| Re11IndentInfo | Already has StringLength mostly |
| Re11IndentPrdInfo | Add `[MaxLength]` to unbounded strings, fix `double → decimal` |
| AllLoadingSheet | Add `[MaxLength]` to strings |
| AllLoadingIndentDeatils | Fix `double → decimal` |
| CustomerMaster | Already has StringLength |
| TransportMaster | Already has StringLength |
| MagzineMaster | Add `[MaxLength]` to strings |
| BarcodeData/BarcodeDataFinal | Add proper PK (Id), add StringLength |
| BatchMasters | Mostly good |
| BatchDetails | Add StringLength |
| ProductionPlan | Already has StringLength |
| ProductionTransfer | Add StringLength |
| RE6Generation | Add StringLength, fix date types |
| hht_prodtomagtransfer | Add StringLength, fix date types |
| l1barcodereprint | Add StringLength |
| l1boxdeletion | Add StringLength |
| reprint_l2barcode | Already good |
| canre11indentinfo | Add StringLength |
| canre11indent_prd_info | Add StringLength |
| shiftmanagement | Add StringLength |
| DeviceMaster | Add StringLength |
| DispatchDownload | Add StringLength |
| TruckToMAGBarcode | Add StringLength |
| formre2magallot | Add StringLength |
| MagzineMasterDetails | Add StringLength |
| Report Models | Add StringLength to all |

## Phase 2: ApplicationDbContext Optimization
- Remove unnecessary usings
- Add proper string length configurations via Fluent API
- Clean up DbSet nullable types
- Configure proper column types

## Phase 3: Build & Migration
- Verify build passes
- Create migration

## Key Principles:
1. Add `[MaxLength(N)]` / `[StringLength(N)]` to ALL string properties to prevent unbounded `text` columns
2. Use `[Column(TypeName = "varchar(N)")]` for key columns to optimize index size
3. Keep backward compatibility - don't remove deprecated fields yet (Phase 2 migration columns already exist)
4. Fix `double` → `decimal` where precision matters (weights, quantities)
5. Clean up DbContext with proper Fluent API configurations
