import { NavLink } from 'react-router-dom';
import { useUIStore } from '@/store/uiStore';
import env from '@/lib/env';

type NavLink = { to: string; label: string; icon: string; children?: never };
type NavGroup = { label: string; icon: string; children: { to: string; label: string }[]; to?: never };
type NavItem = NavLink | NavGroup;

const NAV_ITEMS: NavItem[] = [
  { to: '/dashboard', label: 'Dashboard', icon: '📊' },
  {
    label: 'Masters',
    icon: '📋',
    children: [
      { to: '/masters/products', label: 'Products' },
      { to: '/masters/brands', label: 'Brands' },
      { to: '/masters/plants', label: 'Plants' },
      { to: '/masters/customers', label: 'Customers' },
      { to: '/masters/magazines', label: 'Magazines' },
    ],
  },
  {
    label: 'Barcode',
    icon: '🔲',
    children: [
      { to: '/barcode/generate', label: 'Generate' },
      { to: '/barcode/search', label: 'Search' },
    ],
  },
  { to: '/production/plan', label: 'Production', icon: '🏭' },
  { to: '/dispatch', label: 'Dispatch', icon: '🚚' },
  { to: '/reports', label: 'Reports', icon: '📄' },
];

export const Sidebar = () => {
  const isSidebarOpen = useUIStore((s) => s.isSidebarOpen);

  return (
    <aside className={`sidebar ${isSidebarOpen ? 'sidebar--open' : 'sidebar--closed'}`}>
      <div className="sidebar__logo">
        <span className="sidebar__logo-icon">🏷️</span>
        {isSidebarOpen && <span className="sidebar__logo-text">{env.APP_NAME}</span>}
      </div>
      <nav className="sidebar__nav">
        {NAV_ITEMS.map((item) =>
          'children' in item && item.children ? (
            <NavGroup key={item.label} item={item as NavGroup} isOpen={isSidebarOpen} />
          ) : (
            <NavLink
              key={item.to}
              to={item.to}
              className={({ isActive }) =>
                `sidebar__item ${isActive ? 'sidebar__item--active' : ''}`
              }
            >
              <span className="sidebar__item-icon">{item.icon}</span>
              {isSidebarOpen && <span className="sidebar__item-label">{item.label}</span>}
            </NavLink>
          ),
        )}
      </nav>
    </aside>
  );
};

const NavGroup = ({ item, isOpen }: { item: NavGroup; isOpen: boolean }) => (
  <div className="sidebar__group">
    <div className="sidebar__group-header">
      <span className="sidebar__item-icon">{item.icon}</span>
      {isOpen && <span className="sidebar__item-label">{item.label}</span>}
    </div>
    {isOpen && (
      <div className="sidebar__group-children">
        {item.children.map((child) => (
          <NavLink
            key={child.to}
            to={child.to}
            className={({ isActive }) =>
              `sidebar__child-item ${isActive ? 'sidebar__child-item--active' : ''}`
            }
          >
            {child.label}
          </NavLink>
        ))}
      </div>
    )}
  </div>
);
