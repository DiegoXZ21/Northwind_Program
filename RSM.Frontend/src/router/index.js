import { createRouter, createWebHistory } from 'vue-router'
import MainLayout from '../layouts/MainLayout.vue'

import DashboardPage from '../pages/DashBoardPage.vue'
import OrdersPage from '../pages/OrdersPage.vue'
import ReportsPage from '../pages/ReportsPage.vue'
import InventoryPage from '../pages/InventoryPage.vue'
import OrdersReportPage from '../pages/OrdersReportPage.vue'

const routes = [
    {
        path: '/',
        component: MainLayout,
        children: [
            { path: '', redirect: '/dashboard' },
            { path: 'dashboard', component: DashboardPage },
            {path: 'orders', component: OrdersPage },
            {path: 'reports', component: ReportsPage },
            {path: 'inventory', component: InventoryPage },
            {path: 'ordersReport', component: OrdersReportPage}
        ]
        
    }
]

export default createRouter({
    history: createWebHistory(),
    routes
})