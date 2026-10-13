import { useState, type FormEvent } from 'react'
import { Link, Navigate, useLocation, useNavigate } from 'react-router-dom'
import { getErrorMessages } from '../api/client'
import { useAuth } from '../auth/useAuth'
import { ErrorList } from '../components/ErrorList'

export function LoginPage() {
  const { login, status } = useAuth()
  const navigate = useNavigate()
  const location = useLocation()
  const redirectTo = (location.state as { from?: string; registered?: string } | null)?.from ?? '/'
  const registeredEmail = (location.state as { registered?: string } | null)?.registered

  const [email, setEmail] = useState(registeredEmail ?? '')
  const [password, setPassword] = useState('')
  const [errors, setErrors] = useState<string[]>([])
  const [submitting, setSubmitting] = useState(false)

  if (status === 'authenticated') {
    return <Navigate to={redirectTo} replace />
  }

  const handleSubmit = async (event: FormEvent) => {
    event.preventDefault()
    setErrors([])
    setSubmitting(true)
    try {
      await login({ email: email.trim(), password })
      navigate(redirectTo, { replace: true })
    } catch (error) {
      setErrors(getErrorMessages(error))
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <section className="auth">
      <div className="auth__intro">
        <h1 className="display">
          <span className="marker">Kaldığın yerden</span> devam et
        </h1>
        <p className="lede">Sepetin ve siparişlerin hesabına bağlı. Giriş yapınca hepsi seni bekliyor olacak.</p>
      </div>

      <form className="form" onSubmit={handleSubmit} noValidate>
        <h2 className="form__title">Giriş yap</h2>

        {registeredEmail && errors.length === 0 && (
          <p className="notice">Hesabın oluşturuldu. Şimdi şifrenle giriş yapabilirsin.</p>
        )}
        <ErrorList messages={errors} />

        <label className="field">
          <span className="field__label">E-posta</span>
          <input
            type="email"
            autoComplete="email"
            required
            value={email}
            onChange={(event) => setEmail(event.target.value)}
          />
        </label>

        <label className="field">
          <span className="field__label">Şifre</span>
          <input
            type="password"
            autoComplete="current-password"
            required
            value={password}
            onChange={(event) => setPassword(event.target.value)}
          />
        </label>

        <button type="submit" className="button button--primary button--block" disabled={submitting}>
          {submitting ? 'Giriş yapılıyor…' : 'Giriş yap'}
        </button>

        <p className="form__switch">
          Hesabın yok mu? <Link to="/kayit">Kayıt ol</Link>
        </p>
      </form>
    </section>
  )
}
