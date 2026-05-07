<template>
  <q-layout view="lHh Lpr lFf">

    
    <q-header elevated class="bg-primary text-white">
      <q-toolbar>

        
        <q-toolbar-title>
          RSM Order Management
        </q-toolbar-title>

        
        <q-space />

        
        <q-btn flat dense color="white" icon="dashboard" label="Dashboard" to="/dashboard" />
        <q-btn flat dense color="white" icon="list" label="Orders" to="/orders" />
        <q-btn flat dense color="white" icon="inventory" label="Inventory" to="/inventory" />
        <q-btn flat dense color="white" icon="report" label="Orders Report" to="/ordersReport" />

      </q-toolbar>
    </q-header>

    
    <q-page-container>
      <router-view />
    </q-page-container>

  </q-layout>
</template>

<script>
export default {
  name: 'MainLayout'
}
</script>