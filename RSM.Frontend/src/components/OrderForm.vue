<template>
    <q-form @submit="saveOrder">
        <!-- Client Selection -->
        <q-input v-model="order.customerName" label="Customer" @click="showCustomerModal = true"/>
        
        <!-- Employee Selection -->
        <employee-dropdown v-model="order.employeeId" />

        <!-- Order Date-->
        <q-input v-model="order.orderDate" label="Order Date" type="date" />

        <!-- Adress -->
        <q-input v-model="order.shipAdress" label="Shipping Address" />

        <!-- map -->
        <div id="map" style="height:300px;"></div>

        <q-btn label="Save" type="submit" color="primary" />

        <!-- Customers Modal -->
    <customer-modal v-model="showCustomerModal" @select="setCustomer" />
    </q-form>
</template>

<script>
import CustomerModal from './CustomerModal.vue';
import EmployeeDropdown from './EmployeeDropdown.vue';

export default {
    components: { CustomerModal, EmployeeDropdown },
    data() {
        return {
            order: { customerName: '' , employeeId: null, orderDate: '', shipAdress: '' },
            showCustomerModal: false
        }
    },
    methods: {
        saveOrder() { console.log ('Order Saved', this.order) },
        setCustomer (customer) {
            this.order.customerId = customer.id
            this.order.customerName = customer.companyName 
        }
    }
}
</script>