import { useQuery } from '@tanstack/react-query';
import { dashboardService } from '@/api/services/dashboard.service';
import type { MagazineStockSummary } from '@/types/domain.types';

const StatCard = ({ label, value, color }: { label: string; value: number | string; color?: string }) => (
  <div className={`stat-card ${color ? `stat-card--${color}` : ''}`}>
    <span className="stat-card__label">{label}</span>
    <span className="stat-card__value">{value}</span>
  </div>
);

const DashboardPage = () => {
  const { data: cards, isLoading: cardsLoading, isError: cardsError } = useQuery({
    queryKey: ['dashboard', 'cards'],
    queryFn: () => dashboardService.getDashboardCards(),
  });

  const { data: magazineStock = [], isLoading: stockLoading } = useQuery({
    queryKey: ['dashboard', 'magazine-stock'],
    queryFn: () => dashboardService.getMagazineStock(),
  });

  if (cardsLoading) {
    return <div className="page-loading">Loading dashboard...</div>;
  }

  if (cardsError) {
    return <div className="page-error">Failed to load dashboard data.</div>;
  }

  const data = cards as Record<string, unknown> | undefined;

  const getNum = (key: string) => {
    if (!data) return 0;
    const val = data[key];
    return typeof val === 'number' ? val : 0;
  };

  return (
    <div className="dashboard">
      <h1 className="page-title">Dashboard</h1>

      <div className="stat-grid">
        <StatCard label="Total L1 Generated" value={getNum('totalL1') || getNum('l1Count') || getNum('total')} color="blue" />
        <StatCard label="Total L2 Generated" value={getNum('totalL2') || getNum('l2Count')} color="green" />
        <StatCard label="Total L3 Generated" value={getNum('totalL3') || getNum('l3Count')} color="purple" />
        <StatCard label="Total Dispatched" value={getNum('totalDispatched') || getNum('dispatched')} color="orange" />
        <StatCard label="In Magazine (Stock)" value={getNum('totalStock') || getNum('stock')} color="teal" />
        <StatCard label="Pending Dispatch" value={getNum('totalPending') || getNum('pending')} color="red" />
      </div>

      <section className="dashboard__section">
        <h2 className="section-title">Magazine Stock Overview</h2>
        {stockLoading ? (
          <div className="section-loading">Loading stock data...</div>
        ) : (
          <div className="stock-table-wrapper">
            <table className="data-table">
              <thead>
                <tr>
                  <th>Magazine</th>
                  <th>Stock Count</th>
                  <th>Brand ID</th>
                </tr>
              </thead>
              <tbody>
                {(magazineStock as MagazineStockSummary[]).length === 0 ? (
                  <tr>
                    <td colSpan={3} className="table-empty">No magazine stock data available.</td>
                  </tr>
                ) : (
                  (magazineStock as MagazineStockSummary[]).map((stock, idx) => (
                    <tr key={`${stock.magName}-${idx}`}>
                      <td>{stock.magName}</td>
                      <td>{stock.count}</td>
                      <td>{stock.brandId ?? '—'}</td>
                    </tr>
                  ))
                )}
              </tbody>
            </table>
          </div>
        )}
      </section>
    </div>
  );
};

export default DashboardPage;
