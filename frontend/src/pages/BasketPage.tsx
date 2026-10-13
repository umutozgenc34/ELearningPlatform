import { useState, type FormEvent } from 'react'
import { Link } from 'react-router-dom'
import { getErrorMessages } from '../api/client'
import { discountsApi } from '../api/discounts'
import type { DiscountDto } from '../api/types'
import { useBasket } from '../basket/useBasket'
import { CourseCover } from '../components/CourseCover'
import { ErrorList } from '../components/ErrorList'
import { formatPrice } from '../utils/format'

export function BasketPage() {
  const { basket, removeItem } = useBasket()
  const [errors, setErrors] = useState<string[]>([])
  const [removingId, setRemovingId] = useState<string | null>(null)

  const [couponInput, setCouponInput] = useState('')
  const [discount, setDiscount] = useState<DiscountDto | null>(null)
  const [couponErrors, setCouponErrors] = useState<string[]>([])
  const [checkingCoupon, setCheckingCoupon] = useState(false)

  if (!basket) {
    return <p className="page-status">Sepet yükleniyor…</p>
  }

  const handleRemove = async (courseId: string) => {
    setErrors([])
    setRemovingId(courseId)
    try {
      await removeItem(courseId)
    } catch (error) {
      setErrors(getErrorMessages(error))
    } finally {
      setRemovingId(null)
    }
  }

  const handleCoupon = async (event: FormEvent) => {
    event.preventDefault()
    const code = couponInput.trim()
    if (!code) return
    setCouponErrors([])
    setCheckingCoupon(true)
    try {
      setDiscount(await discountsApi.validate(code))
    } catch (error) {
      setDiscount(null)
      setCouponErrors(getErrorMessages(error))
    } finally {
      setCheckingCoupon(false)
    }
  }

  const subtotal = basket.totalPrice
  const discountAmount = discount ? Math.round(subtotal * discount.rate) / 100 : 0
  const total = subtotal - discountAmount

  if (basket.items.length === 0) {
    return (
      <section className="basket basket--empty">
        <h1 className="display">
          <span className="marker">Sepetin boş</span>
        </h1>
        <p className="lede">Bir kursa göz at, beğendiğini sepete ekle; burada seni bekliyor olacak.</p>
        <Link to="/" className="button button--primary">
          Kurslara göz at
        </Link>
      </section>
    )
  }

  return (
    <section className="basket">
      <h1 className="display">Sepet</h1>

      <div className="basket__layout">
        <div>
          <ErrorList messages={errors} />
          <ul className="basket-list">
            {basket.items.map((item) => (
              <li key={item.id} className="basket-list__item">
                <CourseCover imageUrl={item.imageUrl} title={item.name} size="thumb" />
                <Link to={`/kurs/${item.id}`} className="basket-list__name">
                  {item.name}
                </Link>
                <span className="basket-list__price">{formatPrice(item.price)}</span>
                <button
                  type="button"
                  className="text-button"
                  onClick={() => handleRemove(item.id)}
                  disabled={removingId === item.id}
                >
                  {removingId === item.id ? 'Kaldırılıyor…' : 'Kaldır'}
                </button>
              </li>
            ))}
          </ul>
        </div>

        <aside className="summary" aria-label="Sipariş özeti">
          <form className="coupon" onSubmit={handleCoupon}>
            <label className="field">
              <span className="field__label">Kupon kodu</span>
              <span className="coupon__row">
                <input
                  value={couponInput}
                  onChange={(event) => setCouponInput(event.target.value.toUpperCase())}
                  placeholder="ör. BABA10"
                  autoComplete="off"
                />
                <button type="submit" className="button button--quiet" disabled={checkingCoupon || !couponInput.trim()}>
                  {checkingCoupon ? 'Kontrol ediliyor…' : 'Uygula'}
                </button>
              </span>
            </label>
            <ErrorList messages={couponErrors} />
            {discount && (
              <p className="notice">
                {discount.coupon} kuponu uygulandı: %{discount.rate} indirim.{' '}
                <button
                  type="button"
                  className="text-button"
                  onClick={() => {
                    setDiscount(null)
                    setCouponInput('')
                  }}
                >
                  Kaldır
                </button>
              </p>
            )}
          </form>

          <dl className="summary__lines">
            <div>
              <dt>Ara toplam ({basket.items.length} kurs)</dt>
              <dd>{formatPrice(subtotal)}</dd>
            </div>
            {discount && (
              <div>
                <dt>İndirim</dt>
                <dd>−{formatPrice(discountAmount)}</dd>
              </div>
            )}
            <div className="summary__total">
              <dt>Toplam</dt>
              <dd>{formatPrice(total)}</dd>
            </div>
          </dl>

          <button type="button" className="button button--primary button--block" disabled>
            Siparişi tamamla
          </button>
          <p className="purchase__note">Ödeme adımı bir sonraki aşamada eklenecek.</p>
        </aside>
      </div>
    </section>
  )
}
