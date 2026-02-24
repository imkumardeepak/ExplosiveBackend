import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { productsService } from '@/api';
import { useUIStore } from '@/store/uiStore';
import type { ApiError } from '@/types/api.types';

const PRODUCTS_KEY = ['products'] as const;

const ProductsPage = () => {
  const qc = useQueryClient();
  const addToast = useUIStore((s) => s.addToast);

  const { data: products, isLoading } = useQuery({
    queryKey: PRODUCTS_KEY,
    queryFn: productsService.getAll,
  });

  const deleteMutation = useMutation({
    mutationFn: (id: number) => productsService.delete(id),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: PRODUCTS_KEY });
      addToast({ type: 'success', message: 'Product deleted successfully' });
    },
    onError: (err: ApiError) =>
      addToast({ type: 'error', message: err.message }),
  });

  if (isLoading) return <div className="page-loading">Loading products...</div>;

  return (
    <div className="page">
      <div className="page__header">
        <h1 className="page-title">Products</h1>
      </div>
      <div className="table-wrapper">
        <table className="data-table">
          <thead>
            <tr>
              <th>Code</th>
              <th>Name</th>
              <th>Status</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {products?.map((product) => (
              <tr key={product.id}>
                <td>{product.productCode}</td>
                <td>{product.productName}</td>
                <td>
                  <span className={`badge badge--${product.isActive ? 'success' : 'danger'}`}>
                    {product.isActive ? 'Active' : 'Inactive'}
                  </span>
                </td>
                <td>
                  <button
                    className="btn btn--danger btn--sm"
                    onClick={() => deleteMutation.mutate(product.id)}
                    disabled={deleteMutation.isPending}
                  >
                    Delete
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
};

export default ProductsPage;
