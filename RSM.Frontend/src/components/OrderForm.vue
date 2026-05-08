<template>
    <q-form @submit.prevent="saveOrder" ref="orderForm">
        <div class="row q-col-gutter-lg">
            <div class="col-12 col-md-4 column q-gutter-md">
                <q-card class="q-pa-md">
                    <div class="text-h6 q-mb-md">Customer</div>
                    <q-input
                        v-model="order.customerName"
                        label="Customer"
                        readonly
                        outlined
                        class="q-mb-sm"
                        @click="showCustomerModal = true"
                        :rules="[val => !!val || 'Customer is required']"
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
                <q-card class="q-pa-md">
                    <div class="text-h6 q-mb-md">Employee</div>

                    <employee-dropdown 
                        v-model="order.employeeId"
                        :rules="[val => !!val || 'Employee is required']"
                     />
                </q-card>

                <q-card class="q-pa-md col-grow">
                    <div class="text-h6 q-mb-md">Shipper</div>

                    <q-select
                        v-model="order.shipperId"
                        :options="shippers"
                        label="Shipper"
                        emit-value
                        map-options
                        outlined
                        :rules="[val => !!val || 'Shipper is required']"
                    />
                </q-card>

            </div>
            <div class="col-12 col-md-8">

                <q-card class="q-pa-md">
                    <div class="text-h6 q-mb-md">Shipping Details</div>

                    <div class="row q-col-gutter-md">

                        <div class="col-12 col-md-6">
                            <q-input
                                :model-value="formattedOrderDate"
                                label="Order Date"
                                outlined
                                readonly
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

                        <div class="col-12 col-md-6">
                            <q-input
                                v-model="order.shipName"
                                label="Ship Name"
                                outlined
                                :rules="[val => !!val || 'Ship Name is required']"
                            />
                        </div>

                        <div class="col-12 col-md-6">
                            <q-input
                                v-model.number="order.freight"
                                label="Freight"
                                type="number"
                                outlined
                                :rules="[val => !!val || 'Freight is required']"
                            />
                        </div>

                        <div class="col-12">
                            <q-btn
                                color="primary"
                                label="Validate Address"
                                icon="location_on"
                                @click="validateAddress"
                            />
                        </div>

                        <div class="col-12 col-md-6">
                            <q-input
                                :model-value="order.latitude"
                                label="Latitude"
                                outlined
                                readonly
                                :rules="[val => !!val || 'Latitude is required']"
                            />
                        </div>

                        <div class="col-12 col-md-6">
                            <q-input
                                :model-value="order.longitude"
                                label="Longitude"
                                outlined
                                readonly
                                :rules="[val => !!val || 'Longitud is required']"
                            />
                        </div>
                    </div>

                    <div class="q-mt-lg">
                        <div id="map" style="height:300px; border-radius: 8px;"></div>
                    </div>

                </q-card>

            </div>
        </div>



        <q-card class="q-pa-md q-mt-md">

            <div class="row items-center justify-between q-mb-md">
                <div class="text-h6">Products</div>

                <q-btn
                    color="primary"
                    icon="add"
                    label="Add Product"
                    @click="showProductDialog = true"
                />
            </div>

            <OrderProductsTable
                :products="products"
                :isEditing="true"
                @remove="removeProduct"
            />

        </q-card>

        <div class="q-mt-md">
            <q-btn label="Save Order" type="submit" color="primary" />
        </div>
        <!-- Customers Modal -->
    <customer-modal v-if="showCustomerModal" v-model="showCustomerModal" @select="setCustomer" />

    <ProductSelectDialog
        v-model="showProductDialog"
        :products="availableProducts"
        @add="handleAddProduct"
    />
    </q-form>
</template>

<script>
import CustomerModal from './CustomerModal.vue';
import EmployeeDropdown from './EmployeeDropdown.vue';
import api from '../boot/axios';
import OrderProductsTable from './OrderProductsTable.vue'
import ProductSelectDialog from './ProductSelectDialog.vue'

export default {
    components: { 
        CustomerModal, 
        EmployeeDropdown,
        OrderProductsTable,
        ProductSelectDialog 
    },
    data() {
        return {
            products: [],
            availableProducts: [],
            showProductDialog: false,
            shippers: [],
            order: { 
                customerId: '',
                customerName: '', 
                employeeId: null,
                shipperId: null,
                shipName: null,
                freight: 0,
                orderDate: new Date().toISOString(),
                requiredDate: null,
                shipAddress: '', 
                shipCity: '', 
                shipRegion: '', 
                shipPostalCode: '', 
                latitude: null,
                longitude: null,
                shipCountry: '' },
            showCustomerModal: false
        }
    },
    methods: {
        async saveOrder() {
            await this.$nextTick()
            const isValid = await this.$refs.orderForm.validate()

            if (!isValid) return

            if (!this.products.length) {
                this.$q.notify({
                    type: 'warning',
                    message: 'You must add at least one product'
                })
                return
            }

            const invalidProduct = this.products.find(p => {
                const quantity = Number(p.quantity)
                const stock = Number(p.unitsInStock ?? 0)

                return (
                    isNaN(quantity) ||
                    quantity < 1 ||
                    quantity > stock
                )
            })

            if (invalidProduct) {

                invalidProduct.quantity = invalidProduct.unitsInStock

                this.$q.notify({
                    type: 'negative',
                    message: `Insufficient stock for "${invalidProduct.productName}"`
                })
                return
            }

            
            const validAddress = await this.validateAddress()
            if (!validAddress)
                return

            const payload = {
                customerId: this.order.customerId,
                employeeId: this.order.employeeId,
                shipperId: this.order.shipperId,

                shipName: this.order.shipName,
                freight: this.order.freight,

                orderDate: this.order.orderDate,
                requiredDate: new Date(
                    new Date(this.order.orderDate).setMonth(
                        new Date(this.order.orderDate).getMonth() + 1
                    )
                ).toISOString(),

                shippingAddress: this.order.shipAddress,
                city: this.order.shipCity,
                region: this.order.shipRegion,
                country: this.order.shipCountry,
                postalCode: this.order.shipPostalCode,

                status: 0,
                latitude: this.order.latitude,
                longitude: this.order.longitude,
                
                products: this.products.map(p => ({
                    productId: p.productId,
                    quantity: p.quantity,
                    discount: p.discount
                }))
            }
             try {
                await api.post('/Order', payload)

                this.$q.notify({
                    type: 'positive',
                    message: 'Order created successfully'
                })

                this.resetForm()
                this.$nextTick(() => {
                    this.$refs.orderForm.resetValidation()
                })

                this.$router.push('/orders')
             } catch (error) {
                this.$q.notify({
                    type: 'negative',
                    message: error.response?.data || 'Error creating order'
                })
             }
        },
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
                this.order.shipName = customer.companyName

            } catch (error) {
                console.error('Error setting customer:', error);
            }
        },
        async loadShippers() {
            try {
                const res = await api.get('/Shippers/shippers')

                this.shippers = res.data.map(s => ({
                    label: s.companyName,
                    value: s.shipperId
                }))

            } catch (error) {
                console.error(error)
            }
        },
        async loadProducts() {
            try {
                const res = await api.get('/Product/available')
                this.availableProducts = res.data
            } catch (error) {
                console.error(error)
            }
        },
        handleAddProduct(product) {
            if (this.products.some(p => p.productId === product.productId)) {
                this.$q.notify({
                    type: "warning",
                    message: "Product already added"
                })
                return
            }

            this.products.push({
                productId: product.productId,
                productName: product.productName,
                quantityPerUnit: product.quantityPerUnit,
                unitPrice: product.unitPrice,
                quantity: 1,
                discount: 0,
                unitsInStock: product.unitsInStock
            })
        }, removeProduct(product) {
            this.products = this.products.filter(
                p => p.productId !== product.productId
            )
        },
        async validateAddress() {
            try {

                const payload = {
                    address: this.order.shipAddress,
                    city: this.order.shipCity,
                    region: this.order.shipRegion,
                    country: this.order.shipCountry,
                    postalCode: this.order.shipPostalCode
                }

                const res = await api.post('GoogleAddress/validate', payload)

                if (!res.data.isValid) {
                    this.$q.notify({
                        type: 'negative',
                        message: 'Invalid shipping address'
                    })

                    return false
                }

                this.order.latitude = res.data.latitude
                this.order.longitude = res.data.longitude

                this.renderMap()

                this.$q.notify({
                    type: 'positive',
                    message: 'Address validated successfully'
                })

                return true

            } catch (error) {

                this.$q.notify({
                    type: 'negative',
                    message: 'Error validating address'
                })

                return false
            }
        },
        renderMap() {

            if (!this.order.latitude || !this.order.longitude)
                return

            const map = new google.maps.Map(
                document.getElementById("map"),
                {
                    center: {
                        lat: Number(this.order.latitude),
                        lng: Number(this.order.longitude)
                    },
                    zoom: 14
                }
            )

            new google.maps.Marker({
                position: {
                    lat: Number(this.order.latitude),
                    lng: Number(this.order.longitude)
                },
                map
            })
        },
        resetForm() {

            this.products = []

            this.order = {
                customerId: '',
                customerName: '',
                employeeId: null,
                shipperId: null,

                orderDate: new Date().toISOString(),
                requiredDate: null,

                shipAddress: '',
                shipCity: '',
                shipRegion: '',
                shipPostalCode: '',
                shipCountry: '',

                latitude: null,
                longitude: null
            }

            const mapElement = document.getElementById("map")

            if (mapElement) {
                mapElement.innerHTML = ""
            }
        }
    },
    mounted() {
        this.loadProducts()
        this.loadShippers()
    }, computed: {
        formattedOrderDate() {
        return new Date(this.order.orderDate).toLocaleDateString()
    }
    }
}
</script>