import { useUIStore } from '@/store/uiStore';
import { useCurrentUser, useLogout } from '@/hooks/useAuth';

export const Topbar = () => {
  const toggleSidebar = useUIStore((s) => s.toggleSidebar);
  const user = useCurrentUser();
  const logout = useLogout();

  return (
    <header className="topbar">
      <button className="topbar__menu-btn" onClick={toggleSidebar} aria-label="Toggle sidebar">
        ☰
      </button>
      <div className="topbar__right">
        <span className="topbar__role">{user?.role}</span>
        <span className="topbar__username">{user?.username}</span>
        <button className="topbar__logout-btn" onClick={logout}>
          Logout
        </button>
      </div>
    </header>
  );
};
