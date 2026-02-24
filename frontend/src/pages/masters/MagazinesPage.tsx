import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { magazinesService } from '@/api/services/magazines.service';
import { magazineSchema, type MagazineFormData } from '@/lib/schemas/magazine.schema';
import { plantsService } from '@/api/services/plants.service';
import { DataTable } from '@/components/ui/DataTable';
import { Modal } from '@/components/ui/Modal';
import { ConfirmDialog } from '@/components/ui/ConfirmDialog';
import { FormField } from '@/components/forms/FormField';
import { TextInput } from '@/components/forms/TextInput';
import { useUIStore } from '@/store/uiStore';
import type { MagazineMaster } from '@/types/domain.types';

const MagazinesPage = () => {
  const queryClient = useQueryClient();
  const addToast = useUIStore((s) => s.addToast);
  const [modalOpen, setModalOpen] = useState(false);
  const [editingItem, setEditingItem] = useState<MagazineMaster | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<MagazineMaster | null>(null);

  const { data: magazines = [], isLoading } = useQuery({
    queryKey: ['magazines'],
    queryFn: () => magazinesService.getAll(),
  });

  const { data: plantNames = [] } = useQuery({
    queryKey: ['plants', 'names'],
    queryFn: () => plantsService.getNames(),
  });

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<MagazineFormData>({
    resolver: zodResolver(magazineSchema),
    defaultValues: { magname: '', mcode: '', licno: '', plantCode: '' },
  });

  const createMutation = useMutation({
    mutationFn: (data: MagazineFormData) => magazinesService.create(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['magazines'] });
      addToast({ type: 'success', message: 'Magazine created.' });
      closeModal();
    },
    onError: (err: { message: string }) => addToast({ type: 'error', message: err.message }),
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: number; data: MagazineFormData }) =>
      magazinesService.update(id, { ...data, id }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['magazines'] });
      addToast({ type: 'success', message: 'Magazine updated.' });
      closeModal();
    },
    onError: (err: { message: string }) => addToast({ type: 'error', message: err.message }),
  });

  const deleteMutation = useMutation({
    mutationFn: (id: number) => magazinesService.delete(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['magazines'] });
      addToast({ type: 'success', message: 'Magazine deleted.' });
      setDeleteTarget(null);
    },
    onError: (err: { message: string }) => addToast({ type: 'error', message: err.message }),
  });

  const openCreate = () => {
    setEditingItem(null);
    reset({ magname: '', mcode: '', licno: '', plantCode: '' });
    setModalOpen(true);
  };

  const openEdit = (item: MagazineMaster) => {
    setEditingItem(item);
    reset({ magname: item.magname, mcode: item.mcode, licno: item.licno ?? '', plantCode: item.plantCode ?? '' });
    setModalOpen(true);
  };

  const closeModal = () => {
    setModalOpen(false);
    setEditingItem(null);
  };

  const onSubmit = (data: MagazineFormData) => {
    if (editingItem) {
      updateMutation.mutate({ id: editingItem.id, data });
    } else {
      createMutation.mutate(data);
    }
  };

  const isSaving = createMutation.isPending || updateMutation.isPending;

  const columns = [
    { key: 'magname' as keyof MagazineMaster, header: 'Magazine Name' },
    { key: 'mcode' as keyof MagazineMaster, header: 'Code' },
    { key: 'licno' as keyof MagazineMaster, header: 'License No.' },
    { key: 'capacity' as keyof MagazineMaster, header: 'Capacity' },
    { key: 'plantCode' as keyof MagazineMaster, header: 'Plant Code' },
    {
      key: 'actions',
      header: 'Actions',
      render: (row: MagazineMaster) => (
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
        <h1 className="page-title">Magazine Master</h1>
        <button className="btn btn--primary" onClick={openCreate}>
          + Add Magazine
        </button>
      </div>

      <DataTable
        columns={columns}
        data={magazines}
        isLoading={isLoading}
        keyField="id"
        emptyMessage="No magazines found."
      />

      <Modal
        isOpen={modalOpen}
        onClose={closeModal}
        title={editingItem ? 'Edit Magazine' : 'Add Magazine'}
      >
        <form onSubmit={handleSubmit(onSubmit)} className="form-grid">
          <FormField label="Magazine Name" required error={errors.magname?.message}>
            <TextInput {...register('magname')} placeholder="e.g. MAG-01" error={!!errors.magname} />
          </FormField>
          <FormField label="Magazine Code" required error={errors.mcode?.message}>
            <TextInput {...register('mcode')} placeholder="e.g. M01" error={!!errors.mcode} />
          </FormField>
          <FormField label="License No." error={errors.licno?.message}>
            <TextInput {...register('licno')} placeholder="e.g. LIC-123" />
          </FormField>
          <FormField label="Capacity" error={errors.capacity?.message}>
            <TextInput
              {...register('capacity', { valueAsNumber: true })}
              type="number"
              placeholder="e.g. 1000"
              error={!!errors.capacity}
            />
          </FormField>
          <FormField label="Plant Code" error={errors.plantCode?.message}>
            <select className="text-input" {...register('plantCode')}>
              <option value="">Select plant...</option>
              {plantNames.map((name) => (
                <option key={name} value={name}>
                  {name}
                </option>
              ))}
            </select>
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
        message={`Are you sure you want to delete magazine "${deleteTarget?.magname}"?`}
        confirmLabel="Delete"
        isLoading={deleteMutation.isPending}
      />
    </div>
  );
};

export default MagazinesPage;
