import { useDashboardCards, useMagazineStock } from '@/hooks/useDashboard';
import type { DashboardCard, MagazineStock } from '@/types/domain.types';

const StatCard = ({ label, value }: { label: string; value: number | string }) => (
  <div className="stat-card">
    <span className="stat-card__label">{label}</span>
    <span className="stat-card__value">{value}</span>
  </div>
);

const DashboardPage = () => {
  const { data: cards, isLoading: cardsLoading, isError: cardsError } = useDashboardCards();
  const { data: magazineStock, isLoading: stockLoading } = useMagazineStock();

  if (cardsLoading) {
    return <div className="page-loading">Loading dashboard...</div>;
  }

  if (cardsError) {
    return <div className="page-error">Failed to load dashboard data.</div>;
  }

  const dashboardData = cards as DashboardCard | undefined;

  return (
    <div className="dashboard">
      <h1 className="page-title">Dashboard</h1>
      <div className="stat-grid">
        <StatCard label="L1 Generated" value={dashboardData?.totalL1Generated ?? 0} />
        <StatCard label="L2 Generated" value={dashboardData?.totalL2Generated ?? 0} />
        <StatCard label="L3 Generated" value={dashboardData?.totalL3Generated ?? 0} />
        <StatCard label="Dispatched" value={dashboardData?.totalDispatched ?? 0} />
        <StatCard label="Pending Dispatch" value={dashboardData?.totalPendingDispatch ?? 0} />
        <StatCard label="Returned" value={dashboardData?.totalReturned ?? 0} />
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
                  <th>Product</th>
                  <th>Current Stock</th>
                  <th>Allocated</th>
                  <th>Available</th>
                </tr>
              </thead>
              <tbody>
                {(magazineStock as MagazineStock[] | undefined)?.map((stock) => (
                  <tr key={stock.magazineId}>
                    <td>{stock.magazineName}</td>
                    <td>{stock.productName}</td>
                    <td>{stock.currentStock}</td>
                    <td>{stock.allocatedStock}</td>
                    <td>{stock.availableStock}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </section>
    </div>
  );
};

export default DashboardPage;
