import { api, unwrap } from './client'
import type { CourseDto, LessonDto, ServiceResult } from './types'

export const coursesApi = {
  getAll: () => unwrap(api.get<ServiceResult<CourseDto[]>>('/courses')),

  getById: (id: string) => unwrap(api.get<ServiceResult<CourseDto>>(`/courses/${id}`)),

  getByCategory: (categoryId: number) =>
    unwrap(api.get<ServiceResult<CourseDto[]>>('/courses/byCategory', { params: { categoryId } })),

  search: (keyword: string) =>
    unwrap(api.get<ServiceResult<CourseDto[]>>('/courses/search', { params: { keyword } })),

  getLessons: (courseId: string) => unwrap(api.get<ServiceResult<LessonDto[]>>(`/courses/${courseId}/lessons`)),
}
