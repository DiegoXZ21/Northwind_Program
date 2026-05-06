<template>
    <q-page padding>
        <div class="text-h5 q-mb-md">
            Orders Report
        </div>

        <q-card class="q-pa-md">
            <div class="row q-col-gutter-md q-mb-md items-end">
                <div class="col-12 col-md-3">
                    <q-select 
                        v-model="filters.year"
                        :options="years"
                        label="Year"
                        dense
                        outlined
                        emit-value
                        map-options
                        clearable
                    />
                </div>

                <div class="col-12 col-md-3">
                    <q-select 
                        v-model="filters.month"
                        :options="months"
                        label="Month"
                        dense
                        outlined
                        emit-value
                        map-options
                        clearable
                    />
                </div>

                <div class="col-12 col-md-3">
                    <q-input
                        v-model="filters.country"
                        label="Country"
                        dense
                        outlined
                        clearable
                    />
                </div>

                <div class="col-12 col-md-3 flex items-end justify-end q-gutter-sm">

                    <q-btn
                        color="primary"
                        icon="picture_as_pdf"
                        label="PDF"
                        :disable="selected.length === 0"
                        @click="exportPdf"
                    />

                    <q-btn
                        color="secondary"
                        icon="table_view"
                        label="Excel"
                        :disable="selected.length === 0"
                        @click="exportExcel"
                    />

                </div>

            </div>

            

            <q-table
                :rows="orders"
                :columns="columns"
                row-key="orderId"
                :loading="loading"
                selection="multiple"
                v-model:selected="selected"
            >

            <template v-slot:body-cell-actions="props">
                <q-td :props="props">
                    <q-btn 
                        flat
                        dense
                        icon="visibility"
                        color="primary"
                        @click="goToDetail(props.row.orderId)"
                    />
                </q-td>
            </template>


            <template>
                <q-inner-loading showing>
                    <q-spinner-dots size="40px" />
                </q-inner-loading>
            </template>


            <template>
                <div class="full-width text-center q-pa-md">
                    No orders found
                </div>
            </template>
            </q-table>
        </q-card>
    </q-page>
</template>

<script>
import api from '../boot/axios'
import { generateOrdersPDF } from '../services/reportService'
import { generateOrdersExcel } from '../services/orderReportExcel'
export default {
    data() {
        return {
            orders: [],
            selected: [],
            loading: false,

            years: [],
            months: [
                { label: 'January', value: 1},
                { label: 'February', value: 2},
                { label: 'March', value: 3},
                { label: 'April', value: 4},
                { label: 'May', value: 5},
                { label: 'June', value: 6},
                { label: 'July', value: 7},
                { label: 'August', value: 8 },
                { label: 'September', value: 9 },
                { label: 'October', value: 10 },
                { label: 'November', value: 11 },
                { label: 'December', value: 12 }
            ],

            filters: {
                year: null,
                month: null,
                country: ''
            },
            columns: [
                { name: 'orderId', label: 'ID', field: 'orderId', align: 'center'},
                { name: 'customerName', label: 'Customer', field: 'customerName', align: 'center' },
                { name: 'orderDate', label: 'Order Date', field: 'orderDate', align: 'center' },
                { name: 'country', label: 'Country', field: 'country', align: 'center' },
                { name: 'status', label: 'Status', field: 'status', align: 'center' },
                { name: 'totalAmount', label: 'Total', field: 'totalAmount', align: 'center',  format: val => new Intl.NumberFormat('en-US', {minimumFractionDigits: 2, maximumFractionDigits: 2}).format(val ?? 0)},
                { name: 'productCount', label: 'Products', field: 'productCount', align: 'center' },
                { name: 'actions', label: 'Actions', field: 'actions', align: 'center'}
            ]
        }
    },
    mounted() {
        this.loadYears()
        this.loadOrders()
    },
    watch: {
        filters: {
            deep: true,
            handler() {
                this.loadOrders()
            }
        }
    },
    methods: {
        async loadYears() {
            try {
                const res = await api.get('/Order/years')
                this.years = res.data
            } catch (error) {
                console.error(error)
            }
        },
        async loadOrders() {
            this.loading = true
            try {
                const res = await api.get('/Order', {
                    params: {
                        year: this.filters.year,
                        month: this.filters.month,
                        country: this.filters.country
                    }
                })

                this.orders = res.data
            } catch (error) {
                console.error(error)
            } finally {
                this.loading = false
            }
        },
        exportPdf() {
            if (this.selected.length === 0) {
                this.$q.notify({
                    type: "warning",
                    message: "Select at least one order"
                })
                return
            }

            generateOrdersPDF(this.selected, this.filters)

        },
        exportExcel() {
            if (this.selected.length === 0) {
                this.$q.notify({
                    type: "warning",
                    message: "Select at least one order"
                })
                return
            }

            generateOrdersExcel(this.selected, this.filters)
        },
        goToDetail(orderId) {
            this.$router.push(`/orders/${orderId}`)
        }
    }
}
</script>