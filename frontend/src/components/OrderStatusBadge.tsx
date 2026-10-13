import type { OrderStatus } from '../api/types'

const labels: Record<OrderStatus, string> = {
  Pending: 'Ödeme bekliyor',
  Confirmed: 'Ödendi',
  Canceled: 'İptal edildi',
}

export function OrderStatusBadge({ status }: { status: OrderStatus }) {
  return <span className={`status status--${status.toLowerCase()}`}>{labels[status]}</span>
}
