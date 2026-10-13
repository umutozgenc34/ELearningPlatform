import { BrowserRouter, Route, Routes } from 'react-router-dom'
import { AuthProvider } from './auth/AuthProvider'
import { RequireAuth } from './auth/RequireAuth'
import { BasketProvider } from './basket/BasketProvider'
import { Layout } from './components/Layout'
import { BasketPage } from './pages/BasketPage'
import { CourseDetailPage } from './pages/CourseDetailPage'
import { HomePage } from './pages/HomePage'
import { LoginPage } from './pages/LoginPage'
import { NotFoundPage } from './pages/NotFoundPage'
import { RegisterPage } from './pages/RegisterPage'

export default function App() {
  return (
    <AuthProvider>
      <BasketProvider>
        <BrowserRouter>
          <Routes>
            <Route element={<Layout />}>
              <Route index element={<HomePage />} />
              <Route path="kurs/:id" element={<CourseDetailPage />} />
              <Route
                path="sepet"
                element={
                  <RequireAuth>
                    <BasketPage />
                  </RequireAuth>
                }
              />
              <Route path="giris" element={<LoginPage />} />
              <Route path="kayit" element={<RegisterPage />} />
              <Route path="*" element={<NotFoundPage />} />
            </Route>
          </Routes>
        </BrowserRouter>
      </BasketProvider>
    </AuthProvider>
  )
}
