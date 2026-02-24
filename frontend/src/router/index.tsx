import { lazy, Suspense } from 'react';
import { createBrowserRouter, RouterProvider, Navigate } from 'react-router-dom';
import { ProtectedRoute } from './ProtectedRoute';
import { PageLoader } from '@/components/ui/PageLoader';

const AppLayout = lazy(() => import('@/components/layout/AppLayout'));
const LoginPage = lazy(() => import('@/pages/LoginPage'));
const DashboardPage = lazy(() => import('@/pages/DashboardPage'));
const UnauthorizedPage = lazy(() => import('@/pages/UnauthorizedPage'));

const ProductsPage = lazy(() => import('@/pages/masters/ProductsPage'));
const BrandsPage = lazy(() => import('@/pages/masters/BrandsPage'));
const PlantsPage = lazy(() => import('@/pages/masters/PlantsPage'));
const CustomersPage = lazy(() => import('@/pages/masters/CustomersPage'));
const MagazinesPage = lazy(() => import('@/pages/masters/MagazinesPage'));

const BarcodeGeneratePage = lazy(() => import('@/pages/barcode/BarcodeGeneratePage'));
const BarcodeSearchPage = lazy(() => import('@/pages/barcode/BarcodeSearchPage'));

const ProductionPlanPage = lazy(() => import('@/pages/production/ProductionPlanPage'));
const DispatchPage = lazy(() => import('@/pages/dispatch/DispatchPage'));
const ReportsPage = lazy(() => import('@/pages/reports/ReportsPage'));

const withSuspense = (Component: React.ComponentType) => (
  <Suspense fallback={<PageLoader />}>
    <Component />
  </Suspense>
);

const router = createBrowserRouter([
  {
    path: '/login',
    element: withSuspense(LoginPage),
  },
  {
    path: '/unauthorized',
    element: withSuspense(UnauthorizedPage),
  },
  {
    element: <ProtectedRoute />,
    children: [
      {
        element: withSuspense(AppLayout),
        children: [
          { index: true, element: <Navigate to="/dashboard" replace /> },
          { path: '/dashboard', element: withSuspense(DashboardPage) },
          {
            path: '/masters',
            children: [
              { index: true, element: <Navigate to="/masters/brands" replace /> },
              { path: 'products', element: withSuspense(ProductsPage) },
              { path: 'brands', element: withSuspense(BrandsPage) },
              { path: 'plants', element: withSuspense(PlantsPage) },
              { path: 'customers', element: withSuspense(CustomersPage) },
              { path: 'magazines', element: withSuspense(MagazinesPage) },
            ],
          },
          {
            path: '/barcode',
            children: [
              { index: true, element: <Navigate to="/barcode/generate" replace /> },
              { path: 'generate', element: withSuspense(BarcodeGeneratePage) },
              { path: 'search', element: withSuspense(BarcodeSearchPage) },
            ],
          },
          {
            path: '/production',
            children: [
              { index: true, element: <Navigate to="/production/plan" replace /> },
              { path: 'plan', element: withSuspense(ProductionPlanPage) },
            ],
          },
          {
            path: '/dispatch',
            children: [
              { index: true, element: withSuspense(DispatchPage) },
            ],
          },
          { path: '/reports', element: withSuspense(ReportsPage) },
        ],
      },
    ],
  },
  { path: '*', element: <Navigate to="/dashboard" replace /> },
]);

export const AppRouter = () => <RouterProvider router={router} />;
