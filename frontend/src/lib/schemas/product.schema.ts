import { z } from 'zod';

export const productSchema = z.object({
  productName: z
    .string()
    .min(1, 'Product name is required')
    .max(200, 'Product name must be less than 200 characters'),
  productCode: z
    .string()
    .min(1, 'Product code is required')
    .max(50, 'Product code must be less than 50 characters'),
  brandId: z
    .number({ error: 'Brand is required' })
    .int()
    .positive('Brand is required'),
  isActive: z.boolean().default(true),
});

export type ProductFormValues = z.infer<typeof productSchema>;
