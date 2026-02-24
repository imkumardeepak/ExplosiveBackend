import { apiGet } from '../baseService';
import type { DashboardCard, MagazineStockSummary } from '@/types/domain.types';

export const dashboardService = {
  getDashboardCards: () => apiGet<DashboardCard>('/Dashboard/Getdashboardcard'),
  getMagazineStock: () => apiGet<MagazineStockSummary[]>('/Dashboard/GetMagazinesstock'),
  getRE2: () => apiGet<unknown>('/Dashboard/GetRE2'),
  getAllot: () => apiGet<unknown>('/Dashboard/Getallot'),
  getDispatchStatus: () => apiGet<unknown>('/Dashboard/GetdispatchStatus'),
  getDispatch: () => apiGet<unknown>('/Dashboard/GetDispatch'),
  getProductionDispatchData: (fromDate?: string, toDate?: string) =>
    apiGet<unknown>('/Dashboard/GetProductionDispatchData', { fromDate, toDate }),
  getSlurryData: (fromDate?: string, toDate?: string) =>
    apiGet<unknown>('/Dashboard/GetSlurryData', { fromDate, toDate }),
  getDetonatorData: (fromDate?: string, toDate?: string) =>
    apiGet<unknown>('/Dashboard/GetDetonatorData', { fromDate, toDate }),
  getEmulsionData: (fromDate?: string, toDate?: string) =>
    apiGet<unknown>('/Dashboard/GetEmultionData', { fromDate, toDate }),
  getPETNData: (fromDate?: string, toDate?: string) =>
    apiGet<unknown>('/Dashboard/GetPETNData', { fromDate, toDate }),
};
