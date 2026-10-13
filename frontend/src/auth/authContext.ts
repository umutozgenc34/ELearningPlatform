import { createContext } from 'react'
import type { LoginRequest, UserDto } from '../api/types'

export type AuthStatus = 'loading' | 'authenticated' | 'anonymous'

export interface AuthContextValue {
  user: UserDto | null
  status: AuthStatus
  login: (request: LoginRequest) => Promise<void>
  logout: () => void
  hasRole: (...roles: string[]) => boolean
}

export const AuthContext = createContext<AuthContextValue | null>(null)
