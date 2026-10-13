import { Link, NavLink, Outlet, useNavigate } from 'react-router-dom'
import { useAuth } from '../auth/useAuth'
import { useBasket } from '../basket/useBasket'

export function Layout() {
  const { user, status, logout } = useAuth()
  const { itemCount } = useBasket()
  const navigate = useNavigate()

  const handleLogout = () => {
    logout()
    navigate('/')
  }

  return (
    <div className="shell">
      <header className="topbar">
        <div className="topbar__inner">
          <Link to="/" className="wordmark" aria-label="ELearning ana sayfa">
            <span className="wordmark__mark" aria-hidden="true" />
            ELearning
          </Link>

          <nav className="topbar__nav" aria-label="Ana menü">
            <NavLink to="/" end>
              Keşfet
            </NavLink>
            {status === 'authenticated' && (
              <NavLink to="/sepet" aria-label={`Sepet, ${itemCount} kurs`}>
                Sepet
                {itemCount > 0 && <span className="count-badge">{itemCount}</span>}
              </NavLink>
            )}
          </nav>

          <div className="topbar__account">
            {status === 'authenticated' && user ? (
              <>
                <span className="topbar__user">
                  {user.userName}
                  {user.roles.length > 0 && <span className="role-tag">{user.roles.join(', ')}</span>}
                </span>
                <button type="button" className="button button--quiet" onClick={handleLogout}>
                  Çıkış yap
                </button>
              </>
            ) : status === 'anonymous' ? (
              <>
                <Link to="/giris" className="button button--quiet">
                  Giriş yap
                </Link>
                <Link to="/kayit" className="button button--primary">
                  Kayıt ol
                </Link>
              </>
            ) : null}
          </div>
        </div>
      </header>

      <main className="content">
        <Outlet />
      </main>
    </div>
  )
}
