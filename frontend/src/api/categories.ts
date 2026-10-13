import { api, unwrap } from './client'
import type { CategoryDto, ServiceResult } from './types'

export const categoriesApi = {
  getAll: () => unwrap(api.get<ServiceResult<CategoryDto[]>>('/categories')),
}
