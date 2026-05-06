<template>
  <q-card class="q-pa-md q-mb-md">
    <div class="text-h6 q-mb-md">Shipping Information</div>

    <div class="row q-col-gutter-md">

      <div class="col-12 col-md-6">
        <q-input
          v-model="localModel.shippingAddress"
          label="Address"
          dense
          :readonly="readonly"
        />
      </div>

      <div class="col-12 col-md-6">
        <q-input
          v-model="localModel.city"
          label="City"
          dense
          :readonly="readonly"
        />
      </div>

      <div class="col-12 col-md-4">
        <q-input
          v-model="localModel.region"
          label="Region"
          dense
          :readonly="readonly"
        />
      </div>

      <div class="col-12 col-md-4">
        <q-input
          v-model="localModel.country"
          label="Country"
          dense
          :readonly="readonly"
        />
      </div>

      <div class="col-12 col-md-4">
        <q-input
          v-model="localModel.postalCode"
          label="Postal Code"
          dense
          :readonly="readonly"
        />
      </div>

      <div class="col-12 col-md-6">
        <q-select
          v-model="localModel.shipperId"
          :options="shippers"
          label="Shipper"
          emit-value
          map-options
          dense
          :disable="readonly"
        />
      </div>

      <div class="col-12">
        <q-banner
          v-if="formattedAddress"
          class="bg-grey-2 text-dark q-mt-md"
        >
          <div class="text-caption">Validated Address</div>
          <div class="text-body2">
            {{ formattedAddress }}
          </div>
        </q-banner>
      </div>
    </div>

    <q-btn
      class="q-mt-md"
      color="primary"
      label="Validate Address"
      @click="validateAddress"
      :loading="loading"
    />

    <div v-if="lat && lng" class="q-mt-md text-caption">
      📍 Lat: {{ lat }} | Lng: {{ lng }}
    </div>

    <div id="map" class="q-mt-md" style="height: 500px;" v-show="lat && lng"></div>
  </q-card>
</template>

<script>
import api from '../boot/axios'

export default {
  props: {
    modelValue: {
      type: Object,
      default: () => ({
        shippingAddress: '',
        city: '',
        region: '',
        country: '',
        postalCode: '',
        shipperId: null
      })
    },
    shippers: Array,
    readonly: {
      type: Boolean,
      default: false
    }
  },

  emits: ['update:modelValue'],

  data() {
    return {
      loading: false,
      lat: null,
      lng: null,
      map: null,
      marker: null,
      formattedAddress: null
    }
  },

  computed: {
    localModel: {
      get() {
        return this.modelValue
      },
      set(val) {
        this.$emit('update:modelValue', val)
      }
    }
  },

  methods: {
    async validateAddress() {
      this.loading = true

      try {
        const res = await api.post('/GoogleAddress/validate', {
          address: this.localModel.shippingAddress,
          city: this.localModel.city,
          region: this.localModel.region,
          country: this.localModel.country,
          postalCode: this.localModel.postalCode
        })

        const data = res.data

        if (!data.isValid) {
          this.$q.notify({
            type: 'negative',
            message: 'Invalid address'
          })
          return
        }

        this.lat = data.latitude
        this.lng = data.longitude
        this.formattedAddress = data.formattedAddress

        this.loadMap()

        this.$q.notify({
          type: 'positive',
          message: 'Address validated'
        })

      } catch (err) {
        this.$q.notify({
          type: 'negative',
          message: 'Error validating address'
        })
      } finally {
        this.loading = false
      }
    },

    loadMap() {
      const waitGoogle = () =>
        new Promise((resolve) => {
          const check = setInterval(() => {
            if (window.google && window.google.maps) {
              clearInterval(check)
              resolve()
            }
          }, 100)
        })

      waitGoogle().then(() => {
        const position = { lat: this.lat, lng: this.lng }

        this.map = new google.maps.Map(document.getElementById('map'), {
          center: position,
          zoom: 15
        })

        this.marker = new google.maps.Marker({
          position,
          map: this.map
        })
      })
    }
  }
}
</script>