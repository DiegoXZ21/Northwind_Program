<template>
    <q-select
        :model-value="modelValue"
        @update:model-value="$emit('update:modelValue', $event)"
        :options="employees"
        option-label="employeeName"
        option-value="employeeId"
        emit-value
        map-options
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