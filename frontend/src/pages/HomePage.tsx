import { useEffect, useState } from 'react'
import { categoriesApi } from '../api/categories'
import { getErrorMessages } from '../api/client'
import type { CategoryDto } from '../api/types'
import { useAuth } from '../auth/useAuth'
import { ErrorList } from '../components/ErrorList'

export function HomePage() {
  const { user } = useAuth()
  const [categories, setCategories] = useState<CategoryDto[] | null>(null)
  const [errors, setErrors] = useState<string[]>([])

  useEffect(() => {
    let cancelled = false
    categoriesApi
      .getAll()
      .then((result) => {
        if (!cancelled) setCategories(result)
      })
      .catch((error) => {
        if (!cancelled) setErrors(getErrorMessages(error))
      })
    return () => {
      cancelled = true
    }
  }, [])

  return (
    <section className="home">
      <h1 className="display display--xl">
        <span className="marker">{user ? `Merhaba ${user.userName},` : 'Bugün'}</span>
        <br />
        ne öğrenmek istersin?
      </h1>

      <div className="home__categories">
        <h2 className="section-title">Kategoriler</h2>
        <ErrorList messages={errors} />

        {categories === null && errors.length === 0 && <p className="page-status">Kategoriler yükleniyor…</p>}

        {categories !== null && categories.length === 0 && (
          <p className="page-status">Henüz kategori yok. Eğitmenler ilk kategoriyi ekleyebilir.</p>
        )}

        {categories !== null && categories.length > 0 && (
          <ul className="chips">
            {categories.map((category) => (
              <li key={category.id} className="chip">
                {category.name}
              </li>
            ))}
          </ul>
        )}
      </div>
    </section>
  )
}
