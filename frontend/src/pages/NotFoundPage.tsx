import { Link } from 'react-router-dom'

export function NotFoundPage() {
  return (
    <section className="not-found">
      <h1 className="display">
        <span className="marker">Bu sayfa</span> burada değil
      </h1>
      <p className="lede">Adres yanlış yazılmış ya da sayfa kaldırılmış olabilir.</p>
      <Link to="/" className="button button--primary">
        Ana sayfaya dön
      </Link>
    </section>
  )
}
