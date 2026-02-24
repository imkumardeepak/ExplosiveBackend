import { z } from 'zod';

export const plantSchema = z.object({
  pName: z.string().min(1, 'Plant name is required'),
  pCode: z.string().min(1, 'Plant code is required').max(10, 'Plant code too long'),
  mfgLocationId: z.number().optional(),
});

export type PlantFormData = z.infer<typeof plantSchema>;
