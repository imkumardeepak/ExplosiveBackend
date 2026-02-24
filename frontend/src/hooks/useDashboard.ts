import { useQuery } from '@tanstack/react-query';
import { dashboardService } from '@/api';

export const DASHBOARD_KEYS = {
  cards: ['dashboard', 'cards'] as const,
  magazineStock: ['dashboard', 'magazineStock'] as const,
  dispatch: ['dashboard', 'dispatch'] as const,
  production: (plantName: string, timeRange: string) =>
    ['dashboard', 'production', plantName, timeRange] as const,
  slurry: (timeRange: string) => ['dashboard', 'slurry', timeRange] as const,
  detonator: (timeRange: string) => ['dashboard', 'detonator', timeRange] as const,
};

export const useDashboardCards = () =>
  useQuery({
    queryKey: DASHBOARD_KEYS.cards,
    queryFn: dashboardService.getDashboardCards,
  });

export const useMagazineStock = () =>
  useQuery({
    queryKey: DASHBOARD_KEYS.magazineStock,
    queryFn: dashboardService.getMagazineStock,
  });

export const useProductionDispatchData = (plantName: string, timeRange: string) =>
  useQuery({
    queryKey: DASHBOARD_KEYS.production(plantName, timeRange),
    queryFn: () => dashboardService.getProductionDispatchData(plantName, timeRange),
    enabled: !!plantName && !!timeRange,
  });

export const useSlurryData = (timeRange: string) =>
  useQuery({
    queryKey: DASHBOARD_KEYS.slurry(timeRange),
    queryFn: () => dashboardService.getSlurryData(timeRange),
    enabled: !!timeRange,
  });
