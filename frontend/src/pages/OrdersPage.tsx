import { useEffect, useState } from 'react'
import { Link, useLocation } from 'react-router-dom'
import { getErrorMessages } from '../api/client'
import { ordersApi } from '../api/orders'
import type { Order } from '../api/types'
import { ErrorList } from '../components/ErrorList'
import { OrderStatusBadge } from '../components/OrderStatusBadge'
import { formatDateTime, formatPrice } from '../utils/format'

export function OrdersPage() {
  const location = useLocation()
  const paymentMessage = (location.state as { message?: string } | null)?.message

  const [orders, setOrders] = useState<Order[] | null>(null)
  const [errors, setErrors] = useState<string[]>([])

  useEffect(() => {
    ordersApi.getMine().then(setOrders, (error) => setErrors(getErrorMessages(error)))
  }, [])

  return (
    <section className="orders">
      <h1 className="display">Siparişlerim</h1>

      {paymentMessage && <p className="notice orders__notice">{paymentMessage}</p>}
      <ErrorList messages={errors} />

      {orders === null && errors.length === 0 && <p className="page-status">Siparişler yükleniyor…</p>}

      {orders !== null && orders.length === 0 && (
        <div className="orders__empty">
          <p className="lede">Henüz bir siparişin yok.</p>
          <Link to="/" className="button button--primary">
            Kurslara göz at
          </Link>
        </div>
      )}

      {orders !== null && orders.length > 0 && (
        <ul className="order-list">
          {orders.map((order) => (
            <li key={order.id} className="order">
              <div className="order__head">
                <div>
                  <p className="order__date">{formatDateTime(order.created)}</p>
                  <OrderStatusBadge status={order.status} />
                </div>
                <p className="order__total">{formatPrice(order.totalPrice)}</p>
              </div>

              <ul className="order-lines">
                {order.orderItems.map((item) => (
                  <li key={item.courseId}>
                    {order.status === 'Confirmed' ? (
                      <Link to={`/kurs/${item.courseId}`}>{item.courseName}</Link>
                    ) : (
                      <span>{item.courseName}</span>
                    )}
                    <span>{formatPrice(item.price)}</span>
                  </li>
                ))}
              </ul>

              {order.status === 'Pending' && (
                <Link to={`/odeme/${order.id}`} className="button button--primary">
                  Ödemeyi tamamla
                </Link>
              )}
            </li>
          ))}
        </ul>
      )}
    </section>
  )
}
