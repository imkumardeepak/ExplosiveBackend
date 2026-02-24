import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { plantsService } from '@/api/services/plants.service';
import { plantSchema, type PlantFormData } from '@/lib/schemas/plant.schema';
import { DataTable } from '@/components/ui/DataTable';
import { Modal } from '@/components/ui/Modal';
import { ConfirmDialog } from '@/components/ui/ConfirmDialog';
import { FormField } from '@/components/forms/FormField';
import { TextInput } from '@/components/forms/TextInput';
import { useUIStore } from '@/store/uiStore';
import type { PlantMaster } from '@/types/domain.types';

const PlantsPage = () => {
  const queryClient = useQueryClient();
  const addToast = useUIStore((s) => s.addToast);
  const [modalOpen, setModalOpen] = useState(false);
  const [editingItem, setEditingItem] = useState<PlantMaster | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<PlantMaster | null>(null);

  const { data: plants = [], isLoading } = useQuery({
    queryKey: ['plants'],
    queryFn: () => plantsService.getAll(),
  });

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<PlantFormData>({
    resolver: zodResolver(plantSchema),
    defaultValues: { pName: '', pCode: '' },
  });

  const createMutation = useMutation({
    mutationFn: (data: PlantFormData) => plantsService.create(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['plants'] });
      addToast({ type: 'success', message: 'Plant created successfully.' });
      closeModal();
    },
    onError: (err: { message: string }) => addToast({ type: 'error', message: err.message }),
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: number; data: PlantFormData }) =>
      plantsService.update(id, { ...data, id }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['plants'] });
      addToast({ type: 'success', message: 'Plant updated successfully.' });
      closeModal();
    },
    onError: (err: { message: string }) => addToast({ type: 'error', message: err.message }),
  });

  const deleteMutation = useMutation({
    mutationFn: (id: number) => plantsService.delete(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['plants'] });
      addToast({ type: 'success', message: 'Plant deleted.' });
      setDeleteTarget(null);
    },
    onError: (err: { message: string }) => addToast({ type: 'error', message: err.message }),
  });

  const openCreate = () => {
    setEditingItem(null);
    reset({ pName: '', pCode: '' });
    setModalOpen(true);
  };

  const openEdit = (item: PlantMaster) => {
    setEditingItem(item);
    reset({ pName: item.pName, pCode: item.pCode, mfgLocationId: item.mfgLocationId });
    setModalOpen(true);
  };

  const closeModal = () => {
    setModalOpen(false);
    setEditingItem(null);
  };

  const onSubmit = (data: PlantFormData) => {
    if (editingItem) {
      updateMutation.mutate({ id: editingItem.id, data });
    } else {
      createMutation.mutate(data);
    }
  };

  const isSaving = createMutation.isPending || updateMutation.isPending;

  const columns = [
    { key: 'pName' as keyof PlantMaster, header: 'Plant Name' },
    { key: 'pCode' as keyof PlantMaster, header: 'Plant Code' },
    {
      key: 'actions',
      header: 'Actions',
      render: (row: PlantMaster) => (
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
        <h1 className="page-title">Plant Master</h1>
        <button className="btn btn--primary" onClick={openCreate}>
          + Add Plant
        </button>
      </div>

      <DataTable
        columns={columns}
        data={plants}
        isLoading={isLoading}
        keyField="id"
        emptyMessage="No plants found. Add your first plant."
      />

      <Modal
        isOpen={modalOpen}
        onClose={closeModal}
        title={editingItem ? 'Edit Plant' : 'Add Plant'}
      >
        <form onSubmit={handleSubmit(onSubmit)} className="form-grid">
          <FormField label="Plant Name" required error={errors.pName?.message}>
            <TextInput {...register('pName')} placeholder="e.g. Plant A" error={!!errors.pName} />
          </FormField>
          <FormField label="Plant Code" required error={errors.pCode?.message}>
            <TextInput {...register('pCode')} placeholder="e.g. PA" error={!!errors.pCode} />
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
        message={`Are you sure you want to delete plant "${deleteTarget?.pName}"?`}
        confirmLabel="Delete"
        isLoading={deleteMutation.isPending}
      />
    </div>
  );
};

export default PlantsPage;
