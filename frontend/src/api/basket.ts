import { api, unwrap } from './client'
import type { BasketDto, ServiceResult } from './types'

export const basketApi = {
  get: () => unwrap(api.get<ServiceResult<BasketDto>>('/baskets/user')),

  addItem: (courseId: string) => api.post('/baskets/item', { courseId }),

  removeItem: (courseId: string) => api.delete(`/baskets/item/${courseId}`),
}
