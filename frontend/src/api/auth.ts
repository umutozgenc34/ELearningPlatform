import { api, unwrap } from './client'
import type { LoginRequest, RegisterRequest, ServiceResult, TokenDto, UserDto } from './types'

export const authApi = {
  login: (request: LoginRequest) =>
    unwrap(api.post<ServiceResult<TokenDto>>('/auths/login', request)),

  register: (request: RegisterRequest) =>
    unwrap(api.post<ServiceResult<UserDto>>('/users/register', request)),

  me: () => unwrap(api.get<ServiceResult<UserDto>>('/users/me')),
}
