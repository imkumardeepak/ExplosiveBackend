import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { customersService } from '@/api/services/customers.service';
import { customerSchema, type CustomerFormData } from '@/lib/schemas/customer.schema';
import { DataTable } from '@/components/ui/DataTable';
import { Modal } from '@/components/ui/Modal';
import { ConfirmDialog } from '@/components/ui/ConfirmDialog';
import { FormField } from '@/components/forms/FormField';
import { TextInput } from '@/components/forms/TextInput';
import { useUIStore } from '@/store/uiStore';
import type { CustomerMaster } from '@/types/domain.types';

const CustomersPage = () => {
  const queryClient = useQueryClient();
  const addToast = useUIStore((s) => s.addToast);
  const [modalOpen, setModalOpen] = useState(false);
  const [editingItem, setEditingItem] = useState<CustomerMaster | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<CustomerMaster | null>(null);

  const { data: customers = [], isLoading } = useQuery({
    queryKey: ['customers'],
    queryFn: () => customersService.getAll(),
  });

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<CustomerFormData>({
    resolver: zodResolver(customerSchema),
    defaultValues: { custName: '', custCode: '', address: '', licNo: '' },
  });

  const createMutation = useMutation({
    mutationFn: (data: CustomerFormData) => customersService.create(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['customers'] });
      addToast({ type: 'success', message: 'Customer created.' });
      closeModal();
    },
    onError: (err: { message: string }) => addToast({ type: 'error', message: err.message }),
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: number; data: CustomerFormData }) =>
      customersService.update(id, { ...data, id }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['customers'] });
      addToast({ type: 'success', message: 'Customer updated.' });
      closeModal();
    },
    onError: (err: { message: string }) => addToast({ type: 'error', message: err.message }),
  });

  const deleteMutation = useMutation({
    mutationFn: (id: number) => customersService.delete(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['customers'] });
      addToast({ type: 'success', message: 'Customer deleted.' });
      setDeleteTarget(null);
    },
    onError: (err: { message: string }) => addToast({ type: 'error', message: err.message }),
  });

  const openCreate = () => {
    setEditingItem(null);
    reset({ custName: '', custCode: '', address: '', licNo: '' });
    setModalOpen(true);
  };

  const openEdit = (item: CustomerMaster) => {
    setEditingItem(item);
    reset({ custName: item.custName, custCode: item.custCode ?? '', address: item.address ?? '', licNo: item.licNo ?? '' });
    setModalOpen(true);
  };

  const closeModal = () => {
    setModalOpen(false);
    setEditingItem(null);
  };

  const onSubmit = (data: CustomerFormData) => {
    if (editingItem) {
      updateMutation.mutate({ id: editingItem.id, data });
    } else {
      createMutation.mutate(data);
    }
  };

  const isSaving = createMutation.isPending || updateMutation.isPending;

  const columns = [
    { key: 'custName' as keyof CustomerMaster, header: 'Customer Name' },
    { key: 'custCode' as keyof CustomerMaster, header: 'Customer Code' },
    { key: 'address' as keyof CustomerMaster, header: 'Address' },
    { key: 'licNo' as keyof CustomerMaster, header: 'License No.' },
    {
      key: 'actions',
      header: 'Actions',
      render: (row: CustomerMaster) => (
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
        <h1 className="page-title">Customer Master</h1>
        <button className="btn btn--primary" onClick={openCreate}>
          + Add Customer
        </button>
      </div>

      <DataTable
        columns={columns}
        data={customers}
        isLoading={isLoading}
        keyField="id"
        emptyMessage="No customers found."
      />

      <Modal
        isOpen={modalOpen}
        onClose={closeModal}
        title={editingItem ? 'Edit Customer' : 'Add Customer'}
      >
        <form onSubmit={handleSubmit(onSubmit)} className="form-grid">
          <FormField label="Customer Name" required error={errors.custName?.message}>
            <TextInput {...register('custName')} placeholder="e.g. ABC Mining Ltd" error={!!errors.custName} />
          </FormField>
          <FormField label="Customer Code" error={errors.custCode?.message}>
            <TextInput {...register('custCode')} placeholder="e.g. CUST01" />
          </FormField>
          <FormField label="Address" error={errors.address?.message}>
            <TextInput {...register('address')} placeholder="Full address" />
          </FormField>
          <FormField label="License No." error={errors.licNo?.message}>
            <TextInput {...register('licNo')} placeholder="e.g. LIC-2026-001" />
          </FormField>
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
        message={`Are you sure you want to delete customer "${deleteTarget?.custName}"?`}
        confirmLabel="Delete"
        isLoading={deleteMutation.isPending}
      />
    </div>
  );
};

export default CustomersPage;
