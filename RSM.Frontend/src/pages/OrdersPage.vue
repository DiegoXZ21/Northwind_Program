<template>
    <q-page padding>
        <h4>Orders</h4>
        <order-form />
    </q-page>
</template>

<script>
import OrderForm from '../components/OrderForm.vue';
export default { components: { OrderForm } }
</script>