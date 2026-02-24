import { z } from 'zod';

export const customerSchema = z.object({
  custName: z.string().min(1, 'Customer name is required'),
  custCode: z.string().optional(),
  address: z.string().optional(),
  licNo: z.string().optional(),
});

export type CustomerFormData = z.infer<typeof customerSchema>;
