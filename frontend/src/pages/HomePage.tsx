import { useEffect, useMemo, useState } from 'react'
import { useSearchParams } from 'react-router-dom'
import { categoriesApi } from '../api/categories'
import { getErrorMessages } from '../api/client'
import { coursesApi } from '../api/courses'
import type { CategoryDto, CourseDto } from '../api/types'
import { useAuth } from '../auth/useAuth'
import { CourseCard } from '../components/CourseCard'
import { ErrorList } from '../components/ErrorList'

function useDebouncedValue<T>(value: T, delay: number) {
  const [debounced, setDebounced] = useState(value)
  useEffect(() => {
    const timer = setTimeout(() => setDebounced(value), delay)
    return () => clearTimeout(timer)
  }, [value, delay])
  return debounced
}

export function HomePage() {
  const { user } = useAuth()
  const [searchParams, setSearchParams] = useSearchParams()
  const categoryId = Number(searchParams.get('kategori')) || null
  const [keyword, setKeyword] = useState(searchParams.get('ara') ?? '')
  const debouncedKeyword = useDebouncedValue(keyword.trim(), 300)

  const [categories, setCategories] = useState<CategoryDto[]>([])
  const [courses, setCourses] = useState<CourseDto[] | null>(null)
  const [errors, setErrors] = useState<string[]>([])

  useEffect(() => {
    categoriesApi.getAll().then(setCategories, (error) => setErrors(getErrorMessages(error)))
  }, [])

  useEffect(() => {
    let cancelled = false
    const request = debouncedKeyword
      ? coursesApi.search(debouncedKeyword)
      : categoryId
        ? coursesApi.getByCategory(categoryId)
        : coursesApi.getAll()

    request.then(
      (result) => {
        if (cancelled) return
        setErrors([])
        setCourses(debouncedKeyword && categoryId ? result.filter((c) => c.category?.id === categoryId) : result)
      },
      (error) => {
        if (!cancelled) setErrors(getErrorMessages(error))
      },
    )
    return () => {
      cancelled = true
    }
  }, [categoryId, debouncedKeyword])

  useEffect(() => {
    setSearchParams(
      (current) => {
        const next = new URLSearchParams(current)
        if (debouncedKeyword) next.set('ara', debouncedKeyword)
        else next.delete('ara')
        return next
      },
      { replace: true },
    )
  }, [debouncedKeyword, setSearchParams])

  const selectCategory = (id: number | null) => {
    setSearchParams((current) => {
      const next = new URLSearchParams(current)
      if (id) next.set('kategori', String(id))
      else next.delete('kategori')
      return next
    })
  }

  const activeCategory = useMemo(() => categories.find((c) => c.id === categoryId), [categories, categoryId])

  const resultSummary = (() => {
    if (courses === null) return 'Kurslar yükleniyor…'
    const scope = activeCategory ? `${activeCategory.name} kategorisinde` : 'Tüm kategorilerde'
    if (courses.length === 0) return `${scope} ${debouncedKeyword ? `“${debouncedKeyword}” için ` : ''}kurs bulunamadı.`
    return `${scope} ${courses.length} kurs`
  })()

  return (
    <section className="home">
      <div className="home__hero">
        {user && <p className="home__greeting">Merhaba {user.userName}</p>}
        <h1 className="display display--xl">
          <span className="marker">Bugün ne öğrenmek</span> istersin?
        </h1>

        <label className="search">
          <span className="visually-hidden">Kurs ara</span>
          <input
            type="search"
            placeholder="Kurs adı veya konu ara, ör. Unity"
            value={keyword}
            onChange={(event) => setKeyword(event.target.value)}
          />
        </label>
      </div>

      <div className="catalog">
        <div className="filters" role="group" aria-label="Kategoriye göre filtrele">
          <button
            type="button"
            className="chip chip--button"
            aria-pressed={categoryId === null}
            onClick={() => selectCategory(null)}
          >
            Tümü
          </button>
          {categories.map((category) => (
            <button
              key={category.id}
              type="button"
              className="chip chip--button"
              aria-pressed={categoryId === category.id}
              onClick={() => selectCategory(category.id)}
            >
              {category.name}
            </button>
          ))}
        </div>

        <ErrorList messages={errors} />
        <p className="catalog__summary" aria-live="polite">
          {resultSummary}
        </p>

        {courses && courses.length > 0 && (
          <div className="course-grid">
            {courses.map((course) => (
              <CourseCard key={course.id} course={course} />
            ))}
          </div>
        )}
      </div>
    </section>
  )
}
