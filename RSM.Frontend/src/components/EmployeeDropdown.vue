<template>
    <q-select
        v-model="selectedEmployee"
        :options="employees"
        option-value="employeeId"
        option-label="employeeName"
        label="Employee"
        @update:model-value="updateEmployee"    
    />
</template>

<script>
import api from '../boot/axios'
export default {
    props: { modelValue: Number },
    emits: ['update:modelValue'],
    data() {
        return{
            selectedEmployee: this.modelValue,
            employees: []
        }
    },
    mounted() {
        this.loadEmployees()
    },
    methods: {
        async loadEmployees(){
            try {
                const response = await api.get('/Employee')
                this.employees = response.data
            } catch (error) {
                console.error('Error loading employees:', error)
            }
        }, 
        updateEmployee(value) {
            this.$emit('update:modelValue', value)
        }
    }
}
</script>