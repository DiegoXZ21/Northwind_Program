import { createRouter, createWebHistory } from 'vue-router'
import DashboardPage from '../pages/DashBoardPage.vue'
import OrdersPage from '../pages/OrdersPage.vue'
import ReportsPage from '../pages/ReportsPage.vue'
import CustomersPage from '../pages/CustomersPage.vue'

const routes = [
    { path: '/dashboard', component: DashboardPage },
    {path: '/orders', component: OrdersPage },
    {path: '/reports', component: ReportsPage },
    {path: '/customers', component: CustomersPage },
    {path: '/', redirect: '/dashboard' }
]

export default createRouter({
    history: createWebHistory(),
    routes
})