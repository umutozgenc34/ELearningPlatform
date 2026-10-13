import { useEffect, useState } from 'react'
import { ordersApi } from '../api/orders'
import { useAuth } from '../auth/useAuth'

export function useOwnedCourseIds() {
  const { status } = useAuth()
  const [ownedIds, setOwnedIds] = useState<Set<string>>(new Set())

  useEffect(() => {
    if (status !== 'authenticated') return
    let cancelled = false
    ordersApi.getMine().then(
      (orders) => {
        if (cancelled) return
        const ids = orders
          .filter((order) => order.status === 'Confirmed')
          .flatMap((order) => order.orderItems.map((item) => item.courseId))
        setOwnedIds(new Set(ids))
      },
      () => undefined,
    )
    return () => {
      cancelled = true
    }
  }, [status])

  return status === 'authenticated' ? ownedIds : new Set<string>()
}
