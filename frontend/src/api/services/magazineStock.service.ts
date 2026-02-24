import { apiGet, apiPost } from '../baseService';
import type { MagazineStock, MagazineStockSummary, ManualMagAllotRequest } from '@/types/domain.types';

export const magazineStockService = {
  getAll: () => apiGet<MagazineStock[]>('/MagazineStocks/GetAllMagazineStock'),
  getSummary: () => apiGet<MagazineStockSummary[]>('/MagazineStocks/GetMagazinesstock'),
  create: (data: MagazineStock) => apiPost<MagazineStock>('/MagazineStocks/CreateMagazineStock', data),
};

export const magazineAllotmentService = {
  getManual: () => apiGet<unknown>('/MagzineAllotment/GetManualMagzineAllot'),
  getL1BarcodeDetails: (mfgdt: string, pcode: string, brandid: string, psizecode: string) =>
    apiGet<unknown[]>(`/MagzineAllotment/GetL1barcodeDetails/${mfgdt}/${pcode}/${brandid}/${psizecode}`),
  setAllotment: (data: ManualMagAllotRequest) =>
    apiPost<unknown>('/MagzineAllotment/SetMagzineAllotment', data),
};
