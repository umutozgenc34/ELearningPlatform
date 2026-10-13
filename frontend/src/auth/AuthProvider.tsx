import { useCallback, useEffect, useMemo, useState, type ReactNode } from 'react'
import { authApi } from '../api/auth'
import { setUnauthorizedListener } from '../api/client'
import type { LoginRequest, UserDto } from '../api/types'
import { AuthContext, type AuthStatus } from './authContext'
import { tokenStorage } from './tokenStorage'

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<UserDto | null>(null)
  const [status, setStatus] = useState<AuthStatus>(() => (tokenStorage.get() ? 'loading' : 'anonymous'))

  const logout = useCallback(() => {
    tokenStorage.clear()
    setUser(null)
    setStatus('anonymous')
  }, [])

  const fetchCurrentUser = useCallback(
    () =>
      authApi.me().then(
        (currentUser) => {
          setUser(currentUser)
          setStatus('authenticated')
        },
        () => logout(),
      ),
    [logout],
  )

  useEffect(() => {
    setUnauthorizedListener(logout)
    return () => setUnauthorizedListener(null)
  }, [logout])

  useEffect(() => {
    if (tokenStorage.get()) {
      void fetchCurrentUser()
    }
  }, [fetchCurrentUser])

  const login = useCallback(
    async (request: LoginRequest) => {
      const token = await authApi.login(request)
      tokenStorage.set(token)
      await fetchCurrentUser()
    },
    [fetchCurrentUser],
  )

  const hasRole = useCallback((...roles: string[]) => roles.some((role) => user?.roles.includes(role)), [user])

  const value = useMemo(() => ({ user, status, login, logout, hasRole }), [user, status, login, logout, hasRole])

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}
