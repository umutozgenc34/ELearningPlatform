import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { getErrorMessages } from '../../api/client'
import { coursesApi } from '../../api/courses'
import { panelApi } from '../../api/panel'
import type { CourseDto } from '../../api/types'
import { CourseCover } from '../../components/CourseCover'
import { ErrorList } from '../../components/ErrorList'
import { formatPrice } from '../../utils/format'

export function PanelCoursesPage() {
  const [courses, setCourses] = useState<CourseDto[] | null>(null)
  const [errors, setErrors] = useState<string[]>([])
  const [deletingId, setDeletingId] = useState<string | null>(null)

  useEffect(() => {
    coursesApi.getAll().then(setCourses, (error) => setErrors(getErrorMessages(error)))
  }, [])

  const handleDelete = async (course: CourseDto) => {
    if (!window.confirm(`“${course.name}” kursu ve tüm dersleri silinsin mi? Bu işlem geri alınamaz.`)) return
    setErrors([])
    setDeletingId(course.id)
    try {
      await panelApi.deleteCourse(course.id)
      setCourses((current) => current?.filter((c) => c.id !== course.id) ?? null)
    } catch (error) {
      setErrors(getErrorMessages(error))
    } finally {
      setDeletingId(null)
    }
  }

  return (
    <div className="panel__section">
      <div className="panel__toolbar">
        <p className="catalog__summary">{courses ? `${courses.length} kurs` : 'Kurslar yükleniyor…'}</p>
        <Link to="/panel/kurs/yeni" className="button button--primary">
          Yeni kurs ekle
        </Link>
      </div>

      <ErrorList messages={errors} />

      {courses && courses.length === 0 && (
        <p className="page-status">Henüz kurs yok. İlk kursunu ekleyerek başla.</p>
      )}

      {courses && courses.length > 0 && (
        <ul className="panel-list">
          {courses.map((course) => (
            <li key={course.id} className="panel-list__item">
              <CourseCover imageUrl={course.imageUrl} title={course.name} size="thumb" />
              <div className="panel-list__main">
                <Link to={`/panel/kurs/${course.id}`} className="panel-list__name">
                  {course.name}
                </Link>
                <span className="panel-list__meta">
                  <span>{course.category?.name ?? 'Kategorisiz'}</span>
                  <span>{formatPrice(course.price)}</span>
                </span>
              </div>
              <div className="panel-list__actions">
                <Link to={`/panel/kurs/${course.id}`} className="button button--quiet">
                  Düzenle
                </Link>
                <button
                  type="button"
                  className="text-button text-button--danger"
                  onClick={() => handleDelete(course)}
                  disabled={deletingId === course.id}
                >
                  {deletingId === course.id ? 'Siliniyor…' : 'Sil'}
                </button>
              </div>
            </li>
          ))}
        </ul>
      )}
    </div>
  )
}
