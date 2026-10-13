export interface ServiceResult<T> {
  data?: T | null
  errorMessage?: string[] | null
  message?: string | null
}

export interface TokenDto {
  accessToken: string
  accessTokenExpiration: string
}

export type RoleType = 'User' | 'Educator'

export interface UserDto {
  id: string
  userName: string
  email: string
  roles: string[]
}

export interface LoginRequest {
  email: string
  password: string
}

export interface RegisterRequest {
  userName: string
  firstName: string
  lastName: string
  email: string
  password: string
  role: RoleType
}

export interface CategoryDto {
  id: number
  name: string
  created: string
  updated: string
}
