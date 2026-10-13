import { useCallback, useEffect, useMemo, useState, type ReactNode } from 'react'
import { basketApi } from '../api/basket'
import type { BasketDto } from '../api/types'
import { useAuth } from '../auth/useAuth'
import { BasketContext } from './basketContext'

export function BasketProvider({ children }: { children: ReactNode }) {
  const { status } = useAuth()
  const [basket, setBasket] = useState<BasketDto | null>(null)

  const refresh = useCallback(() => basketApi.get().then(setBasket), [])

  useEffect(() => {
    if (status !== 'authenticated') return
    let cancelled = false
    basketApi.get().then(
      (result) => {
        if (!cancelled) setBasket(result)
      },
      () => {
        if (!cancelled) setBasket(null)
      },
    )
    return () => {
      cancelled = true
    }
  }, [status])

  const addItem = useCallback(
    async (courseId: string) => {
      await basketApi.addItem(courseId)
      await refresh()
    },
    [refresh],
  )

  const removeItem = useCallback(
    async (courseId: string) => {
      await basketApi.removeItem(courseId)
      await refresh()
    },
    [refresh],
  )

  const visibleBasket = status === 'authenticated' ? basket : null

  const value = useMemo(
    () => ({
      basket: visibleBasket,
      itemCount: visibleBasket?.items.length ?? 0,
      contains: (courseId: string) => visibleBasket?.items.some((item) => item.id === courseId) ?? false,
      addItem,
      removeItem,
      refresh,
    }),
    [visibleBasket, addItem, removeItem, refresh],
  )

  return <BasketContext.Provider value={value}>{children}</BasketContext.Provider>
}
