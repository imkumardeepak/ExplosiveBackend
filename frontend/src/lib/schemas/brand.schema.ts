import { z } from 'zod';

export const brandSchema = z.object({
  bname: z.string().min(1, 'Brand name is required'),
  bid: z.string().min(1, 'Brand ID is required').max(10, 'Brand ID too long'),
  bclass: z.string().min(1, 'Class is required'),
  bdivision: z.string().min(1, 'Division is required'),
  bsdcat: z.string().min(1, 'SD Category is required'),
  bunnoclass: z.string().min(1, 'UN No. Class is required'),
  bunit: z.string().min(1, 'Unit is required'),
  plantCode: z.string().min(1, 'Plant code is required'),
});

export type BrandFormData = z.infer<typeof brandSchema>;
