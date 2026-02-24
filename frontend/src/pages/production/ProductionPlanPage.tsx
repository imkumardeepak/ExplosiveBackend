import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { productionService } from '@/api/services/production.service';
import { plantsService } from '@/api/services/plants.service';
import { brandsService } from '@/api/services/brands.service';
import { shiftsService } from '@/api/services/shifts.service';
import { DataTable } from '@/components/ui/DataTable';
import { Modal } from '@/components/ui/Modal';
import { ConfirmDialog } from '@/components/ui/ConfirmDialog';
import { FormField } from '@/components/forms/FormField';
import { TextInput } from '@/components/forms/TextInput';
import { useUIStore } from '@/store/uiStore';
import type { ProductionPlan } from '@/types/domain.types';

const ProductionPlanPage = () => {
  const queryClient = useQueryClient();
  const addToast = useUIStore((s) => s.addToast);
  const [modalOpen, setModalOpen] = useState(false);
  const [editingItem, setEditingItem] = useState<ProductionPlan | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<ProductionPlan | null>(null);
  const [form, setForm] = useState<Partial<ProductionPlan>>({
    mfgDt: new Date().toISOString().split('T')[0],
    noOfbox: 1,
    noOfstickers: 1,
  });

  const { data: plans = [], isLoading } = useQuery({
    queryKey: ['production-plans'],
    queryFn: () => productionService.getAllPlans(),
  });

  const { data: plantNames = [] } = useQuery({
    queryKey: ['plants', 'names'],
    queryFn: () => plantsService.getNames(),
  });

  const { data: brandNames = [] } = useQuery({
    queryKey: ['brands', 'names'],
    queryFn: () => brandsService.getAllNames(),
  });

  const { data: shifts = [] } = useQuery({
    queryKey: ['shifts'],
    queryFn: () => shiftsService.getAll(),
  });

  const createMutation = useMutation({
    mutationFn: (data: Omit<ProductionPlan, 'id'>) => productionService.createPlan(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['production-plans'] });
      addToast({ type: 'success', message: 'Production plan created.' });
      closeModal();
    },
    onError: (err: { message: string }) => addToast({ type: 'error', message: err.message }),
  });

  const deleteMutation = useMutation({
    mutationFn: (id: number) => productionService.deletePlan(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['production-plans'] });
      addToast({ type: 'success', message: 'Plan deleted.' });
      setDeleteTarget(null);
    },
    onError: (err: { message: string }) => addToast({ type: 'error', message: err.message }),
  });

  const openCreate = () => {
    setEditingItem(null);
    setForm({ mfgDt: new Date().toISOString().split('T')[0], noOfbox: 1, noOfstickers: 1 });
    setModalOpen(true);
  };

  const closeModal = () => {
    setModalOpen(false);
    setEditingItem(null);
  };

  const handleChange = (field: keyof ProductionPlan, value: string | number) => {
    setForm((prev) => ({ ...prev, [field]: value }));
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (!form.mfgDt || !form.plantCode || !form.brandId || !form.pSizeCode || !form.machineCode || !form.shift) {
      addToast({ type: 'error', message: 'Please fill all required fields.' });
      return;
    }
    createMutation.mutate(form as Omit<ProductionPlan, 'id'>);
  };

  const columns = [
    { key: 'mfgDt' as keyof ProductionPlan, header: 'Mfg. Date' },
    { key: 'plantCode' as keyof ProductionPlan, header: 'Plant' },
    { key: 'brandId' as keyof ProductionPlan, header: 'Brand ID' },
    { key: 'pSizeCode' as keyof ProductionPlan, header: 'Size Code' },
    { key: 'machineCode' as keyof ProductionPlan, header: 'Machine' },
    { key: 'shift' as keyof ProductionPlan, header: 'Shift' },
    { key: 'noOfbox' as keyof ProductionPlan, header: 'Boxes' },
    { key: 'noOfstickers' as keyof ProductionPlan, header: 'Stickers' },
    {
      key: 'actions',
      header: 'Actions',
      render: (row: ProductionPlan) => (
        <div className="table-actions">
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
        <h1 className="page-title">Production Plan</h1>
        <button className="btn btn--primary" onClick={openCreate}>
          + Create Plan
        </button>
      </div>

      <DataTable
        columns={columns}
        data={plans}
        isLoading={isLoading}
        keyField="id"
        emptyMessage="No production plans found."
      />

      <Modal isOpen={modalOpen} onClose={closeModal} title="Create Production Plan" size="lg">
        <form onSubmit={handleSubmit} className="form-grid">
          <div className="form-row">
            <FormField label="Mfg. Date" required>
              <TextInput
                type="date"
                value={form.mfgDt ?? ''}
                onChange={(e) => handleChange('mfgDt', e.target.value)}
              />
            </FormField>
            <FormField label="Plant Code" required>
              <select className="text-input" value={form.plantCode ?? ''} onChange={(e) => handleChange('plantCode', e.target.value)}>
                <option value="">Select plant...</option>
                {plantNames.map((name) => (
                  <option key={name} value={name}>{name}</option>
                ))}
              </select>
            </FormField>
          </div>
          <div className="form-row">
            <FormField label="Brand ID" required>
              <select className="text-input" value={form.brandId ?? ''} onChange={(e) => handleChange('brandId', e.target.value)}>
                <option value="">Select brand...</option>
                {brandNames.map((name) => (
                  <option key={name} value={name}>{name}</option>
                ))}
              </select>
            </FormField>
            <FormField label="Size Code" required>
              <TextInput
                value={form.pSizeCode ?? ''}
                onChange={(e) => handleChange('pSizeCode', e.target.value)}
                placeholder="e.g. 025"
              />
            </FormField>
          </div>
          <div className="form-row">
            <FormField label="Machine Code" required>
              <TextInput
                value={form.machineCode ?? ''}
                onChange={(e) => handleChange('machineCode', e.target.value)}
                placeholder="e.g. 1"
              />
            </FormField>
            <FormField label="Shift" required>
              <select className="text-input" value={form.shift ?? ''} onChange={(e) => handleChange('shift', e.target.value)}>
                <option value="">Select shift...</option>
                {shifts.map((s) => (
                  <option key={s.id} value={s.shiftCode}>{s.shiftName}</option>
                ))}
              </select>
            </FormField>
          </div>
          <div className="form-row">
            <FormField label="No. of Boxes" required>
              <TextInput
                type="number"
                value={form.noOfbox ?? 1}
                onChange={(e) => handleChange('noOfbox', Number(e.target.value))}
                placeholder="e.g. 100"
              />
            </FormField>
            <FormField label="No. of Stickers" required>
              <TextInput
                type="number"
                value={form.noOfstickers ?? 1}
                onChange={(e) => handleChange('noOfstickers', Number(e.target.value))}
                placeholder="e.g. 100"
              />
            </FormField>
          </div>
          <div className="form-actions">
            <button type="button" className="btn btn--secondary" onClick={closeModal}>
              Cancel
            </button>
            <button type="submit" className="btn btn--primary" disabled={createMutation.isPending}>
              {createMutation.isPending ? 'Creating...' : 'Create Plan'}
            </button>
          </div>
        </form>
      </Modal>

      <ConfirmDialog
        isOpen={!!deleteTarget}
        onConfirm={() => deleteTarget?.id && deleteMutation.mutate(deleteTarget.id)}
        onCancel={() => setDeleteTarget(null)}
        message="Are you sure you want to delete this production plan?"
        confirmLabel="Delete"
        isLoading={deleteMutation.isPending}
      />
    </div>
  );
};

export default ProductionPlanPage;
