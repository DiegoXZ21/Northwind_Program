<template>
    <q-form @submit="saveOrder">
        <div class="row q-col-gutter-lg">
            <div class="col-12 col-md-4">
                <q-card class="q-pa-md">
                    <div class="text-h6 q-mb-md">Customer</div>
                    <q-input
                        v-model="order.customerName"
                        label="Customer"
                        readonly
                        outlined
                        class="q-mb-sm"
                        @click="showCustomerModal = true"
                    />
                    <q-input
                        v-model="order.city"
                        label="City"
                        readonly
                        outlined
                        class="q-mb-sm"
                    />
                    <q-input
                        v-model="order.country"
                        label="Country"
                        readonly
                        outlined
                    />
                </q-card>
                <q-card class="q-pa-md q-mt-md">
                    <div class="text-h6 q-mb-md">Employee</div>
                    <employee-dropdown v-model="order.employeeId" />
                </q-card>

            </div>
            <div class="col-12 col-md-8">

                <q-card class="q-pa-md">
                <div class="text-h6 q-mb-md">Order Details</div>
                <q-input
                    v-model="order.orderDate"
                    label="Order Date"
                    type="date"
                    outlined
                    class="q-mb-sm"
                />
                <q-input
                    v-model="order.shipAdress"
                    label="Shipping Address"
                    outlined
                    class="q-mb-md"
                />
                
                <div class="q-mt-md">
                    <div id="map" style="height:300px; border-radius: 8px;"></div>
                </div>
                </q-card>

            </div>
        </div>

        <div class="q-mt-md">
        <q-btn label="Save Order" type="submit" color="primary" />
        </div>

        <!-- Customers Modal -->
    <customer-modal v-if="showCustomerModal" v-model="showCustomerModal" @select="setCustomer" />
    </q-form>
</template>

<script>
import CustomerModal from './CustomerModal.vue';
import EmployeeDropdown from './EmployeeDropdown.vue';

export default {
    components: { CustomerModal, EmployeeDropdown },
    data() {
        return {
            order: { customerId: '', customerName: '', city: '', country: '', employeeId: null, orderDate: '', shipAdress: '' },
            showCustomerModal: false
        }
    },
    methods: {
        saveOrder() { console.log ('Order Saved', this.order) },
        setCustomer (customer) {
            this.order = {
                ...this.order,
                customerId: customer.customerId,
                customerName: customer.companyName,
                city: customer.city,
                country: customer.country
            }
        }
    }
}
</script>