import { apiGet, apiPost, apiDelete } from '../baseService';
import type { Re11IndentInfo, AllLoadingSheet } from '@/types/domain.types';

export const dispatchService = {
  getAllIndents: () => apiGet<Re11IndentInfo[]>('/Re11IndentInfos/GetAllIndents'),
  getIndentNumbers: () => apiGet<string[]>('/Re11IndentInfos/GetOnlyIndentsNo'),
  getIndentByNo: (indentNo: string) =>
    apiGet<Re11IndentInfo>(`/Re11IndentInfos/GetIndentByNo/${indentNo}`),
  createIndent: (data: Re11IndentInfo) =>
    apiPost<Re11IndentInfo>('/Re11IndentInfos/CreateIndent', data),
  deleteIndent: (indentNo: string) =>
    apiDelete<void>(`/Re11IndentInfos/Deleteindent/${indentNo}`),

  getAllLoadingSheets: () => apiGet<AllLoadingSheet[]>('/AllLoadingSheets/GetAllLoadingSheets'),
  getLoadingSheetsByFlag: (cflag: number) =>
    apiGet<AllLoadingSheet[]>(`/AllLoadingSheets/GetLoadingSheetsByCFlag/${cflag}`),
  getLoadingSheetById: (id: number) =>
    apiGet<AllLoadingSheet>(`/AllLoadingSheets/GetAllLoadingSheetById/${id}`),
  createLoadingSheet: (data: AllLoadingSheet) =>
    apiPost<AllLoadingSheet>('/AllLoadingSheets/CreateAllLoadingSheet', data),
  deleteLoadingSheet: (id: number) =>
    apiDelete<void>(`/AllLoadingSheets/DeleteAllLoadingSheet/${id}`),
  getIndentsByCustomer: (custname: string) =>
    apiGet<Re11IndentInfo[]>(`/AllLoadingSheets/GetIndentsByCName/${custname}`),
};
