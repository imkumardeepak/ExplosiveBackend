// ─── Master Entities ──────────────────────────────────────────────────────────

export interface CountryMaster {
  id: number;
  cname: string;
  code: string;
}

export interface StateMaster {
  id: number;
  state: string;
  st_code: string;
}

export interface MfgMaster {
  id: number;
  mfgname: string;
  code: string;
}

export interface MfgLocationMaster {
  id: number;
  locationName: string;
  locationCode: string;
  mfgId?: number;
}

export interface PlantMaster {
  id: number;
  pName: string;
  pCode: string;
  mfgLocationId?: number;
}

export interface PlantTypeMaster {
  id: number;
  typeName: string;
}

export interface BrandMaster {
  id: number;
  bname: string;
  bid: string;
  bclass: string;
  bdivision: string;
  bsdcat: string;
  bunnoclass: string;
  bunit: string;
  plantCode: string;
}

export interface ProductMaster {
  id: number;
  bid: string;
  psize: string;
  psizecod: string;
  l1netwt: number;
  l1netunit: string;
  noofl2: number;
  noofl3perl2: number;
  noofl3perl1: number;
  bpl1: boolean;
  bpl2: boolean;
  bpl3: boolean;
}

export interface MagazineMaster {
  id: number;
  magname: string;
  mcode: string;
  licno?: string;
  capacity?: number;
  plantCode?: string;
}

export interface CustomerMaster {
  id: number;
  custName: string;
  custCode?: string;
  stateId?: number;
  address?: string;
  licNo?: string;
  members?: CustMemberDetail[];
  magazines?: CustMagazineDetail[];
}

export interface CustMemberDetail {
  id: number;
  custId: number;
  memberName: string;
  designation?: string;
}

export interface CustMagazineDetail {
  id: number;
  custId: number;
  magazineName: string;
  licNo?: string;
}

export interface TransportMaster {
  id: number;
  tName: string;
  tCode?: string;
  vehicles?: TransVehicleDetail[];
  members?: TransMemberDetail[];
}

export interface TransVehicleDetail {
  id: number;
  transportId: number;
  truckNo: string;
  vehicleType?: string;
  licNo?: string;
}

export interface TransMemberDetail {
  id: number;
  transportId: number;
  memberName: string;
  designation?: string;
  licNo?: string;
}

export interface ShiftMaster {
  id: number;
  shiftName: string;
  shiftCode: string;
}

export interface MachineCodeMaster {
  id: number;
  machineName: string;
  machineCode: string;
  plantCode?: string;
}

export interface BatchMaster {
  id: number;
  batchName: string;
  batchCode: string;
  plantCode?: string;
}

export interface RouteMaster {
  id: number;
  routeName: string;
  routeCode?: string;
}

export interface ResetTypeMaster {
  id: number;
  resetType: string;
}

export interface IntimationMaster {
  id: number;
  intimationName: string;
  intimationNo?: string;
}

export interface DeviceMaster {
  id: number;
  deviceName: string;
  deviceId: string;
  deviceType?: string;
}

export interface UserMaster {
  id: number;
  username: string;
  email?: string;
  fullname?: string;
  plantCode?: string;
  company_ID?: string;
  roleId?: number;
  role?: { id: number; roleName: string };
  isActive?: boolean;
}

export interface RoleMaster {
  id: number;
  roleName: string;
  pageAccesses?: { pageName: string; canView: boolean; canCreate: boolean; canEdit: boolean; canDelete: boolean }[];
}

// ─── Barcode Entities ─────────────────────────────────────────────────────────

export interface L1Barcode {
  l1Barcode: string;
  srNo: number;
  country: string;
  countryCode: string;
  mfgName: string;
  mfgLoc: string;
  mfgCode: string;
  plantName: string;
  pCode: string;
  mCode: string;
  shift: string;
  brandName: string;
  brandId: string;
  productSize: string;
  pSizeCode: string;
  sdCat: string;
  unNoClass: string;
  mfgDt: string;
  mfgTime?: string;
  l1NetWt: number;
  l1NetUnit: string;
  noOfL2: number;
  noOfL3: number;
  mFlag?: number;
  checkFlag?: boolean;
  l2Barcodes?: L2Barcode[];
}

export interface L2Barcode {
  l2Barcode: string;
  l1Barcode: string;
  srNo: number;
  mfgDt: string;
  l3Barcodes?: L3Barcode[];
}

export interface L3Barcode {
  l3Barcode: string;
  l1Barcode: string;
  l2Barcode: string;
  srNo: number;
  mfgDt: string;
}

export interface L1GenerateRequest {
  country: string;
  countryCode: string;
  mfgName: string;
  mfgLoc: string;
  mfgCode: string;
  plantName: string;
  pCode: string;
  mCode: string;
  shift: string;
  brandName: string;
  brandId: string;
  productSize: string;
  pSizeCode: string;
  class: string;
  division: string;
  sdCat: string;
  unNoClass: string;
  mfgDt: string;
  l1NetWt: number;
  l1NetUnit: string;
  noOfL2: number;
  noOfL3perL2: number;
  noOfL3: number;
  noOfbox: number;
  noOfstickers: number;
}

// ─── Magazine & Storage ───────────────────────────────────────────────────────

export interface MagazineStock {
  l1Barcode: string;
  magName: string;
  brandId: string;
  brandName: string;
  pSizeCode: string;
  pSize: string;
  stock: number;
  stkDt: string;
  re2: boolean;
  re12: boolean;
  parentL1?: string;
}

export interface MagazineStockSummary {
  magName: string;
  count: number;
  brandId?: string;
}

export interface ManualMagAllotRequest {
  mfgDt: string;
  magCode: string;
  plantCode: string;
  smallModels: { l1Barcode: string; brandId: string; psizeCode: string }[];
}

// ─── Dashboard ────────────────────────────────────────────────────────────────

export interface DashboardCard {
  totalL1?: number;
  totalL2?: number;
  totalL3?: number;
  totalDispatched?: number;
  totalStock?: number;
  totalPending?: number;
  [key: string]: unknown;
}

// ─── Dispatch Entities ────────────────────────────────────────────────────────

export interface Re11IndentInfo {
  indentNo: string;
  indentDt: string;
  pesoDt: string;
  custName: string;
  conName: string;
  conNo: string;
  clic: string;
  month: string;
  year: string;
  completedIndent?: boolean;
  productItems?: Re11IndentPrdInfo[];
}

export interface Re11IndentPrdInfo {
  id?: number;
  indentNo: string;
  brand: string;
  bid: string;
  psize: string;
  psizecode: string;
  reqWt: number;
  reqCase: number;
  remWt: number;
  remCase: number;
  loadWt?: number;
  loadCase?: number;
}

export interface AllLoadingSheet {
  id?: number;
  loadingSheetNo: string;
  tName: string;
  truckNo: string;
  compflag?: number;
  indentDetails: LoadingSheetDetail[];
}

export interface LoadingSheetDetail {
  indentNo: string;
  bname: string;
  bid: string;
  psize: string;
  sizeCode: string;
  loadWt: number;
  loadcase: number;
  mag: string;
  batch?: string;
  typeOfDispatch?: string;
}

export interface DispatchTransaction {
  tid?: number;
  indentNo: string;
  l1Barcode: string;
  brand: string;
  bid: string;
  pSize: string;
  pSizeCode: string;
  truckNo: string;
  magName: string;
  dispDt?: string;
  l1NetWt?: number;
  l1NetUnit?: string;
  re12?: boolean;
}

export interface RE12GenRequest {
  loadingSheet: string;
  indentNo: string;
  truckNo: string;
  brandName: string;
  brandId: string;
  productSize: string;
  pSizeCode: string;
  magname: string;
  loadcase: number;
}

// ─── Production ───────────────────────────────────────────────────────────────

export interface ProductionPlan {
  id?: number;
  mfgDt: string;
  plantCode: string;
  brandId: string;
  pSizeCode: string;
  machineCode: string;
  shift: string;
  noOfbox: number;
  noOfstickers: number;
}

// ─── Reports ──────────────────────────────────────────────────────────────────

export interface ReportParams {
  fromDate?: string;
  toDate?: string;
  reportType?: string;
  pcode?: string;
  plant?: string;
  month?: string;
  year?: string;
  brand?: string;
}
