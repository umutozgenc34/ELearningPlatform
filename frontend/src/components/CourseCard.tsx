import { Link } from 'react-router-dom'
import type { CourseDto } from '../api/types'
import { formatPrice, formatRating } from '../utils/format'
import { CourseCover } from './CourseCover'

export function CourseCard({ course }: { course: CourseDto }) {
  const details = course.courseDetails

  return (
    <article className="course-card">
      <Link to={`/kurs/${course.id}`} className="course-card__link">
        <CourseCover imageUrl={course.imageUrl} title={course.name} />
        <div className="course-card__body">
          {course.category && <span className="course-card__category">{course.category.name}</span>}
          <h3 className="course-card__title">{course.name}</h3>
          {details?.educatorFullName && <p className="course-card__educator">{details.educatorFullName}</p>}
          <div className="course-card__meta">
            <span className="course-card__price">{formatPrice(course.price)}</span>
            {details && details.rating > 0 && (
              <span className="course-card__rating" aria-label={`Puan ${formatRating(details.rating)} / 5`}>
                ★ {formatRating(details.rating)}
              </span>
            )}
          </div>
        </div>
      </Link>
    </article>
  )
}
