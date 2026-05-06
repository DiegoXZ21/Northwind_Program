<template>
  <q-dialog v-model="model">
    <q-card style="min-width: 400px">

      <q-card-section>
        <div class="text-h6">Select Product</div>
      </q-card-section>

      <q-card-section>
        <q-select
          v-model="selectedProduct"
          :options="filteredProducts"
          option-label="productName"
          option-value="productId"
          use-input
          input-debounce="300"
          @filter="filterProducts"
          emit-value
          map-options
          label="Search product"
        >
          <template v-slot:option="props">
            <q-item v-bind="props.itemProps">
              <q-item-section>
                <q-item-label>{{ props.opt.productName }}</q-item-label>
                <q-item-label caption>
                  Stock: {{ props.opt.unitsInStock }}
                </q-item-label>
              </q-item-section>
            </q-item>
          </template>
        </q-select>
      </q-card-section>

      <q-card-actions align="right">
        <q-btn flat label="Cancel" @click="closeDialog" />
        <q-btn color="primary" label="Add" @click="addProduct" />
      </q-card-actions>

    </q-card>
  </q-dialog>
</template>

<script>
export default {
  props: {
    modelValue: Boolean,
    products: Array
  },

  data() {
    return {
      selectedProduct: null,
      filteredProducts: []
    }
  },

  computed: {
    model: {
      get() {
        return this.modelValue
      },
      set(val) {
        this.$emit('update:modelValue', val)
      }
    }
  },

  watch: {
    products: {
      immediate: true,
      handler(val) {
        this.filteredProducts = val
      }
    }
  },

  methods: {
    filterProducts(val, update) {
      update(() => {
        const needle = val.toLowerCase()
        this.filteredProducts = this.products.filter(p =>
          p.productName.toLowerCase().includes(needle)
        )
      })
    },

    addProduct() {
      const product = this.products.find(p => p.productId === this.selectedProduct)

      if (!product) return

      this.$emit('add', product)

      this.selectedProduct = null
      this.filteredProducts = this.products
      this.closeDialog()
    },

    closeDialog() {
      this.selectedProduct = null
      this.filteredProducts = this.products
      this.model = false
    }
  }
}
</script>