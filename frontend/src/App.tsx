import { QueryClientProvider } from '@tanstack/react-query';
import { ReactQueryDevtools } from '@tanstack/react-query-devtools';
import { queryClient } from '@/lib/queryClient';
import { AppRouter } from '@/router';
import { ErrorBoundary } from '@/components/ui/ErrorBoundary';
import '@/styles/global.css';

const App = () => (
  <ErrorBoundary>
    <QueryClientProvider client={queryClient}>
      <AppRouter />
      {import.meta.env.DEV && <ReactQueryDevtools initialIsOpen={false} />}
    </QueryClientProvider>
  </ErrorBoundary>
);

export default App;
