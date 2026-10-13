import { BrowserRouter, Route, Routes } from 'react-router-dom'
import { AuthProvider } from './auth/AuthProvider'
import { RequireAuth } from './auth/RequireAuth'
import { RequireRole } from './auth/RequireRole'
import { BasketProvider } from './basket/BasketProvider'
import { Layout } from './components/Layout'
import { BasketPage } from './pages/BasketPage'
import { CourseDetailPage } from './pages/CourseDetailPage'
import { HomePage } from './pages/HomePage'
import { LoginPage } from './pages/LoginPage'
import { NotFoundPage } from './pages/NotFoundPage'
import { OrdersPage } from './pages/OrdersPage'
import { PaymentPage } from './pages/PaymentPage'
import { CourseEditorPage } from './pages/panel/CourseEditorPage'
import { PanelCategoriesPage } from './pages/panel/PanelCategoriesPage'
import { PanelCouponsPage } from './pages/panel/PanelCouponsPage'
import { PanelCoursesPage } from './pages/panel/PanelCoursesPage'
import { PanelLayout } from './pages/panel/PanelLayout'
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
              <Route
                path="odeme/:orderId"
                element={
                  <RequireAuth>
                    <PaymentPage />
                  </RequireAuth>
                }
              />
              <Route
                path="siparisler"
                element={
                  <RequireAuth>
                    <OrdersPage />
                  </RequireAuth>
                }
              />
              <Route
                path="panel"
                element={
                  <RequireRole roles={['Educator', 'Admin']}>
                    <PanelLayout />
                  </RequireRole>
                }
              >
                <Route index element={<PanelCoursesPage />} />
                <Route path="kurs/yeni" element={<CourseEditorPage key="new" />} />
                <Route path="kurs/:id" element={<CourseEditorPage key="edit" />} />
                <Route path="kategoriler" element={<PanelCategoriesPage />} />
                <Route path="kuponlar" element={<PanelCouponsPage />} />
              </Route>
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
