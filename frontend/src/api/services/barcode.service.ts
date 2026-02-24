import { apiGet, apiPost, apiGetBlob } from '../baseService';
import type { BarcodeData } from '@/types/domain.types';

export const barcodeService = {
  getAll: () => apiGet<BarcodeData[]>('/BarcodeDatas/GetAllBarcodes'),
  searchBarcode: (barcodeNo: string) =>
    apiGet<BarcodeData>('/BarcodeDatas/SearchBarcode', { barcodeNo }),
  generateL1: (data: unknown) => apiPost<BarcodeData[]>('/L1Generate/GenerateL1', data),
  reprintBarcode: (data: unknown) => apiPost<Blob>('/ReprintBarcode/Reprint', data),
  downloadL1Report: (params: Record<string, unknown>) =>
    apiGetBlob('/L1ReprintReport/DownloadReport', params),
};
