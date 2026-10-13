const currency = new Intl.NumberFormat('tr-TR', { style: 'currency', currency: 'TRY' })
const decimal = new Intl.NumberFormat('tr-TR', { maximumFractionDigits: 1 })

export const formatPrice = (value: number) => currency.format(value)

export const formatRating = (value: number) => decimal.format(value)

const dateTime = new Intl.DateTimeFormat('tr-TR', { dateStyle: 'medium', timeStyle: 'short' })

export const formatDateTime = (value: string) => dateTime.format(new Date(value))
