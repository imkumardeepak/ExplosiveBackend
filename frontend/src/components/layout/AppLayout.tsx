import { Outlet } from 'react-router-dom';
import { Sidebar } from './Sidebar';
import { Topbar } from './Topbar';
import { ToastContainer } from '@/components/ui/Toast';
import { useUIStore } from '@/store/uiStore';

const AppLayout = () => {
  const isSidebarOpen = useUIStore((s) => s.isSidebarOpen);

  return (
    <div className={`app-layout ${isSidebarOpen ? 'sidebar-open' : 'sidebar-closed'}`}>
      <Sidebar />
      <div className="app-layout__main">
        <Topbar />
        <main className="app-layout__content">
          <Outlet />
        </main>
      </div>
      <ToastContainer />
    </div>
  );
};

export default AppLayout;
