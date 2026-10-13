import { useEffect, useState, type FormEvent } from 'react'
import { getErrorMessages } from '../../api/client'
import { panelApi } from '../../api/panel'
import type { DiscountDto } from '../../api/types'
import { ErrorList } from '../../components/ErrorList'

export function PanelCouponsPage() {
  const [discounts, setDiscounts] = useState<DiscountDto[] | null>(null)
  const [coupon, setCoupon] = useState('')
  const [rate, setRate] = useState('')
  const [errors, setErrors] = useState<string[]>([])
  const [busy, setBusy] = useState(false)

  const reload = () => panelApi.getDiscounts().then(setDiscounts, (error) => setErrors(getErrorMessages(error)))

  useEffect(() => {
    panelApi.getDiscounts().then(setDiscounts, (error) => setErrors(getErrorMessages(error)))
  }, [])

  const handleAdd = async (event: FormEvent) => {
    event.preventDefault()
    const code = coupon.trim()
    const rateValue = Number(rate)
    const clientErrors: string[] = []
    if (!code) clientErrors.push('Kupon kodunu yaz.')
    if (code.length > 12) clientErrors.push('Kupon kodu en fazla 12 karakter olabilir.')
    if (!rate || Number.isNaN(rateValue) || rateValue <= 0 || rateValue > 100) {
      clientErrors.push('İndirim oranı 1 ile 100 arasında olmalı.')
    }
    setErrors(clientErrors)
    if (clientErrors.length > 0) return

    setBusy(true)
    try {
      await panelApi.createDiscount(code, rateValue)
      setCoupon('')
      setRate('')
      await reload()
    } catch (error) {
      setErrors(getErrorMessages(error))
    } finally {
      setBusy(false)
    }
  }

  const handleDelete = async (code: string) => {
    if (!window.confirm(`${code} kuponu silinsin mi?`)) return
    setErrors([])
    try {
      await panelApi.deleteDiscount(code)
      await reload()
    } catch (error) {
      setErrors(getErrorMessages(error))
    }
  }

  return (
    <div className="panel__section panel__section--narrow">
      <form className="inline-form inline-form--add" onSubmit={handleAdd} noValidate>
        <label className="field">
          <span className="field__label">Kupon kodu</span>
          <input
            value={coupon}
            maxLength={12}
            placeholder="ör. YAZ25"
            onChange={(event) => setCoupon(event.target.value.toUpperCase().replace(/\s/g, ''))}
          />
        </label>
        <label className="field field--short">
          <span className="field__label">İndirim (%)</span>
          <input inputMode="numeric" value={rate} onChange={(event) => setRate(event.target.value.replace(/\D/g, ''))} />
        </label>
        <button type="submit" className="button button--primary" disabled={busy}>
          {busy ? 'Ekleniyor…' : 'Ekle'}
        </button>
      </form>
      <ErrorList messages={errors} />

      {discounts === null && errors.length === 0 && <p className="page-status">Kuponlar yükleniyor…</p>}
      {discounts && discounts.length === 0 && <p className="page-status">Henüz kupon yok.</p>}
      {discounts && discounts.length > 0 && (
        <ul className="panel-list">
          {discounts.map((discount) => (
            <li key={discount.coupon} className="panel-list__item panel-list__item--compact">
              <span className="panel-list__name coupon-code">{discount.coupon}</span>
              <span className="panel-list__meta">%{discount.rate} indirim</span>
              <span className="panel-list__actions">
                <button type="button" className="text-button text-button--danger" onClick={() => handleDelete(discount.coupon)}>
                  Sil
                </button>
              </span>
            </li>
          ))}
        </ul>
      )}
    </div>
  )
}
