import type { TokenDto } from '../api/types'

const STORAGE_KEY = 'elearning.token'

function isExpired(token: TokenDto) {
  return new Date(token.accessTokenExpiration).getTime() <= Date.now()
}

export const tokenStorage = {
  get(): TokenDto | null {
    try {
      const raw = localStorage.getItem(STORAGE_KEY)
      if (!raw) return null
      const token = JSON.parse(raw) as TokenDto
      if (isExpired(token)) {
        localStorage.removeItem(STORAGE_KEY)
        return null
      }
      return token
    } catch {
      return null
    }
  },

  set(token: TokenDto) {
    try {
      localStorage.setItem(STORAGE_KEY, JSON.stringify(token))
    } catch {
      return
    }
  },

  clear() {
    try {
      localStorage.removeItem(STORAGE_KEY)
    } catch {
      return
    }
  },
}
