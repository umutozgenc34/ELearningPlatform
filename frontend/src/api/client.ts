import axios, { AxiosError } from 'axios'
import type { ServiceResult } from './types'
import { tokenStorage } from '../auth/tokenStorage'

export const api = axios.create({
  baseURL: '/api/v1',
  headers: { 'Content-Type': 'application/json' },
})

api.interceptors.request.use((config) => {
  const token = tokenStorage.get()
  if (token) {
    config.headers.Authorization = `Bearer ${token.accessToken}`
  }
  return config
})

type UnauthorizedListener = () => void
let onUnauthorized: UnauthorizedListener | null = null

export function setUnauthorizedListener(listener: UnauthorizedListener | null) {
  onUnauthorized = listener
}

api.interceptors.response.use(
  (response) => response,
  (error: AxiosError) => {
    if (error.response?.status === 401 && tokenStorage.get()) {
      onUnauthorized?.()
    }
    return Promise.reject(error)
  },
)

export async function unwrap<T>(request: Promise<{ data: ServiceResult<T> }>): Promise<T> {
  const response = await request
  return response.data.data as T
}

export function getErrorMessages(error: unknown): string[] {
  if (axios.isAxiosError<ServiceResult<unknown>>(error)) {
    const messages = error.response?.data?.errorMessage
    if (messages && messages.length > 0) return messages

    if (!error.response) {
      return ['Sunucuya ulaşılamıyor. API çalışıyor mu?']
    }

    switch (error.response.status) {
      case 401:
        return ['Bu işlem için giriş yapmalısın.']
      case 403:
        return ['Bu işlem için yetkin yok.']
      case 404:
        return ['Aradığın kayıt bulunamadı.']
      default:
        return [`Beklenmeyen bir hata oluştu (${error.response.status}).`]
    }
  }

  return ['Beklenmeyen bir hata oluştu.']
}
