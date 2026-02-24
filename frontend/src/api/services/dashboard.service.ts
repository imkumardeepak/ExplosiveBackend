import { apiGet } from '../baseService';
import type { DashboardCard, MagazineStock } from '@/types/domain.types';

export const dashboardService = {
  getDashboardCards: () => apiGet<DashboardCard>('/Dashboard/Getdashboardcard'),
  getMagazineStock: () => apiGet<MagazineStock[]>('/Dashboard/GetMagazinesstock'),
  getRE2Data: () => apiGet<unknown[]>('/Dashboard/GetRE2'),
  getAllotData: () => apiGet<unknown[]>('/Dashboard/Getallot'),
  getDispatchStatus: () => apiGet<unknown[]>('/Dashboard/GetdispatchStatus'),
  getDispatch: () => apiGet<unknown[]>('/Dashboard/GetDispatch'),
  getProductionDispatchData: (plantName: string, timeRange: string) =>
    apiGet<{ production: unknown[]; dispatch: unknown[] }>(
      '/Dashboard/GetProductionDispatchData',
      { plantName, timeRange },
    ),
  getSlurryData: (timeRange: string) =>
    apiGet<unknown[]>('/Dashboard/GetSlurryData', { timeRange }),
  getDetonatorData: (timeRange: string) =>
    apiGet<unknown[]>('/Dashboard/GetDetonatorData', { timeRange }),
  getEmultionData: (timeRange: string) =>
    apiGet<unknown[]>('/Dashboard/GetEmultionData', { timeRange }),
  getPETNData: (timeRange: string) =>
    apiGet<unknown[]>('/Dashboard/GetPETNData', { timeRange }),
};
