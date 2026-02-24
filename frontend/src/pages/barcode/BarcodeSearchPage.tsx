import { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { barcodeService } from '@/api/services/barcode.service';
import { DataTable } from '@/components/ui/DataTable';
import { FormField } from '@/components/forms/FormField';
import { TextInput } from '@/components/forms/TextInput';
import type { L1Barcode } from '@/types/domain.types';

const BarcodeSearchPage = () => {
  const [searchBarcode, setSearchBarcode] = useState('');
  const [submittedBarcode, setSubmittedBarcode] = useState('');
  const [fromDate, setFromDate] = useState('');
  const [toDate, setToDate] = useState('');
  const [submittedParams, setSubmittedParams] = useState<Record<string, unknown> | null>(null);

  const { data: barcodeDetail, isLoading: detailLoading, isError: detailError } = useQuery({
    queryKey: ['barcode', 'detail', submittedBarcode],
    queryFn: () => barcodeService.getByL1Number(submittedBarcode),
    enabled: !!submittedBarcode,
  });

  const { data: barcodeList = [], isLoading: listLoading } = useQuery({
    queryKey: ['barcodes', 'list', submittedParams],
    queryFn: () => barcodeService.getAll(submittedParams ?? {}),
    enabled: !!submittedParams,
  });

  const handleBarcodeSearch = (e: React.FormEvent) => {
    e.preventDefault();
    if (searchBarcode.trim()) {
      setSubmittedBarcode(searchBarcode.trim());
      setSubmittedParams(null);
    }
  };

  const handleRangeSearch = (e: React.FormEvent) => {
    e.preventDefault();
    setSubmittedBarcode('');
    setSubmittedParams({ fromDate, toDate });
  };

  const listColumns = [
    { key: 'l1Barcode' as keyof L1Barcode, header: 'L1 Barcode' },
    { key: 'brandName' as keyof L1Barcode, header: 'Brand' },
    { key: 'productSize' as keyof L1Barcode, header: 'Size' },
    { key: 'plantName' as keyof L1Barcode, header: 'Plant' },
    { key: 'mfgDt' as keyof L1Barcode, header: 'Mfg. Date' },
    { key: 'shift' as keyof L1Barcode, header: 'Shift' },
    { key: 'noOfL2' as keyof L1Barcode, header: 'L2 Count' },
    { key: 'noOfL3' as keyof L1Barcode, header: 'L3 Count' },
    {
      key: 'mFlag',
      header: 'In Magazine',
      render: (row: L1Barcode) => (
        <span className={`badge ${row.mFlag ? 'badge--success' : 'badge--warning'}`}>
          {row.mFlag ? 'Yes' : 'No'}
        </span>
      ),
    },
  ];

  return (
    <div className="page">
      <div className="page-header">
        <h1 className="page-title">Barcode Search</h1>
      </div>

      <div className="search-panels">
        <div className="card">
          <h3 className="card-title">Search by L1 Barcode</h3>
          <form onSubmit={handleBarcodeSearch} className="search-form">
            <FormField label="L1 Barcode Number">
              <TextInput
                value={searchBarcode}
                onChange={(e) => setSearchBarcode(e.target.value)}
                placeholder="Enter full L1 barcode..."
              />
            </FormField>
            <button type="submit" className="btn btn--primary" disabled={!searchBarcode.trim()}>
              Search
            </button>
          </form>

          {detailLoading && <div className="loading-text">Searching...</div>}
          {detailError && <div className="error-text">Barcode not found.</div>}
          {barcodeDetail && (
            <div className="barcode-detail">
              <h4 className="detail-title">Barcode Details</h4>
              <div className="detail-grid">
                <div className="detail-row"><span>L1 Barcode:</span><strong>{barcodeDetail.l1Barcode}</strong></div>
                <div className="detail-row"><span>Brand:</span><strong>{barcodeDetail.brandName}</strong></div>
                <div className="detail-row"><span>Brand ID:</span><strong>{barcodeDetail.brandId}</strong></div>
                <div className="detail-row"><span>Product Size:</span><strong>{barcodeDetail.productSize}</strong></div>
                <div className="detail-row"><span>Size Code:</span><strong>{barcodeDetail.pSizeCode}</strong></div>
                <div className="detail-row"><span>Plant:</span><strong>{barcodeDetail.plantName}</strong></div>
                <div className="detail-row"><span>Mfg. Date:</span><strong>{barcodeDetail.mfgDt}</strong></div>
                <div className="detail-row"><span>Shift:</span><strong>{barcodeDetail.shift}</strong></div>
                <div className="detail-row"><span>Net Weight:</span><strong>{barcodeDetail.l1NetWt} {barcodeDetail.l1NetUnit}</strong></div>
                <div className="detail-row"><span>L2 Count:</span><strong>{barcodeDetail.noOfL2}</strong></div>
                <div className="detail-row"><span>L3 Count:</span><strong>{barcodeDetail.noOfL3}</strong></div>
                <div className="detail-row">
                  <span>In Magazine:</span>
                  <strong>
                    <span className={`badge ${barcodeDetail.mFlag ? 'badge--success' : 'badge--warning'}`}>
                      {barcodeDetail.mFlag ? 'Yes' : 'No'}
                    </span>
                  </strong>
                </div>
              </div>
            </div>
          )}
        </div>

        <div className="card">
          <h3 className="card-title">Search by Date Range</h3>
          <form onSubmit={handleRangeSearch} className="search-form">
            <FormField label="From Date">
              <TextInput
                type="date"
                value={fromDate}
                onChange={(e) => setFromDate(e.target.value)}
              />
            </FormField>
            <FormField label="To Date">
              <TextInput
                type="date"
                value={toDate}
                onChange={(e) => setToDate(e.target.value)}
              />
            </FormField>
            <button type="submit" className="btn btn--primary">
              Search
            </button>
          </form>
        </div>
      </div>

      {submittedParams && (
        <div className="search-results">
          <h3 className="section-title">Search Results</h3>
          <DataTable
            columns={listColumns}
            data={barcodeList}
            isLoading={listLoading}
            keyField="l1Barcode"
            emptyMessage="No barcodes found for the selected criteria."
          />
        </div>
      )}
    </div>
  );
};

export default BarcodeSearchPage;
