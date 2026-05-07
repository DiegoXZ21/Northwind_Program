import { createRouter, createWebHistory } from 'vue-router'
import MainLayout from '../layouts/MainLayout.vue'

import DashboardPage from '../pages/DashBoardPage.vue'
import OrdersPage from '../pages/OrdersPage.vue'
import InventoryPage from '../pages/InventoryPage.vue'
import OrdersReportPage from '../pages/OrdersReportPage.vue'
import OrderDetail from '../pages/OrderDetailPage.vue'

const routes = [
    {
        path: '/',
        component: MainLayout,
        children: [
            { path: '', redirect: '/dashboard' },
            { path: 'dashboard', component: DashboardPage },
            {path: 'orders', component: OrdersPage },
            {path: 'inventory', component: InventoryPage },
            {path: 'ordersReport', component: OrdersReportPage},
            {path: 'orders/:id', component: OrderDetail}
        ]
        
    }
]

export default createRouter({
    history: createWebHistory(),
    routes
})