<template>
  <q-layout view="lHh Lpr lFf">
    <q-header elevated>
        <q-toolbar>
            <q-toolbar-title>RSM Order Management</q-toolbar-title>
            <q-space />
            <q-btn flat dense icon="dashboard" label="Dashboard" tag="router-link" to="/dashboard" />
            <q-btn flat dense icon="list" label="Orders" tag="router-link" to="/orders" />
            <q-btn flat dense icon="bar_chart" label="Reports" tag="router-link" to="/reports" />
            <q-btn flat dense icon="people" label="Customers" tag="router-link" to="/customers" />
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
