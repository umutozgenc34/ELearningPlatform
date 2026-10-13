import { createContext } from 'react'
import type { BasketDto } from '../api/types'

export interface BasketContextValue {
  basket: BasketDto | null
  itemCount: number
  contains: (courseId: string) => boolean
  addItem: (courseId: string) => Promise<void>
  removeItem: (courseId: string) => Promise<void>
  refresh: () => Promise<void>
}

export const BasketContext = createContext<BasketContextValue | null>(null)
