import { createApp } from 'vue'
import { Quasar, Dialog, Notify } from 'quasar'
import App from './App.vue'
import router from './router'

import 'quasar/dist/quasar.css'
import '@quasar/extras/material-icons/material-icons.css'

const app = createApp(App)

app.use(Quasar, { 
    plugins: {
        Dialog,
        Notify
    }
 })
app.use(router)

app.mount('#app')