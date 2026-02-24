import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { productsService } from '@/api/services/products.service';
import { brandsService } from '@/api/services/brands.service';
import { productSchema, type ProductFormData } from '@/lib/schemas/product.schema';
import { DataTable } from '@/components/ui/DataTable';
import { Modal } from '@/components/ui/Modal';
import { ConfirmDialog } from '@/components/ui/ConfirmDialog';
import { FormField } from '@/components/forms/FormField';
import { TextInput } from '@/components/forms/TextInput';
import { useUIStore } from '@/store/uiStore';
import type { ProductMaster } from '@/types/domain.types';

const ProductsPage = () => {
  const queryClient = useQueryClient();
  const addToast = useUIStore((s) => s.addToast);
  const [modalOpen, setModalOpen] = useState(false);
  const [editingItem, setEditingItem] = useState<ProductMaster | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<ProductMaster | null>(null);

  const { data: products = [], isLoading } = useQuery({
    queryKey: ['products'],
    queryFn: () => productsService.getAll(),
  });

  const { data: brandNames = [] } = useQuery({
    queryKey: ['brands', 'names'],
    queryFn: () => brandsService.getAllNames(),
  });

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<ProductFormData>({
    resolver: zodResolver(productSchema),
    defaultValues: {
      bid: '',
      psize: '',
      psizecod: '',
      l1netwt: 0,
      l1netunit: 'KG',
      noofl2: 1,
      noofl3perl2: 1,
      noofl3perl1: 1,
      bpl1: true,
      bpl2: true,
      bpl3: false,
    },
  });

  const createMutation = useMutation({
    mutationFn: (data: ProductFormData) => productsService.create(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['products'] });
      addToast({ type: 'success', message: 'Product created.' });
      closeModal();
    },
    onError: (err: { message: string }) => addToast({ type: 'error', message: err.message }),
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: number; data: ProductFormData }) =>
      productsService.update(id, { ...data, id }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['products'] });
      addToast({ type: 'success', message: 'Product updated.' });
      closeModal();
    },
    onError: (err: { message: string }) => addToast({ type: 'error', message: err.message }),
  });

  const deleteMutation = useMutation({
    mutationFn: (id: number) => productsService.delete(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['products'] });
      addToast({ type: 'success', message: 'Product deleted.' });
      setDeleteTarget(null);
    },
    onError: (err: { message: string }) => addToast({ type: 'error', message: err.message }),
  });

  const openCreate = () => {
    setEditingItem(null);
    reset({ bid: '', psize: '', psizecod: '', l1netwt: 0, l1netunit: 'KG', noofl2: 1, noofl3perl2: 1, noofl3perl1: 1, bpl1: true, bpl2: true, bpl3: false });
    setModalOpen(true);
  };

  const openEdit = (item: ProductMaster) => {
    setEditingItem(item);
    reset({
      bid: item.bid,
      psize: item.psize,
      psizecod: item.psizecod,
      l1netwt: item.l1netwt,
      l1netunit: item.l1netunit,
      noofl2: item.noofl2,
      noofl3perl2: item.noofl3perl2,
      noofl3perl1: item.noofl3perl1,
      bpl1: item.bpl1,
      bpl2: item.bpl2,
      bpl3: item.bpl3,
    });
    setModalOpen(true);
  };

  const closeModal = () => {
    setModalOpen(false);
    setEditingItem(null);
  };

  const onSubmit = (data: ProductFormData) => {
    if (editingItem) {
      updateMutation.mutate({ id: editingItem.id, data });
    } else {
      createMutation.mutate(data);
    }
  };

  const isSaving = createMutation.isPending || updateMutation.isPending;

  const columns = [
    { key: 'bid' as keyof ProductMaster, header: 'Brand ID' },
    { key: 'psize' as keyof ProductMaster, header: 'Product Size' },
    { key: 'psizecod' as keyof ProductMaster, header: 'Size Code' },
    { key: 'l1netwt' as keyof ProductMaster, header: 'Net Wt.' },
    { key: 'l1netunit' as keyof ProductMaster, header: 'Unit' },
    { key: 'noofl2' as keyof ProductMaster, header: 'L2 Count' },
    { key: 'noofl3perl2' as keyof ProductMaster, header: 'L3/L2' },
    {
      key: 'actions',
      header: 'Actions',
      render: (row: ProductMaster) => (
        <div className="table-actions">
          <button className="btn btn--sm btn--secondary" onClick={() => openEdit(row)}>
            Edit
          </button>
          <button className="btn btn--sm btn--danger" onClick={() => setDeleteTarget(row)}>
            Delete
          </button>
        </div>
      ),
    },
  ];

  return (
    <div className="page">
      <div className="page-header">
        <h1 className="page-title">Product Master</h1>
        <button className="btn btn--primary" onClick={openCreate}>
          + Add Product
        </button>
      </div>

      <DataTable
        columns={columns}
        data={products}
        isLoading={isLoading}
        keyField="id"
        emptyMessage="No products found."
      />

      <Modal
        isOpen={modalOpen}
        onClose={closeModal}
        title={editingItem ? 'Edit Product' : 'Add Product'}
        size="lg"
      >
        <form onSubmit={handleSubmit(onSubmit)} className="form-grid">
          <FormField label="Brand ID" required error={errors.bid?.message}>
            <select className="text-input" {...register('bid')}>
              <option value="">Select brand...</option>
              {brandNames.map((name) => (
                <option key={name} value={name}>
                  {name}
                </option>
              ))}
            </select>
          </FormField>
          <FormField label="Product Size" required error={errors.psize?.message}>
            <TextInput {...register('psize')} placeholder="e.g. 25 KG" error={!!errors.psize} />
          </FormField>
          <FormField label="Size Code" required error={errors.psizecod?.message}>
            <TextInput {...register('psizecod')} placeholder="e.g. 025" error={!!errors.psizecod} />
          </FormField>
          <FormField label="Net Weight" required error={errors.l1netwt?.message}>
            <TextInput
              {...register('l1netwt', { valueAsNumber: true })}
              type="number"
              step="0.01"
              placeholder="e.g. 25"
              error={!!errors.l1netwt}
            />
          </FormField>
          <FormField label="Net Unit" required error={errors.l1netunit?.message}>
            <TextInput {...register('l1netunit')} placeholder="e.g. KG" error={!!errors.l1netunit} />
          </FormField>
          <FormField label="No. of L2" required error={errors.noofl2?.message}>
            <TextInput
              {...register('noofl2', { valueAsNumber: true })}
              type="number"
              placeholder="e.g. 5"
              error={!!errors.noofl2}
            />
          </FormField>
          <FormField label="No. of L3 per L2" required error={errors.noofl3perl2?.message}>
            <TextInput
              {...register('noofl3perl2', { valueAsNumber: true })}
              type="number"
              placeholder="e.g. 10"
              error={!!errors.noofl3perl2}
            />
          </FormField>
          <FormField label="No. of L3 per L1" required error={errors.noofl3perl1?.message}>
            <TextInput
              {...register('noofl3perl1', { valueAsNumber: true })}
              type="number"
              placeholder="e.g. 50"
              error={!!errors.noofl3perl1}
            />
          </FormField>
          <div className="form-grid form-grid--inline">
            <label className="checkbox-label">
              <input type="checkbox" {...register('bpl1')} /> Print L1
            </label>
            <label className="checkbox-label">
              <input type="checkbox" {...register('bpl2')} /> Print L2
            </label>
            <label className="checkbox-label">
              <input type="checkbox" {...register('bpl3')} /> Print L3
            </label>
          </div>
          <div className="form-actions">
            <button type="button" className="btn btn--secondary" onClick={closeModal}>
              Cancel
            </button>
            <button type="submit" className="btn btn--primary" disabled={isSaving}>
              {isSaving ? 'Saving...' : editingItem ? 'Update' : 'Create'}
            </button>
          </div>
        </form>
      </Modal>

      <ConfirmDialog
        isOpen={!!deleteTarget}
        onConfirm={() => deleteTarget && deleteMutation.mutate(deleteTarget.id)}
        onCancel={() => setDeleteTarget(null)}
        message={`Are you sure you want to delete product "${deleteTarget?.psize}"?`}
        confirmLabel="Delete"
        isLoading={deleteMutation.isPending}
      />
    </div>
  );
};

export default ProductsPage;
