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
                    <div class="text-h6 q-mb-md">Shipping Details</div>

                    <div class="row q-col-gutter-md">

                        <div class="col-12 col-md-6">
                            <q-input
                            v-model="order.requiredDate"
                            label="Required Date"
                            type="date"
                            outlined
                            />
                        </div>

                        <div class="col-12 col-md-6">
                            <q-input
                            v-model="order.shipAddress"
                            label="Shipping Address"
                            outlined
                            />
                        </div>

                        <div class="col-12 col-md-6">
                            <q-input
                            v-model="order.shipCity"
                            label="City"
                            outlined
                            />
                        </div>

                        <div class="col-12 col-md-6">
                            <q-input
                            v-model="order.shipRegion"
                            label="Region"
                            outlined
                            />
                        </div>

                        <div class="col-12 col-md-6">
                            <q-input
                            v-model="order.shipCountry"
                            label="Country"
                            outlined
                            />
                        </div>

                        <div class="col-12 col-md-6">
                            <q-input
                            v-model="order.shipPostalCode"
                            label="Postal Code"
                            outlined
                            />
                        </div>

                    </div>

                    <div class="q-mt-lg">
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
import api from '../boot/axios';

export default {
    components: { CustomerModal, EmployeeDropdown },
    data() {
        return {
            order: { customerId: '', customerName: '', employeeId: null, orderDate: '', shipAddress: '', shipCity: '', shipRegion: '', shipPostalCode: '', shipCountry: '' },
            showCustomerModal: false
        }
    },
    methods: {
        saveOrder() { console.log ('Order Saved', this.order) },
        async setCustomer (customer) {
            this.order = {
                ...this.order,
                customerId: customer.customerId,
                customerName: customer.companyName,
                city: customer.city,
                country: customer.country
            }

            try {
                const response = await api.get(`/Customer/dto/${customer.customerId}`)

                const data = response.data[0]

                this.order.shipAddress = data.address
                this.order.shipCountry = data.country
                this.order.shipCity = data.city
                this.order.shipRegion = data.region
                this.order.shipPostalCode = data.postalCode

            } catch (error) {
                console.error('Error setting customer:', error);
            }
        }
    }
}
</script>