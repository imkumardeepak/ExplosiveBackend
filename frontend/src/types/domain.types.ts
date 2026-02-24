export interface ProductMaster {
  id: number;
  productName: string;
  productCode: string;
  brandId: number;
  brand?: BrandMaster;
  isActive: boolean;
  createdAt: string;
}

export interface BrandMaster {
  id: number;
  brandName: string;
  brandCode: string;
  isActive: boolean;
}

export interface PlantMaster {
  id: number;
  plantName: string;
  plantCode: string;
  plantTypeId: number;
  isActive: boolean;
}

export interface MagazineMaster {
  id: number;
  magazineName: string;
  magazineCode: string;
  plantId: number;
  capacity: number;
  isActive: boolean;
}

export interface DashboardCard {
  totalL1Generated: number;
  totalL2Generated: number;
  totalL3Generated: number;
  totalDispatched: number;
  totalPendingDispatch: number;
  totalReturned: number;
}

export interface MagazineStock {
  magazineId: number;
  magazineName: string;
  productName: string;
  currentStock: number;
  allocatedStock: number;
  availableStock: number;
}

export interface ProductionPlan {
  id: number;
  plantId: number;
  productId: number;
  batchId: number;
  shiftId: number;
  plannedQuantity: number;
  producedQuantity: number;
  planDate: string;
  status: string;
}

export interface BarcodeData {
  id: number;
  barcodeNo: string;
  productId: number;
  level: 'L1' | 'L2' | 'L3';
  status: string;
  generatedAt: string;
}

export interface DispatchTransaction {
  id: number;
  indentNo: string;
  customerId: number;
  vehicleNo: string;
  dispatchDate: string;
  status: string;
  totalQuantity: number;
}

export interface CustomerMaster {
  id: number;
  customerName: string;
  customerCode: string;
  stateId: number;
  isActive: boolean;
}
