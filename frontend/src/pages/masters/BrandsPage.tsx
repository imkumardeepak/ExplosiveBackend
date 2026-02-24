import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { brandsService } from '@/api/services/brands.service';
import { plantsService } from '@/api/services/plants.service';
import { brandSchema, type BrandFormData } from '@/lib/schemas/brand.schema';
import { DataTable } from '@/components/ui/DataTable';
import { Modal } from '@/components/ui/Modal';
import { ConfirmDialog } from '@/components/ui/ConfirmDialog';
import { FormField } from '@/components/forms/FormField';
import { TextInput } from '@/components/forms/TextInput';
import { useUIStore } from '@/store/uiStore';
import type { BrandMaster } from '@/types/domain.types';

const BrandsPage = () => {
  const queryClient = useQueryClient();
  const addToast = useUIStore((s) => s.addToast);
  const [modalOpen, setModalOpen] = useState(false);
  const [editingItem, setEditingItem] = useState<BrandMaster | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<BrandMaster | null>(null);

  const { data: brands = [], isLoading } = useQuery({
    queryKey: ['brands'],
    queryFn: () => brandsService.getAll(),
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
  } = useForm<BrandFormData>({
    resolver: zodResolver(brandSchema),
    defaultValues: { bname: '', bid: '', bclass: '', bdivision: '', bsdcat: '', bunnoclass: '', bunit: '', plantCode: '' },
  });

  const createMutation = useMutation({
    mutationFn: (data: BrandFormData) => brandsService.create(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['brands'] });
      addToast({ type: 'success', message: 'Brand created successfully.' });
      closeModal();
    },
    onError: (err: { message: string }) => {
      addToast({ type: 'error', message: err.message });
    },
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: number; data: BrandFormData }) =>
      brandsService.update(id, { ...data, id }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['brands'] });
      addToast({ type: 'success', message: 'Brand updated successfully.' });
      closeModal();
    },
    onError: (err: { message: string }) => {
      addToast({ type: 'error', message: err.message });
    },
  });

  const deleteMutation = useMutation({
    mutationFn: (id: number) => brandsService.delete(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['brands'] });
      addToast({ type: 'success', message: 'Brand deleted.' });
      setDeleteTarget(null);
    },
    onError: (err: { message: string }) => {
      addToast({ type: 'error', message: err.message });
    },
  });

  const openCreate = () => {
    setEditingItem(null);
    reset({ bname: '', bid: '', bclass: '', bdivision: '', bsdcat: '', bunnoclass: '', bunit: '', plantCode: '' });
    setModalOpen(true);
  };

  const openEdit = (item: BrandMaster) => {
    setEditingItem(item);
    reset({
      bname: item.bname,
      bid: item.bid,
      bclass: item.bclass,
      bdivision: item.bdivision,
      bsdcat: item.bsdcat,
      bunnoclass: item.bunnoclass,
      bunit: item.bunit,
      plantCode: item.plantCode,
    });
    setModalOpen(true);
  };

  const closeModal = () => {
    setModalOpen(false);
    setEditingItem(null);
  };

  const onSubmit = (data: BrandFormData) => {
    if (editingItem) {
      updateMutation.mutate({ id: editingItem.id, data });
    } else {
      createMutation.mutate(data);
    }
  };

  const isSaving = createMutation.isPending || updateMutation.isPending;

  const columns = [
    { key: 'bname' as keyof BrandMaster, header: 'Brand Name' },
    { key: 'bid' as keyof BrandMaster, header: 'Brand ID' },
    { key: 'bclass' as keyof BrandMaster, header: 'Class' },
    { key: 'bdivision' as keyof BrandMaster, header: 'Division' },
    { key: 'bsdcat' as keyof BrandMaster, header: 'SD Cat.' },
    { key: 'bunnoclass' as keyof BrandMaster, header: 'UN No.' },
    { key: 'bunit' as keyof BrandMaster, header: 'Unit' },
    { key: 'plantCode' as keyof BrandMaster, header: 'Plant Code' },
    {
      key: 'actions',
      header: 'Actions',
      render: (row: BrandMaster) => (
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
        <h1 className="page-title">Brand Master</h1>
        <button className="btn btn--primary" onClick={openCreate}>
          + Add Brand
        </button>
      </div>

      <DataTable
        columns={columns}
        data={brands}
        isLoading={isLoading}
        keyField="id"
        emptyMessage="No brands found. Add your first brand."
      />

      <Modal
        isOpen={modalOpen}
        onClose={closeModal}
        title={editingItem ? 'Edit Brand' : 'Add Brand'}
        size="lg"
      >
        <form onSubmit={handleSubmit(onSubmit)} className="form-grid">
          <FormField label="Brand Name" required error={errors.bname?.message}>
            <TextInput {...register('bname')} placeholder="e.g. Emulsion Explosive" error={!!errors.bname} />
          </FormField>
          <FormField label="Brand ID" required error={errors.bid?.message}>
            <TextInput {...register('bid')} placeholder="e.g. EM01" error={!!errors.bid} />
          </FormField>
          <FormField label="Class" required error={errors.bclass?.message}>
            <TextInput {...register('bclass')} placeholder="e.g. 1" error={!!errors.bclass} />
          </FormField>
          <FormField label="Division" required error={errors.bdivision?.message}>
            <TextInput {...register('bdivision')} placeholder="e.g. 1.1" error={!!errors.bdivision} />
          </FormField>
          <FormField label="SD Category" required error={errors.bsdcat?.message}>
            <TextInput {...register('bsdcat')} placeholder="e.g. SD1" error={!!errors.bsdcat} />
          </FormField>
          <FormField label="UN No. Class" required error={errors.bunnoclass?.message}>
            <TextInput {...register('bunnoclass')} placeholder="e.g. UN0082" error={!!errors.bunnoclass} />
          </FormField>
          <FormField label="Unit" required error={errors.bunit?.message}>
            <TextInput {...register('bunit')} placeholder="e.g. KG" error={!!errors.bunit} />
          </FormField>
          <FormField label="Plant Code" required error={errors.plantCode?.message}>
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
        message={`Are you sure you want to delete brand "${deleteTarget?.bname}"?`}
        confirmLabel="Delete"
        isLoading={deleteMutation.isPending}
      />
    </div>
  );
};

export default BrandsPage;
