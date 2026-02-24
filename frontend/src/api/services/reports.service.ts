import { apiGet, apiGetBlob } from '../baseService';
import type { ReportParams } from '@/types/domain.types';

export const reportsService = {
  getProductionReport: (params: ReportParams) =>
    apiGet<unknown[]>('/Reports/Getproreport', params as Record<string, unknown>),
  exportProductionReportExcel: (params: ReportParams) =>
    apiGetBlob('/Reports/ExportProReportToExcel', params as Record<string, unknown>),

  getStockReport: (params: ReportParams) =>
    apiGet<unknown[]>('/Reports/Getstockreport', params as Record<string, unknown>),
  exportStockReportExcel: (params: ReportParams) =>
    apiGetBlob('/Reports/ExportStockReportToExcel', params as Record<string, unknown>),

  getL2ReprintData: (params: ReportParams) =>
    apiGet<unknown[]>('/Reports/Getl2reprintdata', params as Record<string, unknown>),
  exportL2ReprintExcel: (params: ReportParams) =>
    apiGetBlob('/Reports/ExportL2ReprintDataToExcel', params as Record<string, unknown>),

  getL1ReprintData: (params: ReportParams) =>
    apiGet<unknown[]>('/Reports/Getl1reprintdata', params as Record<string, unknown>),
  exportL1ReprintExcel: (params: ReportParams) =>
    apiGetBlob('/Reports/ExportL1ReprintDataToExcel', params as Record<string, unknown>),

  getL1BoxData: (params: ReportParams) =>
    apiGet<unknown[]>('/Reports/Getl1boxdata', params as Record<string, unknown>),
  exportL1BoxExcel: (params: ReportParams) =>
    apiGetBlob('/Reports/ExportL1BoxDataToExcel', params as Record<string, unknown>),

  getRE7Report: (params: ReportParams) =>
    apiGet<unknown[]>('/Reports/Getre7report', params as Record<string, unknown>),
  exportRE7Excel: (params: ReportParams) =>
    apiGetBlob('/Reports/ExportRE7ReportToExcel', params as Record<string, unknown>),

  getDispatchData: (params: ReportParams) =>
    apiGet<unknown[]>('/Reports/GetDispatchData', params as Record<string, unknown>),
  exportDispatchExcel: (params: ReportParams) =>
    apiGetBlob('/Reports/ExportDispatchDataToExcel', params as Record<string, unknown>),

  getRE11StatusData: (params: ReportParams) =>
    apiGet<unknown[]>('/Reports/GetRE11statusdata', params as Record<string, unknown>),
  exportRE11StatusExcel: (params: ReportParams) =>
    apiGetBlob('/Reports/ExportRE11StatusDataToExcel', params as Record<string, unknown>),

  getRE2StatusData: (params: ReportParams) =>
    apiGet<unknown[]>('/Reports/GetRE2statusdata', params as Record<string, unknown>),
  exportRE2StatusExcel: (params: ReportParams) =>
    apiGetBlob('/Reports/ExportRE2StatusDataToExcel', params as Record<string, unknown>),

  getProTransFetchData: (params: ReportParams) =>
    apiGet<unknown[]>('/Reports/Getprotransfetchdata', params as Record<string, unknown>),
  exportProTransExcel: (params: ReportParams) =>
    apiGetBlob('/Reports/ExportProTransFetchDataToExcel', params as Record<string, unknown>),
};
