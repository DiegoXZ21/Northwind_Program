<template>
  <q-page padding>
    <div class="text-h5 q-mb-md">
      Order Detail #{{ order?.orderId }}
    </div>

    <div class="q-mb-md">
      <q-btn
        color="primary"
        icon="picture_as_pdf"
        label="Export PDF"
        class="q-mr-sm"
        @click="exportPDF"
        :disable="!order"
      />

      <q-btn
        color="secondary"
        icon="table_view"
        label="Export Excel"
        @click="exportExcel"
        :disable="!order"
      />

      <q-btn
        color="negative"
        label="Delete Order"
        icon="delete"
        class="q-ml-sm"
        v-if="canDelete"
        @click="confirmDelete"
      />
    </div>

    <div v-if="order">
      <div class="q-mb-md">
        <q-select
          v-model="order.status"
          :options="statusOptions"
          label="Order Status"
          emit-value
          map-options
          dense
          :disable="!canEditStatus"
          @update:model-value="onStatusChange"
        />
      </div>
    </div>


    <div class="q-mb-md">
      <q-btn
        color="primary"
        label="Edit Shipping"
        icon="edit"
        v-if="canEditShipping && !isEditingShipping"
        @click="isEditingShipping = true"
      />
      <q-btn
        color="positive"
        label="Save Shipping"
        icon="save"
        class="q-ml-sm"
        v-if="isEditingShipping"
        @click="saveShipping"
      />
      <q-btn
        color="negative"
        label="Cancel"
        icon="close"
        class="q-ml-sm"
        v-if="isEditingShipping"
        @click="cancelShipping"
      />
    </div>

    <OrderShippingForm
      v-model="shipping"
      :shippers="shippers"
      :readonly="!isEditingShipping"
    />

    <div class="q-mb-md">
      <q-btn
        color="primary"
        label="Edit Products"
        icon="edit"
        v-if="canEditProducts  && !isEditingProducts"
        @click="isEditingProducts = true"
      />
      <q-btn
        color="positive"
        label="Save Products"
        icon="save"
        class="q-ml-sm"
        v-if="isEditingProducts"
        @click="saveProducts"
      />
      <q-btn
        color="negative"
        label="Cancel"
        icon="close"
        class="q-ml-sm"
        v-if="isEditingProducts"
        @click="cancelProducts"
      />
      <q-btn
        color="secondary"
        label="Add Product"
        icon="add"
        class="q-ml-sm"
        v-if="isEditingProducts && canEditProducts"
        @click="addProduct"
        :disable="!availableProducts.length"
      />
    </div>
    <q-card class="q-pa-md">

    <OrderProductsTable
      :products="order?.products || []"
      :isEditing="isEditingProducts"
      @remove="removeProduct"
    />
    </q-card>

    <div class="text-right text-h6 q-mt-md">
      Total: ${{ total }}
    </div>

    <ProductSelectDialog
      v-model="showProductDialog"
      :products="availableProducts"
      @add="handleAddProduct"
    />

  </q-page>
</template>

<script>
import api from '../boot/axios'
import OrderProductsTable from '../components/OrderProductsTable.vue'
import ProductSelectDialog from '../components/ProductSelectDialog.vue'
import OrderShippingForm from '../components/OrderShippingForm.vue'
import { generateOrderDetailPDF } from '../services/orderDetailReportService'
import { generateOrderDetailExcel } from '../services/orderDetailReportExcell'

export default {
  data() {
    return {
      order: null,
      previousStatus: null,
      shipping: {
        shippingAddress: '',
        city: '',
        region: '',
        country: '',
        postalCode: '',
        shipperId: null
      },
      loading: false,

      isEditingShipping: false,
      isEditingProducts: false,
      isEditingStatus: false,

      availableProducts: [],
      shippers: [],
      showProductDialog: false,

      statusOptions: [
        { label: "Pending", value: 0 },
        { label: "Processing", value: 1 },
        { label: "Shipped", value: 2 },
        { label: "Completed", value: 3 },
        { label: "Cancelled", value: 4 }
      ]
    }
  },

  components: {
    OrderProductsTable,
    ProductSelectDialog,
    OrderShippingForm
  },

  mounted() {
    this.loadOrder()
    this.loadProducts()
    this.loadShippers()
  },

  methods: {

    async loadOrder() {
      this.loading = true
      try {
        const id = this.$route.params.id
        const res = await api.get(`/OrderDetail/${id}`)
        const data = res.data

        this.previousStatus = data.status

        this.order = {
          orderId: data.orderId,
          customerName: data.customerName,
          orderDate: data.orderDate,
          status: data.status,
          latitude: data.latitude,
          longitude: data.longitude,
          products: (data.products ?? []).map(p => ({
            ...p,
            discount: Number(Number(p.discount).toFixed(2))
          }))
        }

        this.shipping = {
          shippingAddress: data.shippingAddress ?? '',
          city: data.city ?? '',
          region: data.region ?? '',
          country: data.country ?? '',
          postalCode: data.postalCode ?? '',
          shipperId: data.shipperId ?? null
        }

      } catch (error) {
        console.error(error)
      } finally {
        this.loading = false
      }
    },

    async loadProducts() {
      try {
        const res = await api.get("/Product/available")
        this.availableProducts = res.data
      } catch (error) {
        console.error(error)
      }
    },

    async loadShippers() {
      try {
        const res = await api.get('Order/shippers')
        this.shippers = res.data.map(s => ({
          label: s.companyName,
          value: s.shipperId
        }))
      } catch (err) {
        console.error(err)
      }
    },

    addProduct() {
      this.showProductDialog = true
    },

    handleAddProduct(product) {
      if (this.order.products.some(p => p.productId === product.productId)) {
        this.$q.notify({
          type: "warning",
          message: "Product already added"
        })
        return
      }

      this.order.products.push({
        productId: product.productId,
        productName: product.productName,
        quantityPerUnit: product.quantityPerUnit,
        unitPrice: product.unitPrice,
        quantity: 1,
        discount: 0,
        discontinued: false,
        unitsInStock: product.unitsInStock
      })
    },

    removeProduct(product) {
      this.order.products = this.order.products.filter(
        p => p.productId !== product.productId
      )
    },

    async saveProducts() {
      try {
        const invalidProduct = this.order.products.find(p =>
          p.quantity > (p.unitsInStock ?? 0)
        )

        if (invalidProduct) {
          this.$q.notify({
            type: "negative",
            message: `Insufficient stock for "${invalidProduct.productName}"`
          })
          return
        }

        const payload = this.buildPayload()

        await api.put(`/Order/${this.order.orderId}`, payload)

        this.$q.notify({ type: "positive", message: "Products updated" })

        this.isEditingProducts = false
        this.loadOrder()

      } catch (error) {
        this.$q.notify({
          type: "negative",
          message: error.response?.data || "Error updating products"
        })
      }
    },

    cancelProducts() {
      this.isEditingProducts = false
      this.loadOrder()
    },

    async saveShipping() {
      try {
        const payload = this.buildPayload()

        await api.put(`/Order/${this.order.orderId}`, payload)

        this.$q.notify({ type: "positive", message: "Shipping updated" })

        this.isEditingShipping = false
        this.loadOrder()

      } catch (error) {
        this.$q.notify({
          type: "negative",
          message: error.response?.data || "Error updating shipping"
        })
      }
    },

    cancelShipping() {
      this.isEditingShipping = false
      this.loadOrder()
    },

    async saveStatus() {
      try {
        const payload = this.buildPayload()

        await api.put(`/Order/${this.order.orderId}`, payload)

        this.$q.notify({ type: "positive", message: "Status updated" })

        this.isEditingStatus = false
        this.loadOrder()

      } catch (error) {
        this.$q.notify({
          type: "negative",
          message: error.response?.data || "Error updating status"
        })
      }
    },

    cancelStatus() {
      this.isEditingStatus = false
      this.loadOrder()
    },

    buildPayload() {
      return {
        orderId: this.order.orderId,
        status: this.order.status,

        shippingAddress: this.shipping.shippingAddress,
        city: this.shipping.city,
        region: this.shipping.region,
        country: this.shipping.country,
        postalCode: this.shipping.postalCode,
        shipperId: this.shipping.shipperId,

        latitude: this.order.latitude,
        longitude: this.order.longitude,

        products: this.order.products.map(p => ({
          productId: p.productId,
          quantity: p.quantity,
          discount: Number(Number(p.discount).toFixed(2))
        }))
      }
    },
    exportPDF() {
      generateOrderDetailPDF({
        ...this.order,
        ...this.shipping
      })
    },

    exportExcel() {
      generateOrderDetailExcel({
        ...this.order
      })
    },
    async deleteOrder() {
      try {
        await api.delete(`/Order/${this.order.orderId}`)

        this.$q.notify({
          type: "positive",
          message: "Order deleted successfully"
        })

        this.$router.push('/orders') // redirigir al listado

      } catch (error) {
        this.$q.notify({
          type: "negative",
          message: error.response?.data || "Error deleting order"
        })
      }
    },
    confirmDelete() {
      this.$q.dialog({
        title: 'Confirm',
        message: 'Are you sure you want to delete this order?',
        cancel: true,
        persistent: true
      }).onOk(() => {
        this.deleteOrder()
      })
    },
    async onStatusChange(newStatus) {
      const allowedTransitions = {
        0: [1, 4],
        1: [2, 4],
        2: [3],
        3: [],
        4: []
      }

      const currentStatus = this.previousStatus

      if(!allowedTransitions[currentStatus].includes(newStatus)){
        this.$q.notify({
          type: "warning",
          message: "Invalid status transition"
        })

        this.order.status = currentStatus
        return
      }

      try {
        await api.put(`/Order/${this.order.orderId}/status`, { status: newStatus })

        this.$q.notify({
          type: "positive",
          message: "Status updated"
        })

        this.previousStatus = newStatus

      } catch (error) {
        this.$q.notify({
          type: "negative",
          message: error.response?.data || "Error updating status"
        })

        this.order.status = currentStatus
      }
    }
  },

  computed: {
    total() {
      const products = this.order?.products ?? []
      return products
        .reduce((sum, p) =>
          sum + (p.unitPrice * p.quantity * (1 - p.discount)), 0
        )
        .toFixed(2)
    },
    canEditShipping() {
      if (!this.order) return false
      return [0, 1].includes(this.order.status)
    },
    canEditProducts() {
      if (!this.order) return false
      return [0, 1].includes(this.order.status)
    },
    canEditStatus() {
      if (!this.order) return false
      return [0, 1, 2].includes(this.order.status)
    },
    canDelete() {
      if (!this.order) return false
      return this.order.status === 0
    },
    canDelete() {
      if (!this.order) return false
      return this.order.status === 0
    }
  }
}
</script>