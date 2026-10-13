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

export interface CourseDetailsDto {
  duration: number
  rating: number
  educatorFullName: string
}

export interface CourseDto {
  id: string
  name: string
  description: string
  price: number
  imageUrl: string | null
  created: string
  updated: string
  category: CategoryDto | null
  courseDetails: CourseDetailsDto | null
}

export interface LessonDto {
  id: number
  courseId: string
  title: string
  videoUrl: string
  lessonOrder: number
  created: string
  updated: string
}

export interface BasketItemDto {
  id: string
  name: string
  imageUrl: string | null
  price: number
  priceByApplyDiscountRate: number | null
}

export interface BasketDto {
  items: BasketItemDto[]
  totalPrice: number
}

export interface DiscountDto {
  coupon: string
  rate: number
}

export type OrderStatus = 'Pending' | 'Confirmed' | 'Canceled'

export interface OrderItem {
  courseId: string
  courseName: string
  price: number
}

export interface Order {
  id: string
  userId: string
  orderItems: OrderItem[]
  totalPrice: number
  status: OrderStatus
  created: string
  updated: string
}

export interface PaymentRequest {
  orderId: string
  cardHolderName: string
  cardNumber: string
  expiryDate: string
  cvv: string
}
