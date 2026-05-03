<template>
  <q-page padding>
    <q-card class="q-pa-md">
      <div class="text-h6 q-mb-md">Inventory</div>

      <div class="row q-col-gutter-md q-mb-md">
        <div class="col">
          <q-input v-model="filters.name" label="Product Name" outlined dense clearable/>
        </div>
        <div class="col">
          <q-input v-model="filters.category" label="Category" outlined dense clearable/>
        </div>
        <div class="col-auto flex flex-center">
          <q-btn label="Clear" color="negative" @click="clearFilters" />
        </div>
      </div>

      <q-table :rows="products" :columns="columns" row-key="productId" :loading="loading" loading-label="Loading products...">

        <template v-slot:body-cell-actions="props">
            <q-td align="center">
              <q-btn label="Update Stock" size="sm" color="primary" @click="openStockModal(props.row)"/>
            </q-td>
        </template>

        <template v-slot:no-data>
          <div class="full-width text-center q-pa-md">
            <q-icon name="search_off" size="40px" class="q-mb-sm" />
            <div class="text-subtitle1">No products found</div>
            <div class="text-caption text-grey">Try adjusting your filters or clear them to see all products.</div>
          </div>
        </template>
      </q-table>
    </q-card>

    <update-stock-modal 
      v-model="showModal"
      :product="selectedProduct"
      @updated="loadInventory"
    />
  </q-page>
</template>
<script>
import api from '../boot/axios'
import UpdateStockModal from '../components/UpdateStockModal.vue'
import { debounce, Loading } from 'quasar'

export default {
  components: { UpdateStockModal },
  
  data(){
    return {
      products: [],
      loading: false,
      selectedProduct: null,
      showModal: false,

      filters: {
        name: '',
        category: ''
      },

      columns: [
        {name: 'productName', label: 'Product', field: 'productName', align: 'center'},
        {name: 'unitsInStock', label: 'Units In Stock', field: 'unitsInStock', align: 'center'},
        {name: 'categoryName', label: 'Category', field: 'categoryName', align: 'center'},
        {name: 'actions', label: 'Actions', field: 'actions', align: 'center'}
      ]
    }
  },
  watch: {
    filters: {
      handler() {
        this.debounceSearch()
      },
      deep: true
    }
  },
  created() {
    this.debounceSearch = debounce (() => {
      this.loadInventory()
    }, 500)
  }, 
  mounted() {
    this.loadInventory()
  },

  methods: {
    async loadInventory() {
      this.loading = true
      try {
        const response = await api.get('/Product/inventory', {
          params: this.filters
        })
        this.products = response.data
      } catch (error) {
        console.error('Error loading inventory:', error)
      } finally {
        this.loading = false
      }
    },
    openStockModal(product) {
      this.selectedProduct = product
      this.showModal = true
    },
    clearFilters() {
      this.filters = {
        name: '',
        category: ''
      }
      this.loadInventory()
    }
  }
}
</script>