import { useCallback, useEffect, useState, type FormEvent } from 'react'
import { getErrorMessages } from '../../api/client'
import { coursesApi } from '../../api/courses'
import { panelApi } from '../../api/panel'
import type { LessonDto } from '../../api/types'
import { ErrorList } from '../../components/ErrorList'

function LessonRow({
  lesson,
  onSaved,
  onDeleted,
}: {
  lesson: LessonDto
  onSaved: () => void
  onDeleted: () => void
}) {
  const [editing, setEditing] = useState(false)
  const [title, setTitle] = useState(lesson.title)
  const [order, setOrder] = useState(String(lesson.lessonOrder))
  const [errors, setErrors] = useState<string[]>([])
  const [busy, setBusy] = useState(false)

  const save = async (event: FormEvent) => {
    event.preventDefault()
    const lessonOrder = Number(order)
    if (!title.trim() || !Number.isInteger(lessonOrder) || lessonOrder < 1) {
      setErrors(['Başlık boş olamaz, sıra 1 veya daha büyük bir tam sayı olmalı.'])
      return
    }
    setBusy(true)
    setErrors([])
    try {
      await panelApi.updateLesson({
        id: lesson.id,
        courseId: lesson.courseId,
        title: title.trim(),
        videoUrl: lesson.videoUrl,
        lessonOrder,
      })
      setEditing(false)
      onSaved()
    } catch (error) {
      setErrors(getErrorMessages(error))
    } finally {
      setBusy(false)
    }
  }

  const remove = async () => {
    if (!window.confirm(`“${lesson.title}” dersi silinsin mi?`)) return
    setBusy(true)
    try {
      await panelApi.deleteLesson(lesson.id)
      onDeleted()
    } catch (error) {
      setErrors(getErrorMessages(error))
      setBusy(false)
    }
  }

  if (editing) {
    return (
      <li className="lesson-row lesson-row--editing">
        <form className="lesson-row__form" onSubmit={save}>
          <input
            className="lesson-row__order-input"
            inputMode="numeric"
            aria-label="Sıra"
            value={order}
            onChange={(event) => setOrder(event.target.value.replace(/\D/g, ''))}
          />
          <input aria-label="Ders başlığı" value={title} maxLength={200} onChange={(event) => setTitle(event.target.value)} />
          <button type="submit" className="button button--primary" disabled={busy}>
            Kaydet
          </button>
          <button
            type="button"
            className="text-button"
            onClick={() => {
              setEditing(false)
              setTitle(lesson.title)
              setOrder(String(lesson.lessonOrder))
              setErrors([])
            }}
          >
            Vazgeç
          </button>
        </form>
        <ErrorList messages={errors} />
      </li>
    )
  }

  return (
    <li className="lesson-row">
      <span className="lesson-list__order">{lesson.lessonOrder}</span>
      <a href={lesson.videoUrl} target="_blank" rel="noreferrer" className="lesson-row__title">
        {lesson.title}
      </a>
      <span className="lesson-row__actions">
        <button type="button" className="text-button" onClick={() => setEditing(true)} disabled={busy}>
          Düzenle
        </button>
        <button type="button" className="text-button text-button--danger" onClick={remove} disabled={busy}>
          Sil
        </button>
      </span>
      <ErrorList messages={errors} />
    </li>
  )
}

export function LessonManager({ courseId }: { courseId: string }) {
  const [lessons, setLessons] = useState<LessonDto[] | null>(null)
  const [loadErrors, setLoadErrors] = useState<string[]>([])

  const [title, setTitle] = useState('')
  const [videoFile, setVideoFile] = useState<File | null>(null)
  const [fileInputKey, setFileInputKey] = useState(0)
  const [progress, setProgress] = useState<number | null>(null)
  const [errors, setErrors] = useState<string[]>([])

  const reload = useCallback(
    () => coursesApi.getLessons(courseId).then(setLessons, (error) => setLoadErrors(getErrorMessages(error))),
    [courseId],
  )

  useEffect(() => {
    let cancelled = false
    coursesApi.getLessons(courseId).then(
      (result) => {
        if (!cancelled) setLessons(result)
      },
      (error) => {
        if (!cancelled) setLoadErrors(getErrorMessages(error))
      },
    )
    return () => {
      cancelled = true
    }
  }, [courseId])

  const nextOrder = (lessons ?? []).reduce((max, lesson) => Math.max(max, lesson.lessonOrder), 0) + 1

  const handleAdd = async (event: FormEvent) => {
    event.preventDefault()
    const clientErrors: string[] = []
    if (!title.trim()) clientErrors.push('Ders başlığını yaz.')
    if (!videoFile) clientErrors.push('Bir video dosyası seç.')
    if (videoFile && videoFile.size > 50 * 1024 * 1024) clientErrors.push('Video en fazla 50 MB olabilir.')
    setErrors(clientErrors)
    if (clientErrors.length > 0 || !videoFile) return

    setProgress(0)
    try {
      await panelApi.createLesson({ courseId, title, lessonOrder: nextOrder, videoFile }, setProgress)
      setTitle('')
      setVideoFile(null)
      setFileInputKey((key) => key + 1)
      await reload()
    } catch (error) {
      setErrors(getErrorMessages(error))
    } finally {
      setProgress(null)
    }
  }

  return (
    <section className="lesson-manager" aria-labelledby="lesson-manager-title">
      <h2 id="lesson-manager-title" className="form__title">
        Dersler
      </h2>
      <ErrorList messages={loadErrors} />

      {lessons && lessons.length === 0 && <p className="page-status">Bu kursta henüz ders yok.</p>}
      {lessons && lessons.length > 0 && (
        <ol className="lesson-rows">
          {lessons.map((lesson) => (
            <LessonRow key={lesson.id} lesson={lesson} onSaved={reload} onDeleted={reload} />
          ))}
        </ol>
      )}

      <form className="lesson-add" onSubmit={handleAdd} noValidate>
        <h3 className="lesson-add__title">{nextOrder}. dersi ekle</h3>
        <ErrorList messages={errors} />
        <label className="field">
          <span className="field__label">Başlık</span>
          <input value={title} maxLength={200} onChange={(event) => setTitle(event.target.value)} />
        </label>
        <label className="field">
          <span className="field__label">Video</span>
          <input
            key={fileInputKey}
            type="file"
            accept="video/*"
            onChange={(event) => setVideoFile(event.target.files?.[0] ?? null)}
          />
          <span className="field__hint">En fazla 50 MB. Video Cloudinary’ye yüklenir.</span>
        </label>

        {progress !== null && (
          <div className="progress" role="progressbar" aria-valuemin={0} aria-valuemax={100} aria-valuenow={progress}>
            <div className="progress__bar" style={{ width: `${progress}%` }} />
            <span className="progress__label">
              {progress < 100 ? `Yükleniyor… %${progress}` : 'Video işleniyor…'}
            </span>
          </div>
        )}

        <button type="submit" className="button button--primary" disabled={progress !== null}>
          Dersi ekle
        </button>
      </form>
    </section>
  )
}
