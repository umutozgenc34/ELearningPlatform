import type { AxiosProgressEvent } from 'axios'
import { api, unwrap } from './client'
import type { CategoryDto, DiscountDto, LessonDto, ServiceResult } from './types'

const multipart = { headers: { 'Content-Type': 'multipart/form-data' } }

export interface CourseFormValues {
  name: string
  description: string
  price: string
  categoryId: string
  imageFile: File | null
  educatorFullName: string
  duration: string
}

export interface LessonUpdateValues {
  id: number
  courseId: string
  title: string
  videoUrl: string
  lessonOrder: number
}

function courseFormData(values: CourseFormValues) {
  const form = new FormData()
  form.append('Name', values.name.trim())
  form.append('Description', values.description.trim())
  form.append('Price', values.price.replace(',', '.'))
  form.append('CategoryId', values.categoryId)
  if (values.imageFile) form.append('ImageFile', values.imageFile)
  return form
}

export const panelApi = {
  createCourse: async (values: CourseFormValues) => {
    const form = courseFormData(values)
    if (values.educatorFullName.trim()) form.append('CourseDetails.EducatorFullName', values.educatorFullName.trim())
    form.append('CourseDetails.Duration', values.duration || '0')
    const response = await api.post<ServiceResult<{ id: string }>>('/courses', form, multipart)
    return response.data.data as { id: string }
  },

  updateCourse: (id: string, values: CourseFormValues) => {
    const form = courseFormData(values)
    form.append('Id', id)
    return api.put('/courses', form, multipart)
  },

  deleteCourse: (id: string) => api.delete(`/courses/${id}`),

  createLesson: (
    values: { courseId: string; title: string; lessonOrder: number; videoFile: File },
    onProgress?: (percent: number) => void,
  ) => {
    const form = new FormData()
    form.append('CourseId', values.courseId)
    form.append('Title', values.title.trim())
    form.append('LessonOrder', String(values.lessonOrder))
    form.append('VideoFile', values.videoFile)
    return unwrap(
      api.post<ServiceResult<LessonDto>>('/lessons', form, {
        ...multipart,
        onUploadProgress: (event: AxiosProgressEvent) => {
          if (event.total) onProgress?.(Math.round((event.loaded / event.total) * 100))
        },
      }),
    )
  },

  updateLesson: (values: LessonUpdateValues) => api.put(`/lessons/${values.id}`, values),

  deleteLesson: (id: number) => api.delete(`/lessons/${id}`),

  createCategory: (name: string) => unwrap(api.post<ServiceResult<CategoryDto>>('/categories', { name })),

  updateCategory: (id: number, name: string) => api.put(`/categories/${id}`, { id, name }),

  deleteCategory: (id: number) => api.delete(`/categories/${id}`),

  getDiscounts: () => unwrap(api.get<ServiceResult<DiscountDto[]>>('/discounts')),

  createDiscount: (coupon: string, rate: number) => api.post('/discounts', { coupon, rate }),

  deleteDiscount: (coupon: string) => api.delete(`/discounts/${encodeURIComponent(coupon)}`),
}
