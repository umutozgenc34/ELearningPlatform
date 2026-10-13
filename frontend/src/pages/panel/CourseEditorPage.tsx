import { useEffect, useState, type FormEvent } from 'react'
import { Link, useNavigate, useParams } from 'react-router-dom'
import { categoriesApi } from '../../api/categories'
import { getErrorMessages } from '../../api/client'
import { coursesApi } from '../../api/courses'
import { panelApi, type CourseFormValues } from '../../api/panel'
import type { CategoryDto, CourseDto } from '../../api/types'
import { ErrorList } from '../../components/ErrorList'
import { LessonManager } from './LessonManager'

const emptyValues: CourseFormValues = {
  name: '',
  description: '',
  price: '',
  categoryId: '',
  imageFile: null,
  educatorFullName: '',
  duration: '',
}

function toFormValues(course: CourseDto): CourseFormValues {
  return {
    name: course.name,
    description: course.description,
    price: String(course.price),
    categoryId: course.category ? String(course.category.id) : '',
    imageFile: null,
    educatorFullName: course.courseDetails?.educatorFullName ?? '',
    duration: course.courseDetails ? String(course.courseDetails.duration) : '',
  }
}

function validate(values: CourseFormValues): string[] {
  const errors: string[] = []
  if (!values.name.trim()) errors.push('Kurs adını yaz.')
  if (values.name.trim().length > 100) errors.push('Kurs adı en fazla 100 karakter olabilir.')
  if (!values.description.trim()) errors.push('Kurs açıklamasını yaz.')
  if (values.description.trim().length > 1000) errors.push('Açıklama en fazla 1000 karakter olabilir.')
  const price = Number(values.price.replace(',', '.'))
  if (!values.price || Number.isNaN(price) || price <= 0) errors.push('Fiyat 0’dan büyük bir sayı olmalı.')
  if (!values.categoryId) errors.push('Bir kategori seç.')
  return errors
}

export function CourseEditorPage() {
  const { id } = useParams()
  const isNew = !id
  const navigate = useNavigate()

  const [categories, setCategories] = useState<CategoryDto[]>([])
  const [course, setCourse] = useState<CourseDto | null>(null)
  const [values, setValues] = useState<CourseFormValues>(emptyValues)
  const [previewUrl, setPreviewUrl] = useState<string | null>(null)
  const [errors, setErrors] = useState<string[]>([])
  const [saved, setSaved] = useState(false)
  const [saving, setSaving] = useState(false)

  useEffect(() => {
    categoriesApi.getAll().then(setCategories, (error) => setErrors(getErrorMessages(error)))
  }, [])

  useEffect(() => {
    if (!id) return
    let cancelled = false
    coursesApi.getById(id).then(
      (result) => {
        if (cancelled) return
        setCourse(result)
        setValues(toFormValues(result))
      },
      (error) => {
        if (!cancelled) setErrors(getErrorMessages(error))
      },
    )
    return () => {
      cancelled = true
    }
  }, [id])

  useEffect(() => {
    return () => {
      if (previewUrl) URL.revokeObjectURL(previewUrl)
    }
  }, [previewUrl])

  const update = <K extends keyof CourseFormValues>(field: K, value: CourseFormValues[K]) => {
    setSaved(false)
    setValues((current) => ({ ...current, [field]: value }))
  }

  const selectImage = (file: File | null) => {
    setPreviewUrl(file ? URL.createObjectURL(file) : null)
    update('imageFile', file)
  }

  const handleSubmit = async (event: FormEvent) => {
    event.preventDefault()
    const clientErrors = validate(values)
    setErrors(clientErrors)
    if (clientErrors.length > 0) return

    setSaving(true)
    try {
      if (isNew) {
        const created = await panelApi.createCourse(values)
        navigate(`/panel/kurs/${created.id}`, { replace: true })
      } else {
        await panelApi.updateCourse(id, values)
        const refreshed = await coursesApi.getById(id)
        setCourse(refreshed)
        setValues(toFormValues(refreshed))
        setPreviewUrl(null)
        setSaved(true)
      }
    } catch (error) {
      setErrors(getErrorMessages(error))
    } finally {
      setSaving(false)
    }
  }

  if (!isNew && !course && errors.length === 0) {
    return <p className="page-status">Kurs yükleniyor…</p>
  }

  const currentImage = previewUrl ?? course?.imageUrl ?? null

  return (
    <div className="panel__section">
      <Link to="/panel" className="back-link">
        Kurslar
      </Link>

      <div className="editor">
        <form className="form editor__form" onSubmit={handleSubmit} noValidate>
          <h2 className="form__title">{isNew ? 'Yeni kurs' : 'Kurs bilgileri'}</h2>
          <ErrorList messages={errors} />
          {saved && <p className="notice">Değişiklikler kaydedildi.</p>}

          <label className="field">
            <span className="field__label">Kurs adı</span>
            <input value={values.name} maxLength={100} onChange={(event) => update('name', event.target.value)} />
          </label>

          <label className="field">
            <span className="field__label">Açıklama</span>
            <textarea
              rows={5}
              maxLength={1000}
              value={values.description}
              onChange={(event) => update('description', event.target.value)}
            />
          </label>

          <div className="field-row">
            <label className="field">
              <span className="field__label">Fiyat (₺)</span>
              <input
                inputMode="decimal"
                placeholder="ör. 149,90"
                value={values.price}
                onChange={(event) => update('price', event.target.value)}
              />
            </label>
            <label className="field">
              <span className="field__label">Kategori</span>
              <select value={values.categoryId} onChange={(event) => update('categoryId', event.target.value)}>
                <option value="">Seç</option>
                {categories.map((category) => (
                  <option key={category.id} value={category.id}>
                    {category.name}
                  </option>
                ))}
              </select>
            </label>
          </div>

          {isNew && (
            <div className="field-row">
              <label className="field">
                <span className="field__label">Eğitmen adı</span>
                <input
                  value={values.educatorFullName}
                  maxLength={100}
                  onChange={(event) => update('educatorFullName', event.target.value)}
                />
              </label>
              <label className="field">
                <span className="field__label">Süre (saat)</span>
                <input
                  inputMode="numeric"
                  value={values.duration}
                  onChange={(event) => update('duration', event.target.value.replace(/\D/g, ''))}
                />
              </label>
            </div>
          )}

          <div className="field">
            <span className="field__label">Kapak resmi</span>
            <div className="image-picker">
              {currentImage ? (
                <img src={currentImage} alt="" className="image-picker__preview" />
              ) : (
                <div className="image-picker__empty">Resim yok</div>
              )}
              <label className="button button--quiet image-picker__button">
                {currentImage ? 'Resmi değiştir' : 'Resim seç'}
                <input
                  type="file"
                  accept="image/*"
                  className="visually-hidden"
                  onChange={(event) => selectImage(event.target.files?.[0] ?? null)}
                />
              </label>
            </div>
            <span className="field__hint">Resim Cloudinary’ye yüklenir. Seçmezsen mevcut resim korunur.</span>
          </div>

          <button type="submit" className="button button--primary button--block" disabled={saving}>
            {saving ? 'Kaydediliyor…' : isNew ? 'Kursu oluştur' : 'Değişiklikleri kaydet'}
          </button>
          {isNew && <p className="purchase__note">Kursu oluşturduktan sonra ders ekleyebilirsin.</p>}
        </form>

        {!isNew && course && (
          <div className="editor__lessons">
            <LessonManager courseId={course.id} />
            <Link to={`/kurs/${course.id}`} className="editor__preview-link">
              Kurs sayfasını görüntüle
            </Link>
          </div>
        )}
      </div>
    </div>
  )
}
