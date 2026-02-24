import { z } from 'zod';

export const productSchema = z.object({
  bid: z.string().min(1, 'Brand ID is required'),
  psize: z.string().min(1, 'Product size is required'),
  psizecod: z.string().min(1, 'Size code is required'),
  l1netwt: z.number({ invalid_type_error: 'Must be a number' }).positive('Must be positive'),
  l1netunit: z.string().min(1, 'Unit is required'),
  noofl2: z
    .number({ invalid_type_error: 'Must be a number' })
    .int()
    .positive('Must be positive'),
  noofl3perl2: z
    .number({ invalid_type_error: 'Must be a number' })
    .int()
    .positive('Must be positive'),
  noofl3perl1: z
    .number({ invalid_type_error: 'Must be a number' })
    .int()
    .positive('Must be positive'),
  bpl1: z.boolean(),
  bpl2: z.boolean(),
  bpl3: z.boolean(),
});

export type ProductFormData = z.infer<typeof productSchema>;
