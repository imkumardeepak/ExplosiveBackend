import { apiGet, apiPost, apiPut, apiDelete, apiGetBlob } from '../baseService';
import type { L1Barcode, L1GenerateRequest, ProductionPlan } from '@/types/domain.types';

export const barcodeService = {
  getAll: (params?: Record<string, unknown>) =>
    apiGet<L1Barcode[]>('/L1Generate/GetL1Generate', params),
  getL1Only: () => apiGet<L1Barcode[]>('/L1Generate/GetOnlyL1L1Generate'),
  getByL1Number: (l1barcode: string) =>
    apiGet<L1Barcode>(`/L1Generate/GetL1detailsByL1Number/${l1barcode}`),
  generate: (data: L1GenerateRequest) => apiPost<L1Barcode>('/L1Generate/CreateL1Generate', data),
  update: (l1barcode: string, data: Partial<L1Barcode>) =>
    apiPut<L1Barcode>(`/L1Generate/UpdateL1Generate/${l1barcode}`, data),
  delete: (l1barcode: string) => apiDelete<void>(`/L1Generate/DeleteL1Generate/${l1barcode}`),
  getDashboardCards: () => apiGet<unknown>('/L1Generate/Getdashboardcard'),
  getProductionReport: (params: Record<string, unknown>) =>
    apiGet<unknown>('/L1Generate/Getproreport', params),
  generateFromPlan: (data: ProductionPlan) =>
    apiPost<L1Barcode>('/L1Generate/ProductionPlanL1Generate', data),
  getLastDetails: () => apiGet<unknown>('/L1Generate/Getlastl1l2l3Details'),
  downloadL1Report: (params: Record<string, unknown>) =>
    apiGetBlob('/Reports/ExportProReportToExcel', params),

  getReprintL1: (params?: Record<string, unknown>) =>
    apiGet<L1Barcode[]>('/ReprintBarcode/GetL1Reprintbarcd', params),
  getReprintL3: (params?: Record<string, unknown>) =>
    apiGet<unknown[]>('/ReprintBarcode/GetReprintL3barcode', params),
};
