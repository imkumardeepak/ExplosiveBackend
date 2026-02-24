import { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { reportsService } from '@/api/services/reports.service';
import { DataTable } from '@/components/ui/DataTable';
import { FormField } from '@/components/forms/FormField';
import { TextInput } from '@/components/forms/TextInput';
import type { ReportParams } from '@/types/domain.types';
import { useUIStore } from '@/store/uiStore';

type ReportType = 'production' | 'stock' | 'dispatch' | 're11-status' | 're2-status' | 're7' | 'l1-reprint' | 'l2-reprint';

interface ReportConfig {
  label: string;
  key: ReportType;
  hasDateRange: boolean;
  hasMonthYear: boolean;
}

const REPORT_CONFIGS: ReportConfig[] = [
  { label: 'Production Report', key: 'production', hasDateRange: true, hasMonthYear: false },
  { label: 'Stock Report', key: 'stock', hasDateRange: true, hasMonthYear: false },
  { label: 'Dispatch Report', key: 'dispatch', hasDateRange: true, hasMonthYear: false },
  { label: 'RE-11 Status', key: 're11-status', hasDateRange: true, hasMonthYear: false },
  { label: 'RE-2 Status', key: 're2-status', hasDateRange: true, hasMonthYear: false },
  { label: 'RE-7 Monthly', key: 're7', hasDateRange: false, hasMonthYear: true },
  { label: 'L1 Reprint', key: 'l1-reprint', hasDateRange: true, hasMonthYear: false },
  { label: 'L2 Reprint', key: 'l2-reprint', hasDateRange: true, hasMonthYear: false },
];

const downloadBlob = (blob: Blob, filename: string) => {
  const url = URL.createObjectURL(blob);
  const a = document.createElement('a');
  a.href = url;
  a.download = filename;
  a.click();
  URL.revokeObjectURL(url);
};

const ReportsPage = () => {
  const addToast = useUIStore((s) => s.addToast);
  const [activeReport, setActiveReport] = useState<ReportType>('production');
  const [params, setParams] = useState<ReportParams>({ fromDate: '', toDate: '', month: '', year: '' });
  const [submittedParams, setSubmittedParams] = useState<ReportParams | null>(null);
  const [isExporting, setIsExporting] = useState(false);

  const activeConfig = REPORT_CONFIGS.find((r) => r.key === activeReport)!;

  const { data: reportData = [], isLoading } = useQuery({
    queryKey: ['report', activeReport, submittedParams],
    queryFn: () => {
      const p = submittedParams ?? {};
      switch (activeReport) {
        case 'production': return reportsService.getProductionReport(p);
        case 'stock': return reportsService.getStockReport(p);
        case 'dispatch': return reportsService.getDispatchData(p);
        case 're11-status': return reportsService.getRE11StatusData(p);
        case 're2-status': return reportsService.getRE2StatusData(p);
        case 're7': return reportsService.getRE7Report(p);
        case 'l1-reprint': return reportsService.getL1ReprintData(p);
        case 'l2-reprint': return reportsService.getL2ReprintData(p);
        default: return Promise.resolve([]);
      }
    },
    enabled: !!submittedParams,
  });

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault();
    setSubmittedParams({ ...params });
  };

  const handleExport = async () => {
    setIsExporting(true);
    try {
      const p = submittedParams ?? {};
      let blob: Blob;
      let filename = `${activeReport}-report.xlsx`;
      switch (activeReport) {
        case 'production':
          blob = await reportsService.exportProductionReportExcel(p);
          filename = 'production-report.xlsx';
          break;
        case 'stock':
          blob = await reportsService.exportStockReportExcel(p);
          filename = 'stock-report.xlsx';
          break;
        case 'dispatch':
          blob = await reportsService.exportDispatchExcel(p);
          filename = 'dispatch-report.xlsx';
          break;
        case 're11-status':
          blob = await reportsService.exportRE11StatusExcel(p);
          filename = 're11-status.xlsx';
          break;
        case 're2-status':
          blob = await reportsService.exportRE2StatusExcel(p);
          filename = 're2-status.xlsx';
          break;
        case 're7':
          blob = await reportsService.exportRE7Excel(p);
          filename = 're7-report.xlsx';
          break;
        case 'l1-reprint':
          blob = await reportsService.exportL1ReprintExcel(p);
          filename = 'l1-reprint.xlsx';
          break;
        case 'l2-reprint':
          blob = await reportsService.exportL2ReprintExcel(p);
          filename = 'l2-reprint.xlsx';
          break;
        default:
          return;
      }
      downloadBlob(blob, filename);
      addToast({ type: 'success', message: 'Export downloaded.' });
    } catch {
      addToast({ type: 'error', message: 'Export failed. Please try again.' });
    } finally {
      setIsExporting(false);
    }
  };

  const dataRows = reportData as Record<string, unknown>[];
  const columns = dataRows.length > 0
    ? Object.keys(dataRows[0]).slice(0, 8).map((key) => ({
        key,
        header: key.charAt(0).toUpperCase() + key.slice(1),
        render: (row: Record<string, unknown>) => String(row[key] ?? ''),
      }))
    : [];

  return (
    <div className="page">
      <div className="page-header">
        <h1 className="page-title">Reports</h1>
      </div>

      <div className="report-layout">
        <div className="report-sidebar">
          {REPORT_CONFIGS.map((config) => (
            <button
              key={config.key}
              className={`report-nav-item ${activeReport === config.key ? 'report-nav-item--active' : ''}`}
              onClick={() => { setActiveReport(config.key); setSubmittedParams(null); }}
            >
              {config.label}
            </button>
          ))}
        </div>

        <div className="report-content">
          <div className="card">
            <h3 className="card-title">{activeConfig.label}</h3>
            <form onSubmit={handleSearch} className="report-filters">
              {activeConfig.hasDateRange && (
                <>
                  <FormField label="From Date">
                    <TextInput
                      type="date"
                      value={params.fromDate ?? ''}
                      onChange={(e) => setParams((p) => ({ ...p, fromDate: e.target.value }))}
                    />
                  </FormField>
                  <FormField label="To Date">
                    <TextInput
                      type="date"
                      value={params.toDate ?? ''}
                      onChange={(e) => setParams((p) => ({ ...p, toDate: e.target.value }))}
                    />
                  </FormField>
                </>
              )}
              {activeConfig.hasMonthYear && (
                <>
                  <FormField label="Month">
                    <select
                      className="text-input"
                      value={params.month ?? ''}
                      onChange={(e) => setParams((p) => ({ ...p, month: e.target.value }))}
                    >
                      <option value="">Select month...</option>
                      {['January','February','March','April','May','June','July','August','September','October','November','December'].map((m) => (
                        <option key={m} value={m}>{m}</option>
                      ))}
                    </select>
                  </FormField>
                  <FormField label="Year">
                    <TextInput
                      type="number"
                      value={params.year ?? ''}
                      onChange={(e) => setParams((p) => ({ ...p, year: e.target.value }))}
                      placeholder="e.g. 2026"
                    />
                  </FormField>
                </>
              )}
              <div className="report-filter-actions">
                <button type="submit" className="btn btn--primary">
                  Load Report
                </button>
                {submittedParams && (
                  <button
                    type="button"
                    className="btn btn--secondary"
                    onClick={handleExport}
                    disabled={isExporting || dataRows.length === 0}
                  >
                    {isExporting ? 'Exporting...' : '⬇ Export Excel'}
                  </button>
                )}
              </div>
            </form>
          </div>

          {submittedParams && (
            <div className="report-table">
              {columns.length > 0 ? (
                <DataTable
                  columns={columns as Parameters<typeof DataTable>[0]['columns']}
                  data={dataRows}
                  isLoading={isLoading}
                  keyField={columns[0]?.key ?? 'id'}
                  emptyMessage="No data found for the selected criteria."
                />
              ) : !isLoading ? (
                <div className="empty-state">No data found for the selected criteria.</div>
              ) : (
                <div className="loading-text">Loading report...</div>
              )}
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default ReportsPage;
