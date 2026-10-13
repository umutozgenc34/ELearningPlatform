import { api, unwrap } from './client'
import type { DiscountDto, ServiceResult } from './types'

export const discountsApi = {
  validate: (coupon: string) =>
    unwrap(api.get<ServiceResult<DiscountDto>>(`/discounts/validate/${encodeURIComponent(coupon)}`)),
}
