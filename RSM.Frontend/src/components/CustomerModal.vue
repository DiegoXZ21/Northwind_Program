<template>
    <q-dialog 
        :model-value="modelValue"
        @update:model-value="$emit('update:modelValue', $event)">
        <q-card>
            <q-card-section>
                <q-table 
                    :rows="customers"
                    :columns="columns"
                    row-key="customerId"
                    @row-click="selectCustomer"
                />
            </q-card-section>
        </q-card>
    </q-dialog>
</template>

<script>
import api from '../boot/axios'
export default {
    props: { 
        type: Boolean,
        default: false },
    emits: ['update:modelValue', 'select'],
    data () {
        return {
            customers: [],
            columns: [
                { name: 'customerId', label: 'ID', field: 'customerId' },
                { name: 'companyName', label: 'Company Name', field: 'companyName' },
                { name: 'city', label: 'City', field: 'city' },
                { name: 'country', label: 'Country', field: 'country' }
            ]
        }
    }, 
    mounted() {
        this.loadCustomers()
    },
    methods: {
        async loadCustomers() {
            try {
                const response = await api.get('/Customer/dto')
                this.customers = response.data
            } catch (error) {
                console.error('Error loading customers:', error)
            }
        },
        selectCustomer (row) {
            this.$emit('select', row)
            this.$emit('update:modelValue', false)
        }
    }
}
</script>