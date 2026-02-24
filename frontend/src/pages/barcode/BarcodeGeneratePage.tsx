import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { barcodeService } from '@/api/services/barcode.service';
import { plantsService } from '@/api/services/plants.service';
import { brandsService } from '@/api/services/brands.service';
import { shiftsService } from '@/api/services/shifts.service';
import { barcodeGenerateSchema, type BarcodeGenerateFormData } from '@/lib/schemas/barcode.schema';
import { FormField } from '@/components/forms/FormField';
import { TextInput } from '@/components/forms/TextInput';
import { useUIStore } from '@/store/uiStore';

const BarcodeGeneratePage = () => {
  const addToast = useUIStore((s) => s.addToast);
  const queryClient = useQueryClient();

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

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<BarcodeGenerateFormData>({
    resolver: zodResolver(barcodeGenerateSchema),
    defaultValues: {
      country: 'India',
      countryCode: 'IN',
      mfgDt: new Date().toISOString().split('T')[0],
      l1NetUnit: 'KG',
      noOfL2: 5,
      noOfL3perL2: 10,
      noOfL3: 50,
      noOfbox: 1,
      noOfstickers: 1,
    },
  });

  const generateMutation = useMutation({
    mutationFn: (data: BarcodeGenerateFormData) => barcodeService.generate(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['barcodes'] });
      addToast({ type: 'success', message: 'Barcodes generated successfully!' });
      reset();
    },
    onError: (err: { message: string }) => addToast({ type: 'error', message: err.message }),
  });

  const onSubmit = (data: BarcodeGenerateFormData) => {
    generateMutation.mutate(data);
  };

  return (
    <div className="page">
      <div className="page-header">
        <h1 className="page-title">Generate L1 Barcodes</h1>
      </div>

      <div className="card">
        <form onSubmit={handleSubmit(onSubmit)} className="form-grid">
          <div className="form-section">
            <h3 className="form-section-title">Manufacturing Details</h3>
            <div className="form-row">
              <FormField label="Country" required error={errors.country?.message}>
                <TextInput {...register('country')} placeholder="e.g. India" error={!!errors.country} />
              </FormField>
              <FormField label="Country Code" required error={errors.countryCode?.message}>
                <TextInput {...register('countryCode')} placeholder="e.g. IN" error={!!errors.countryCode} />
              </FormField>
            </div>
            <div className="form-row">
              <FormField label="MFG Name" required error={errors.mfgName?.message}>
                <TextInput {...register('mfgName')} placeholder="e.g. ABC Explosives Ltd" error={!!errors.mfgName} />
              </FormField>
              <FormField label="MFG Location" required error={errors.mfgLoc?.message}>
                <TextInput {...register('mfgLoc')} placeholder="e.g. Nagpur" error={!!errors.mfgLoc} />
              </FormField>
              <FormField label="MFG Code" required error={errors.mfgCode?.message}>
                <TextInput {...register('mfgCode')} placeholder="e.g. ABC" error={!!errors.mfgCode} />
              </FormField>
            </div>
          </div>

          <div className="form-section">
            <h3 className="form-section-title">Plant & Shift</h3>
            <div className="form-row">
              <FormField label="Plant Name" required error={errors.plantName?.message}>
                <select className="text-input" {...register('plantName')}>
                  <option value="">Select plant...</option>
                  {plantNames.map((name) => (
                    <option key={name} value={name}>{name}</option>
                  ))}
                </select>
              </FormField>
              <FormField label="Plant Code" required error={errors.pCode?.message}>
                <TextInput {...register('pCode')} placeholder="e.g. PA" error={!!errors.pCode} />
              </FormField>
              <FormField label="Machine Code" required error={errors.mCode?.message}>
                <TextInput {...register('mCode')} placeholder="e.g. 1" error={!!errors.mCode} />
              </FormField>
              <FormField label="Shift" required error={errors.shift?.message}>
                <select className="text-input" {...register('shift')}>
                  <option value="">Select shift...</option>
                  {shifts.map((s) => (
                    <option key={s.id} value={s.shiftCode}>{s.shiftName}</option>
                  ))}
                </select>
              </FormField>
            </div>
          </div>

          <div className="form-section">
            <h3 className="form-section-title">Product Details</h3>
            <div className="form-row">
              <FormField label="Brand Name" required error={errors.brandName?.message}>
                <select className="text-input" {...register('brandName')}>
                  <option value="">Select brand...</option>
                  {brandNames.map((name) => (
                    <option key={name} value={name}>{name}</option>
                  ))}
                </select>
              </FormField>
              <FormField label="Brand ID" required error={errors.brandId?.message}>
                <TextInput {...register('brandId')} placeholder="e.g. EM01" error={!!errors.brandId} />
              </FormField>
              <FormField label="Product Size" required error={errors.productSize?.message}>
                <TextInput {...register('productSize')} placeholder="e.g. 25 KG" error={!!errors.productSize} />
              </FormField>
              <FormField label="Size Code" required error={errors.pSizeCode?.message}>
                <TextInput {...register('pSizeCode')} placeholder="e.g. 025" error={!!errors.pSizeCode} />
              </FormField>
            </div>
            <div className="form-row">
              <FormField label="Class" required error={errors.class?.message}>
                <TextInput {...register('class')} placeholder="e.g. 1" error={!!errors.class} />
              </FormField>
              <FormField label="Division" required error={errors.division?.message}>
                <TextInput {...register('division')} placeholder="e.g. 1.1" error={!!errors.division} />
              </FormField>
              <FormField label="SD Category" required error={errors.sdCat?.message}>
                <TextInput {...register('sdCat')} placeholder="e.g. SD1" error={!!errors.sdCat} />
              </FormField>
              <FormField label="UN No. Class" required error={errors.unNoClass?.message}>
                <TextInput {...register('unNoClass')} placeholder="e.g. UN0082" error={!!errors.unNoClass} />
              </FormField>
            </div>
          </div>

          <div className="form-section">
            <h3 className="form-section-title">Quantities</h3>
            <div className="form-row">
              <FormField label="Mfg. Date" required error={errors.mfgDt?.message}>
                <TextInput {...register('mfgDt')} type="date" error={!!errors.mfgDt} />
              </FormField>
              <FormField label="Net Weight" required error={errors.l1NetWt?.message}>
                <TextInput
                  {...register('l1NetWt', { valueAsNumber: true })}
                  type="number"
                  step="0.01"
                  placeholder="e.g. 25"
                  error={!!errors.l1NetWt}
                />
              </FormField>
              <FormField label="Net Unit" required error={errors.l1NetUnit?.message}>
                <TextInput {...register('l1NetUnit')} placeholder="e.g. KG" error={!!errors.l1NetUnit} />
              </FormField>
            </div>
            <div className="form-row">
              <FormField label="No. of L2" required error={errors.noOfL2?.message}>
                <TextInput
                  {...register('noOfL2', { valueAsNumber: true })}
                  type="number"
                  placeholder="e.g. 5"
                  error={!!errors.noOfL2}
                />
              </FormField>
              <FormField label="No. of L3 per L2" required error={errors.noOfL3perL2?.message}>
                <TextInput
                  {...register('noOfL3perL2', { valueAsNumber: true })}
                  type="number"
                  placeholder="e.g. 10"
                  error={!!errors.noOfL3perL2}
                />
              </FormField>
              <FormField label="Total L3" required error={errors.noOfL3?.message}>
                <TextInput
                  {...register('noOfL3', { valueAsNumber: true })}
                  type="number"
                  placeholder="e.g. 50"
                  error={!!errors.noOfL3}
                />
              </FormField>
              <FormField label="No. of Boxes" required error={errors.noOfbox?.message}>
                <TextInput
                  {...register('noOfbox', { valueAsNumber: true })}
                  type="number"
                  placeholder="e.g. 100"
                  error={!!errors.noOfbox}
                />
              </FormField>
              <FormField label="No. of Stickers" required error={errors.noOfstickers?.message}>
                <TextInput
                  {...register('noOfstickers', { valueAsNumber: true })}
                  type="number"
                  placeholder="e.g. 100"
                  error={!!errors.noOfstickers}
                />
              </FormField>
            </div>
          </div>

          <div className="form-actions">
            <button
              type="submit"
              className="btn btn--primary btn--lg"
              disabled={generateMutation.isPending}
            >
              {generateMutation.isPending ? 'Generating...' : 'Generate Barcodes'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default BarcodeGeneratePage;
