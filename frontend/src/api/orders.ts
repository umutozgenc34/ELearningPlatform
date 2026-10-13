import { api, unwrap } from './client'
import type { Order, PaymentRequest, ServiceResult } from './types'

export const ordersApi = {
  create: (coupon?: string) =>
    unwrap(api.post<ServiceResult<Order>>('/orders', null, { params: coupon ? { coupon } : undefined })),

  getById: (id: string) => unwrap(api.get<ServiceResult<Order>>(`/orders/${id}`)),

  getMine: () => unwrap(api.get<ServiceResult<Order[]>>('/orders/user')),

  pay: async (request: PaymentRequest) => {
    const response = await api.post<ServiceResult<null>>('/payments/process', request)
    return response.data.message ?? 'Ödeme başarılı.'
  },
}
