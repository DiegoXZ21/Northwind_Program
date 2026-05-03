<template>
    <q-dialog 
        :model-value="modelValue"
        @update:model-value="$emit('update:modelValue', $event)">
        <q-card style="min-width: 800px; width: 900px">
            <q-card-section>
                <div class="row q-col-gutter-md q-mb-md">
                    <div class="col">
                        <q-input 
                        v-model="filters.company"
                        label = "Company Name"
                        dense
                        filled
                        />
                    </div>

                    <div class="col">
                        <q-input 
                        v-model="filters.city"
                        label = "City"
                        dense
                        filled
                        />
                    </div>

                    <div class="col">
                        <q-input 
                        v-model="filters.country"
                        label = "Country"
                        dense
                        filled
                        />
                    </div>
                </div>
            </q-card-section>
            <q-card-section style="height: 400px;">
                <q-table 
                    :rows="filteredCustomers"
                    :columns="columns"
                    row-key="customerId"
                    :loading="loading"
                    @row-click="selectCustomer"
                >
            
                <template v-slot:loading>
                    <q-inner-loading showing>
                        <q-spinner-dots color="primary" size="50px" />
                    </q-inner-loading>
                </template>

                <template v-slot:no-data>
                    <div class="full-width text-center q-pa-md">
                        <q-icon name="search_off" size="40px" class="q-mb-sm" />
                        <div v-if="filters.company || filters.city || filters.country">
                            No results found
                        </div>

                        <div v-else>
                            No customers available
                        </div>
                    </div>
                </template>

                </q-table>
            </q-card-section>
        </q-card>
    </q-dialog>
</template>

<script>
import api from '../boot/axios'
export default {
    props: { 
        modelValue: {
            type: Boolean,
        default: false
        }
    },
    emits: ['update:modelValue', 'select'],
    data () {
        return {
            customers: [],
            loading: false,
            columns: [
                { name: 'customerId', label: 'ID', field: 'customerId', align: 'center' },
                { name: 'companyName', label: 'Company Name', field: 'companyName', align: 'center' },
                { name: 'city', label: 'City', field: 'city', align: 'center' },
                { name: 'country', label: 'Country', field: 'country', align: 'center' }
            ],
            filters: {
                company: '',
                city: '',
                country: ''
            }
        }
    }, 
    watch: {
        modelValue: {
        immediate: true,
        handler(val) {
            if (val) {
                this.loadCustomers()
            } else {
                this.resetFilters()
            }
        }
    }
    },
    computed: {
        filteredCustomers(){
            return this.customers.filter(c => {
                return(
                    ((c.companyName || '').toLowerCase().includes(this.filters.company.toLowerCase())) &&
                    ((c.city || '').toLowerCase().includes(this.filters.city.toLowerCase())) &&
                    ((c.country || '').toLowerCase().includes(this.filters.country.toLowerCase()))
                )
            })
        }
    },
    methods: {
        async loadCustomers() {
            this.loading = true
            try {
                const response = await api.get('/Customer/dto')
                this.customers = response.data
            } catch (error) {
                console.error('Error loading customers:', error)
            } finally {
                this.loading = false
            }
        },
        selectCustomer (evt, row) {
            this.$emit('select', row)
            this.$emit('update:modelValue', false)
        },
        resetFilters(){
            this.filters = { 
                company: '',
                city: '',
                country: ''
            }
        }
    }
}
</script>