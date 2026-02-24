import { z } from 'zod';

export const magazineSchema = z.object({
  magname: z.string().min(1, 'Magazine name is required'),
  mcode: z.string().min(1, 'Magazine code is required'),
  licno: z.string().optional(),
  capacity: z.number().positive('Must be positive').optional(),
  plantCode: z.string().optional(),
});

export type MagazineFormData = z.infer<typeof magazineSchema>;
