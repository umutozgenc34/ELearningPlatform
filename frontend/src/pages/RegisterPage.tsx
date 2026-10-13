import { useState, type FormEvent } from 'react'
import { Link, Navigate, useNavigate } from 'react-router-dom'
import { authApi } from '../api/auth'
import { getErrorMessages } from '../api/client'
import type { RegisterRequest, RoleType } from '../api/types'
import { useAuth } from '../auth/useAuth'
import { ErrorList } from '../components/ErrorList'

const emptyForm: RegisterRequest = {
  firstName: '',
  lastName: '',
  userName: '',
  email: '',
  password: '',
  role: 'User',
}

function validate(form: RegisterRequest): string[] {
  const errors: string[] = []
  if (!form.firstName.trim()) errors.push('Adını yaz.')
  if (!form.lastName.trim()) errors.push('Soyadını yaz.')
  if (form.userName.trim().length < 3) errors.push('Kullanıcı adı en az 3 karakter olmalı.')
  if (!/^\S+@\S+\.\S+$/.test(form.email.trim())) errors.push('Geçerli bir e-posta adresi yaz.')
  if (form.password.length < 6) errors.push('Şifre en az 6 karakter olmalı.')
  if (!/[a-z]/.test(form.password) || !/[A-Z]/.test(form.password) || !/\d/.test(form.password)) {
    errors.push('Şifrede en az bir büyük harf, bir küçük harf ve bir rakam olmalı.')
  }
  return errors
}

export function RegisterPage() {
  const { status } = useAuth()
  const navigate = useNavigate()
  const [form, setForm] = useState<RegisterRequest>(emptyForm)
  const [errors, setErrors] = useState<string[]>([])
  const [submitting, setSubmitting] = useState(false)

  if (status === 'authenticated') {
    return <Navigate to="/" replace />
  }

  const update = (field: keyof RegisterRequest) => (value: string) =>
    setForm((current) => ({ ...current, [field]: value }))

  const handleSubmit = async (event: FormEvent) => {
    event.preventDefault()
    const clientErrors = validate(form)
    setErrors(clientErrors)
    if (clientErrors.length > 0) return

    setSubmitting(true)
    try {
      await authApi.register({
        ...form,
        firstName: form.firstName.trim(),
        lastName: form.lastName.trim(),
        userName: form.userName.trim(),
        email: form.email.trim(),
      })
      navigate('/giris', { state: { registered: form.email.trim() } })
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
          <span className="marker">Öğrenmeye başla,</span> ya da öğret
        </h1>
        <p className="lede">
          Öğrenci olarak kurs alabilir, eğitmen olarak kendi kurslarını ve derslerini yayınlayabilirsin.
        </p>
      </div>

      <form className="form" onSubmit={handleSubmit} noValidate>
        <h2 className="form__title">Kayıt ol</h2>
        <ErrorList messages={errors} />

        <fieldset className="role-choice">
          <legend className="field__label">Hesap türü</legend>
          {(
            [
              { value: 'User', title: 'Öğrenci', hint: 'Kurs al ve izle' },
              { value: 'Educator', title: 'Eğitmen', hint: 'Kurs ve ders yayınla' },
            ] as { value: RoleType; title: string; hint: string }[]
          ).map((option) => (
            <label key={option.value} className="role-choice__option">
              <input
                type="radio"
                name="role"
                value={option.value}
                checked={form.role === option.value}
                onChange={() => update('role')(option.value)}
              />
              <span>
                <strong>{option.title}</strong>
                <small>{option.hint}</small>
              </span>
            </label>
          ))}
        </fieldset>

        <div className="field-row">
          <label className="field">
            <span className="field__label">Ad</span>
            <input
              autoComplete="given-name"
              value={form.firstName}
              onChange={(event) => update('firstName')(event.target.value)}
            />
          </label>
          <label className="field">
            <span className="field__label">Soyad</span>
            <input
              autoComplete="family-name"
              value={form.lastName}
              onChange={(event) => update('lastName')(event.target.value)}
            />
          </label>
        </div>

        <label className="field">
          <span className="field__label">Kullanıcı adı</span>
          <input
            autoComplete="username"
            value={form.userName}
            onChange={(event) => update('userName')(event.target.value)}
          />
        </label>

        <label className="field">
          <span className="field__label">E-posta</span>
          <input
            type="email"
            autoComplete="email"
            value={form.email}
            onChange={(event) => update('email')(event.target.value)}
          />
        </label>

        <label className="field">
          <span className="field__label">Şifre</span>
          <input
            type="password"
            autoComplete="new-password"
            aria-describedby="password-hint"
            value={form.password}
            onChange={(event) => update('password')(event.target.value)}
          />
          <span id="password-hint" className="field__hint">
            En az 6 karakter; büyük harf, küçük harf ve rakam içermeli.
          </span>
        </label>

        <button type="submit" className="button button--primary button--block" disabled={submitting}>
          {submitting ? 'Hesap oluşturuluyor…' : 'Hesap oluştur'}
        </button>

        <p className="form__switch">
          Zaten hesabın var mı? <Link to="/giris">Giriş yap</Link>
        </p>
      </form>
    </section>
  )
}
