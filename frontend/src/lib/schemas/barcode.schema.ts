import { z } from 'zod';

export const barcodeGenerateSchema = z.object({
  country: z.string().min(1, 'Country is required'),
  countryCode: z.string().min(1, 'Country code is required'),
  mfgName: z.string().min(1, 'MFG name is required'),
  mfgLoc: z.string().min(1, 'MFG location is required'),
  mfgCode: z.string().min(1, 'MFG code is required'),
  plantName: z.string().min(1, 'Plant name is required'),
  pCode: z.string().min(1, 'Plant code is required'),
  mCode: z.string().min(1, 'Machine code is required'),
  shift: z.string().min(1, 'Shift is required'),
  brandName: z.string().min(1, 'Brand name is required'),
  brandId: z.string().min(1, 'Brand ID is required'),
  productSize: z.string().min(1, 'Product size is required'),
  pSizeCode: z.string().min(1, 'Size code is required'),
  class: z.string().min(1, 'Class is required'),
  division: z.string().min(1, 'Division is required'),
  sdCat: z.string().min(1, 'SD category is required'),
  unNoClass: z.string().min(1, 'UN No. Class is required'),
  mfgDt: z.string().min(1, 'Manufacturing date is required'),
  l1NetWt: z.number({ invalid_type_error: 'Must be a number' }).positive('Must be positive'),
  l1NetUnit: z.string().min(1, 'Net unit is required'),
  noOfL2: z
    .number({ invalid_type_error: 'Must be a number' })
    .int()
    .positive('Must be positive'),
  noOfL3perL2: z
    .number({ invalid_type_error: 'Must be a number' })
    .int()
    .positive('Must be positive'),
  noOfL3: z.number({ invalid_type_error: 'Must be a number' }).int().positive('Must be positive'),
  noOfbox: z
    .number({ invalid_type_error: 'Must be a number' })
    .int()
    .positive('Must be positive'),
  noOfstickers: z
    .number({ invalid_type_error: 'Must be a number' })
    .int()
    .positive('Must be positive'),
});

export type BarcodeGenerateFormData = z.infer<typeof barcodeGenerateSchema>;
