import { NavLink, Outlet } from 'react-router-dom'

export function PanelLayout() {
  return (
    <section className="panel">
      <h1 className="display">Eğitmen paneli</h1>
      <nav className="panel__tabs" aria-label="Panel bölümleri">
        <NavLink to="/panel" end>
          Kurslar
        </NavLink>
        <NavLink to="/panel/kategoriler">Kategoriler</NavLink>
        <NavLink to="/panel/kuponlar">Kuponlar</NavLink>
      </nav>
      <Outlet />
    </section>
  )
}
