import { useEffect, useState } from 'react'
import { Link, useLocation, useNavigate, useParams } from 'react-router-dom'
import { getErrorMessages } from '../api/client'
import { coursesApi } from '../api/courses'
import type { CourseDto, LessonDto } from '../api/types'
import { useAuth } from '../auth/useAuth'
import { useBasket } from '../basket/useBasket'
import { CourseCover } from '../components/CourseCover'
import { ErrorList } from '../components/ErrorList'
import { formatPrice, formatRating } from '../utils/format'

export function CourseDetailPage() {
  const { id = '' } = useParams()
  const { status } = useAuth()
  const { contains, addItem } = useBasket()
  const navigate = useNavigate()
  const location = useLocation()

  const [course, setCourse] = useState<CourseDto | null>(null)
  const [lessons, setLessons] = useState<LessonDto[]>([])
  const [loadErrors, setLoadErrors] = useState<string[]>([])
  const [actionErrors, setActionErrors] = useState<string[]>([])
  const [adding, setAdding] = useState(false)

  useEffect(() => {
    let cancelled = false
    Promise.all([coursesApi.getById(id), coursesApi.getLessons(id)]).then(
      ([courseResult, lessonResult]) => {
        if (cancelled) return
        setCourse(courseResult)
        setLessons(lessonResult)
      },
      (error) => {
        if (!cancelled) setLoadErrors(getErrorMessages(error))
      },
    )
    return () => {
      cancelled = true
    }
  }, [id])

  if (loadErrors.length > 0) {
    return (
      <section className="detail">
        <ErrorList messages={loadErrors} />
        <Link to="/">Kurs listesine dön</Link>
      </section>
    )
  }

  if (!course) {
    return <p className="page-status">Kurs yükleniyor…</p>
  }

  const inBasket = contains(course.id)
  const details = course.courseDetails

  const handleAdd = async () => {
    if (status !== 'authenticated') {
      navigate('/giris', { state: { from: location.pathname } })
      return
    }
    setActionErrors([])
    setAdding(true)
    try {
      await addItem(course.id)
    } catch (error) {
      setActionErrors(getErrorMessages(error))
    } finally {
      setAdding(false)
    }
  }

  return (
    <article className="detail">
      <Link to="/" className="back-link">
        Kurs listesi
      </Link>

      <div className="detail__layout">
        <div className="detail__main">
          {course.category && (
            <Link to={`/?kategori=${course.category.id}`} className="detail__category">
              {course.category.name}
            </Link>
          )}
          <h1 className="display">{course.name}</h1>
          <p className="lede">{course.description}</p>

          {details && (
            <dl className="facts">
              {details.educatorFullName && (
                <div>
                  <dt>Eğitmen</dt>
                  <dd>{details.educatorFullName}</dd>
                </div>
              )}
              {details.duration > 0 && (
                <div>
                  <dt>Süre</dt>
                  <dd>{details.duration} saat</dd>
                </div>
              )}
              {details.rating > 0 && (
                <div>
                  <dt>Puan</dt>
                  <dd>★ {formatRating(details.rating)} / 5</dd>
                </div>
              )}
              <div>
                <dt>Ders</dt>
                <dd>{lessons.length}</dd>
              </div>
            </dl>
          )}

          <section className="lessons" aria-labelledby="lessons-title">
            <h2 id="lessons-title" className="section-title">
              Müfredat
            </h2>
            {lessons.length === 0 ? (
              <p className="page-status">Bu kursa henüz ders eklenmemiş.</p>
            ) : (
              <ol className="lesson-list">
                {lessons.map((lesson) => (
                  <li key={lesson.id} className="lesson-list__item">
                    <span className="lesson-list__order">{lesson.lessonOrder}</span>
                    <span className="lesson-list__title">{lesson.title}</span>
                  </li>
                ))}
              </ol>
            )}
          </section>
        </div>

        <aside className="purchase" aria-label="Satın alma">
          <CourseCover imageUrl={course.imageUrl} title={course.name} size="hero" />
          <div className="purchase__body">
            <p className="purchase__price">{formatPrice(course.price)}</p>
            <ErrorList messages={actionErrors} />
            {inBasket ? (
              <Link to="/sepet" className="button button--quiet button--block">
                Sepette, sepete git
              </Link>
            ) : (
              <button type="button" className="button button--primary button--block" onClick={handleAdd} disabled={adding}>
                {adding ? 'Ekleniyor…' : 'Sepete ekle'}
              </button>
            )}
            {status !== 'authenticated' && (
              <p className="purchase__note">Sepete eklemek için giriş yapman gerekiyor.</p>
            )}
          </div>
        </aside>
      </div>
    </article>
  )
}
