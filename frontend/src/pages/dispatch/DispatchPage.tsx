import { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { dispatchService } from '@/api/services/dispatch.service';
import { DataTable } from '@/components/ui/DataTable';
import type { Re11IndentInfo, AllLoadingSheet } from '@/types/domain.types';

type DispatchTab = 'indents' | 'loading-sheets';

const DispatchPage = () => {
  const [activeTab, setActiveTab] = useState<DispatchTab>('indents');

  const { data: indents = [], isLoading: indentsLoading } = useQuery({
    queryKey: ['indents'],
    queryFn: () => dispatchService.getAllIndents(),
    enabled: activeTab === 'indents',
  });

  const { data: loadingSheets = [], isLoading: sheetsLoading } = useQuery({
    queryKey: ['loading-sheets'],
    queryFn: () => dispatchService.getAllLoadingSheets(),
    enabled: activeTab === 'loading-sheets',
  });

  const indentColumns = [
    { key: 'indentNo' as keyof Re11IndentInfo, header: 'Indent No.' },
    { key: 'indentDt' as keyof Re11IndentInfo, header: 'Indent Date' },
    { key: 'custName' as keyof Re11IndentInfo, header: 'Customer' },
    { key: 'conName' as keyof Re11IndentInfo, header: 'Consignee' },
    { key: 'month' as keyof Re11IndentInfo, header: 'Month' },
    { key: 'year' as keyof Re11IndentInfo, header: 'Year' },
    {
      key: 'completedIndent',
      header: 'Status',
      render: (row: Re11IndentInfo) => (
        <span className={`badge ${row.completedIndent ? 'badge--success' : 'badge--warning'}`}>
          {row.completedIndent ? 'Completed' : 'Pending'}
        </span>
      ),
    },
  ];

  const sheetColumns = [
    { key: 'loadingSheetNo' as keyof AllLoadingSheet, header: 'Sheet No.' },
    { key: 'tName' as keyof AllLoadingSheet, header: 'Transporter' },
    { key: 'truckNo' as keyof AllLoadingSheet, header: 'Truck No.' },
    {
      key: 'compflag',
      header: 'Status',
      render: (row: AllLoadingSheet) => {
        const labels: Record<number, string> = { 0: 'Pending', 1: 'Dispatched', 2: 'RE-12 Done' };
        const classes: Record<number, string> = { 0: 'badge--warning', 1: 'badge--info', 2: 'badge--success' };
        const flag = row.compflag ?? 0;
        return (
          <span className={`badge ${classes[flag] ?? 'badge--warning'}`}>
            {labels[flag] ?? 'Unknown'}
          </span>
        );
      },
    },
  ];

  return (
    <div className="page">
      <div className="page-header">
        <h1 className="page-title">Dispatch Management</h1>
      </div>

      <div className="tabs">
        <button
          className={`tab ${activeTab === 'indents' ? 'tab--active' : ''}`}
          onClick={() => setActiveTab('indents')}
        >
          RE-11 Indents
        </button>
        <button
          className={`tab ${activeTab === 'loading-sheets' ? 'tab--active' : ''}`}
          onClick={() => setActiveTab('loading-sheets')}
        >
          Loading Sheets
        </button>
      </div>

      <div className="tab-content">
        {activeTab === 'indents' && (
          <DataTable
            columns={indentColumns}
            data={indents}
            isLoading={indentsLoading}
            keyField="indentNo"
            emptyMessage="No indents found."
          />
        )}
        {activeTab === 'loading-sheets' && (
          <DataTable
            columns={sheetColumns}
            data={loadingSheets}
            isLoading={sheetsLoading}
            keyField="id"
            emptyMessage="No loading sheets found."
          />
        )}
      </div>
    </div>
  );
};

export default DispatchPage;
