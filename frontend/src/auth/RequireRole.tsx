import type { ReactNode } from 'react'
import { Link } from 'react-router-dom'
import { RequireAuth } from './RequireAuth'
import { useAuth } from './useAuth'

function RoleGate({ roles, children }: { roles: string[]; children: ReactNode }) {
  const { hasRole } = useAuth()

  if (!hasRole(...roles)) {
    return (
      <section className="not-found">
        <h1 className="display">Bu sayfa eğitmenler için</h1>
        <p className="lede">Kurs ve ders yönetimi sadece Eğitmen ve Admin hesaplarına açık.</p>
        <Link to="/" className="button button--primary">
          Kurslara dön
        </Link>
      </section>
    )
  }

  return <>{children}</>
}

export function RequireRole({ roles, children }: { roles: string[]; children: ReactNode }) {
  return (
    <RequireAuth>
      <RoleGate roles={roles}>{children}</RoleGate>
    </RequireAuth>
  )
}
