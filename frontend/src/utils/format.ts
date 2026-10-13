const currency = new Intl.NumberFormat('tr-TR', { style: 'currency', currency: 'TRY' })
const decimal = new Intl.NumberFormat('tr-TR', { maximumFractionDigits: 1 })

export const formatPrice = (value: number) => currency.format(value)

export const formatRating = (value: number) => decimal.format(value)
