import { useEffect, useState, type FormEvent } from 'react'
import { categoriesApi } from '../../api/categories'
import { getErrorMessages } from '../../api/client'
import { panelApi } from '../../api/panel'
import type { CategoryDto } from '../../api/types'
import { ErrorList } from '../../components/ErrorList'

function CategoryRow({ category, onChanged }: { category: CategoryDto; onChanged: () => void }) {
  const [editing, setEditing] = useState(false)
  const [name, setName] = useState(category.name)
  const [errors, setErrors] = useState<string[]>([])
  const [busy, setBusy] = useState(false)

  const save = async (event: FormEvent) => {
    event.preventDefault()
    if (!name.trim()) {
      setErrors(['Kategori adı boş olamaz.'])
      return
    }
    setBusy(true)
    setErrors([])
    try {
      await panelApi.updateCategory(category.id, name.trim())
      setEditing(false)
      onChanged()
    } catch (error) {
      setErrors(getErrorMessages(error))
    } finally {
      setBusy(false)
    }
  }

  const remove = async () => {
    const confirmed = window.confirm(
      `“${category.name}” kategorisi silinsin mi? Bu kategorideki tüm kurslar da silinir.`,
    )
    if (!confirmed) return
    setBusy(true)
    try {
      await panelApi.deleteCategory(category.id)
      onChanged()
    } catch (error) {
      setErrors(getErrorMessages(error))
      setBusy(false)
    }
  }

  return (
    <li className="panel-list__item panel-list__item--compact">
      {editing ? (
        <form className="inline-form" onSubmit={save}>
          <input aria-label="Kategori adı" value={name} maxLength={100} onChange={(event) => setName(event.target.value)} />
          <button type="submit" className="button button--primary" disabled={busy}>
            Kaydet
          </button>
          <button
            type="button"
            className="text-button"
            onClick={() => {
              setEditing(false)
              setName(category.name)
              setErrors([])
            }}
          >
            Vazgeç
          </button>
        </form>
      ) : (
        <>
          <span className="panel-list__name">{category.name}</span>
          <span className="panel-list__actions">
            <button type="button" className="text-button" onClick={() => setEditing(true)} disabled={busy}>
              Yeniden adlandır
            </button>
            <button type="button" className="text-button text-button--danger" onClick={remove} disabled={busy}>
              Sil
            </button>
          </span>
        </>
      )}
      <ErrorList messages={errors} />
    </li>
  )
}

export function PanelCategoriesPage() {
  const [categories, setCategories] = useState<CategoryDto[] | null>(null)
  const [name, setName] = useState('')
  const [errors, setErrors] = useState<string[]>([])
  const [adding, setAdding] = useState(false)

  const reload = () => categoriesApi.getAll().then(setCategories, (error) => setErrors(getErrorMessages(error)))

  useEffect(() => {
    categoriesApi.getAll().then(setCategories, (error) => setErrors(getErrorMessages(error)))
  }, [])

  const handleAdd = async (event: FormEvent) => {
    event.preventDefault()
    if (!name.trim()) return
    setErrors([])
    setAdding(true)
    try {
      await panelApi.createCategory(name.trim())
      setName('')
      await reload()
    } catch (error) {
      setErrors(getErrorMessages(error))
    } finally {
      setAdding(false)
    }
  }

  return (
    <div className="panel__section panel__section--narrow">
      <form className="inline-form inline-form--add" onSubmit={handleAdd}>
        <label className="field">
          <span className="field__label">Yeni kategori</span>
          <input value={name} maxLength={100} placeholder="ör. Mobil Geliştirme" onChange={(event) => setName(event.target.value)} />
        </label>
        <button type="submit" className="button button--primary" disabled={adding || !name.trim()}>
          {adding ? 'Ekleniyor…' : 'Ekle'}
        </button>
      </form>
      <ErrorList messages={errors} />

      {categories === null && errors.length === 0 && <p className="page-status">Kategoriler yükleniyor…</p>}
      {categories && (
        <ul className="panel-list">
          {categories.map((category) => (
            <CategoryRow key={category.id} category={category} onChanged={reload} />
          ))}
        </ul>
      )}
    </div>
  )
}
