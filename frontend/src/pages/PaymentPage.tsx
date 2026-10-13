import { useEffect, useState, type FormEvent } from 'react'
import { Link, useNavigate, useParams } from 'react-router-dom'
import { getErrorMessages } from '../api/client'
import { ordersApi } from '../api/orders'
import type { Order } from '../api/types'
import { ErrorList } from '../components/ErrorList'
import { OrderStatusBadge } from '../components/OrderStatusBadge'
import { formatPrice } from '../utils/format'

const digitsOnly = (value: string) => value.replace(/\D/g, '')

function formatCardNumber(value: string) {
  return digitsOnly(value).slice(0, 16).replace(/(\d{4})(?=\d)/g, '$1 ')
}

function formatExpiry(value: string) {
  const digits = digitsOnly(value).slice(0, 4)
  return digits.length > 2 ? `${digits.slice(0, 2)}/${digits.slice(2)}` : digits
}

function validate(holder: string, cardNumber: string, expiry: string, cvv: string): string[] {
  const errors: string[] = []
  if (!holder.trim()) errors.push('Kart üzerindeki ismi yaz.')
  if (holder.trim().length > 30) errors.push('Kart sahibinin adı en fazla 30 karakter olabilir.')
  if (digitsOnly(cardNumber).length !== 16) errors.push('Kart numarası 16 haneli olmalı.')

  const match = /^(\d{2})\/(\d{2})$/.exec(expiry)
  if (!match || Number(match[1]) < 1 || Number(match[1]) > 12) {
    errors.push('Son kullanma tarihini AA/YY biçiminde yaz.')
  }

  if (!/^\d{3}$/.test(cvv)) errors.push('CVV 3 haneli olmalı.')
  return errors
}

export function PaymentPage() {
  const { orderId = '' } = useParams()
  const navigate = useNavigate()

  const [order, setOrder] = useState<Order | null>(null)
  const [loadErrors, setLoadErrors] = useState<string[]>([])

  const [holder, setHolder] = useState('')
  const [cardNumber, setCardNumber] = useState('')
  const [expiry, setExpiry] = useState('')
  const [cvv, setCvv] = useState('')
  const [errors, setErrors] = useState<string[]>([])
  const [paying, setPaying] = useState(false)

  useEffect(() => {
    let cancelled = false
    ordersApi.getById(orderId).then(
      (result) => {
        if (!cancelled) setOrder(result)
      },
      (error) => {
        if (!cancelled) setLoadErrors(getErrorMessages(error))
      },
    )
    return () => {
      cancelled = true
    }
  }, [orderId])

  if (loadErrors.length > 0) {
    return (
      <section className="payment">
        <ErrorList messages={loadErrors} />
        <Link to="/siparisler">Siparişlerime dön</Link>
      </section>
    )
  }

  if (!order) {
    return <p className="page-status">Sipariş yükleniyor…</p>
  }

  const handleSubmit = async (event: FormEvent) => {
    event.preventDefault()
    const clientErrors = validate(holder, cardNumber, expiry, cvv)
    setErrors(clientErrors)
    if (clientErrors.length > 0) return

    setPaying(true)
    try {
      const message = await ordersApi.pay({
        orderId: order.id,
        cardHolderName: holder.trim(),
        cardNumber: digitsOnly(cardNumber),
        expiryDate: expiry,
        cvv,
      })
      navigate('/siparisler', { state: { paidOrderId: order.id, message } })
    } catch (error) {
      setErrors(getErrorMessages(error))
    } finally {
      setPaying(false)
    }
  }

  return (
    <section className="payment">
      <h1 className="display">Ödeme</h1>

      <div className="payment__layout">
        <form className="form" onSubmit={handleSubmit} noValidate>
          <h2 className="form__title">Kart bilgileri</h2>

          {order.status !== 'Pending' ? (
            <>
              <p className="notice">
                Bu sipariş için ödeme alınamaz: <OrderStatusBadge status={order.status} />
              </p>
              <Link to="/siparisler" className="button button--quiet">
                Siparişlerime git
              </Link>
            </>
          ) : (
            <>
              <ErrorList messages={errors} />

              <label className="field">
                <span className="field__label">Kart üzerindeki isim</span>
                <input
                  autoComplete="cc-name"
                  maxLength={30}
                  value={holder}
                  onChange={(event) => setHolder(event.target.value)}
                />
              </label>

              <label className="field">
                <span className="field__label">Kart numarası</span>
                <input
                  inputMode="numeric"
                  autoComplete="cc-number"
                  placeholder="0000 0000 0000 0000"
                  value={cardNumber}
                  onChange={(event) => setCardNumber(formatCardNumber(event.target.value))}
                />
              </label>

              <div className="field-row">
                <label className="field">
                  <span className="field__label">Son kullanma</span>
                  <input
                    inputMode="numeric"
                    autoComplete="cc-exp"
                    placeholder="AA/YY"
                    value={expiry}
                    onChange={(event) => setExpiry(formatExpiry(event.target.value))}
                  />
                </label>
                <label className="field">
                  <span className="field__label">CVV</span>
                  <input
                    inputMode="numeric"
                    autoComplete="cc-csc"
                    placeholder="000"
                    value={cvv}
                    onChange={(event) => setCvv(digitsOnly(event.target.value).slice(0, 3))}
                  />
                </label>
              </div>

              <button type="submit" className="button button--primary button--block" disabled={paying}>
                {paying ? 'Ödeme alınıyor…' : `${formatPrice(order.totalPrice)} öde`}
              </button>

              <p className="purchase__note">
                Bu bir demo ödeme ekranıdır, gerçek bir kart kullanma. 4000 0000 0000 0002 numaralı test kartı
                reddedilir, diğer 16 haneli numaralar onaylanır.
              </p>
            </>
          )}
        </form>

        <aside className="summary" aria-label="Sipariş özeti">
          <h2 className="section-title">Sipariş özeti</h2>
          <ul className="order-lines">
            {order.orderItems.map((item) => (
              <li key={item.courseId}>
                <span>{item.courseName}</span>
                <span>{formatPrice(item.price)}</span>
              </li>
            ))}
          </ul>
          <dl className="summary__lines">
            <div className="summary__total">
              <dt>Ödenecek tutar</dt>
              <dd>{formatPrice(order.totalPrice)}</dd>
            </div>
          </dl>
        </aside>
      </div>
    </section>
  )
}
