<template>
    <q-dialog :model-value="modelValue" @update:model-value="$emit('update:modelValue', $event)" >
        <q-card>

            <q-card-section>
                <div class="text-h6">Update Stock</div>
            </q-card-section>

            <q-card-section>
                <div class="q-mb-md">
                    <strong>{{ product?.productName }}</strong>
                </div>

                <q-input
                v-model.number="newStock"
                label="New Stock"
                type="number"
                outlined
                />
            </q-card-section>

            <q-card-actions align="right">
                <q-btn flat label="Cancel" v-close-popup />
                <q-btn label="Save" color="primary" :loading="loading" :disable="loading" @click="updateStock" />
            </q-card-actions>
        </q-card>
    </q-dialog>
</template>
<script>
import { Loading } from 'quasar';
import api from '../boot/axios'

export default {
    props: {
        modelValue: Boolean,
        product: Object
    },

    emits: ['update:modelValue', 'updated'],

    data() {
        return {
            newStock: 0,
            Loading: false
        }
    },

    watch: {
        product(val) {
            if (val) {
                this.newStock = val.unitsInStock ?? 0
            }
        },
        modelValue(val) {
            if (!val) {
                this.newStock = 0
            }
        }
    },

    methods: {
        async updateStock() {
            try {
                if (this.newStock < 0) {
                this.$q.notify({
                    type: 'negative',
                    message: 'Stock cannot be negative'
                })
                return
                }

                this.Loading = true

                await api.patch(`/Product/inventory/${this.product.productId}`, { unitsInStock: this.newStock })
                this.$emit('updated')
                this.$emit('update:modelValue', false)
                
                this.$q.notify({ 
                    type: 'positive',
                    message: 'Stock updated successfully!' 
                })
                
            } catch (error) {
                console.error('Error updating stock:', error)
            } finally {
                this.Loading = false
            }
        }
    }
}
</script>