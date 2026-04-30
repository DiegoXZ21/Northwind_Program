import { createRouter, createWebHistory } from 'vue-router'
import MainLayout from '../layouts/MainLayout.vue'

import DashboardPage from '../pages/DashBoardPage.vue'
import OrdersPage from '../pages/OrdersPage.vue'
import ReportsPage from '../pages/ReportsPage.vue'
import CustomersPage from '../pages/CustomersPage.vue'

const routes = [
    {
        path: '/',
        component: MainLayout,
        children: [
            { path: '', redirect: '/dashboard' },
            { path: 'dashboard', component: DashboardPage },
            {path: 'orders', component: OrdersPage },
            {path: 'reports', component: ReportsPage },
            {path: 'customers', component: CustomersPage }
        ]
        
    }
]

export default createRouter({
    history: createWebHistory(),
    routes
})