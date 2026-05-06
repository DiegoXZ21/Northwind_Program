<template>
  <q-table
    :rows="products"
    :columns="columns"
    row-key="productId"
    flat
    bordered
  >
    <template v-slot:body-cell-quantity="props">
      <q-td :props="props">
        <div v-if="isEditing">
          <q-input
            v-model.number="props.row.quantity"
            type="number"
            dense
            min="1"
            :max="props.row.unitsInStock"
            :disable="props.row.discontinued"
            @blur="validateStock(props.row)"
            />
        </div>
        <div v-else>
          {{ props.row.quantity }}
        </div>
      </q-td>
    </template>

    <template v-slot:body-cell-discount="props">
      <q-td :props="props">
        <div v-if="isEditing">
          <q-input
            v-model.number="props.row.discount"
            type="number"
            dense
            min="0"
            max="0.9"
            step="0.05"
            suffix="%"
            :disable="props.row.discontinued"
          />
        </div>
        <div v-else>
            {{ props.row.discontinued 
                ? (props.row.discount * 100).toFixed(0) + "% (discontinued)" 
                : (props.row.discount * 100).toFixed(0) + "%" }}
        </div>
      </q-td>
    </template>

    <template v-slot:body-cell-actions="props">
      <q-td :props="props">
        <q-btn
          v-if="isEditing && !props.row.discontinued"
          icon="delete"
          color="negative"
          flat
          dense
          @click="$emit('remove', props.row)"
        />
      </q-td>
    </template>
  </q-table>
</template>

<script>
export default {
  props: {
    products: Array,
    isEditing: Boolean
  },
  data() {
    return {
      columns: [
        { name: 'productName', label: 'Product', field: 'productName', align: 'left' },
        { name: 'quantityPerUnit', label: 'Qty/Unit', field: 'quantityPerUnit', align: 'center' },
        { name: 'unitPrice', label: 'Price', field: row => Number(row.unitPrice).toFixed(2), align: 'right' },
        { name: 'quantity', label: 'Qty', field: 'quantity', align: 'center' },
        { name: 'discount', label: 'Discount', field: row => (row.discount * 100) + "%", align: 'center' },
        {
          name: 'subtotal',
          label: 'Subtotal',
          align: 'right',
          field: row => (row.unitPrice * row.quantity * (1 - row.discount)).toFixed(2)
        },
        { name: 'actions', label: '', align: 'center' }
      ]
    }
  }
}
</script>