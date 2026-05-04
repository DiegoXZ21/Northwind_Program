<template>
  <q-page padding>
    <h4>Dashboard</h4>
    <div class="row q-col-gutter-md">
      <div class="col-12 col-md-6 flex">
        <q-card class="q-pa-md full-width column justify-between">
          <div class="row items-center q-mb-lg">
            <div class="text-subtitle1 col">Orders by Country</div>

            <div class="col-auto">
              <q-select 
                v-model="selectedYear"
                :options="years"
                label="Year"
                dense
                outlined
                emit-value
                map-options
                style="min-width: 120px;"
              />
            </div>
          </div>

          <div v-if="loadingChart" class="text-center q-pa-lg">
            <q-spinner-dots size="40px" />
          </div>
          <div v-else-if="countryLabels.length === 0" class="text-center q-pa-lg">
            No data for selected year
          </div>
          <div v-else class="col">
            <CountryChart 
              :labels="countryLabels"
              :values="countryValues"
            />
          </div>
          
        </q-card>
      </div>
      <div class="col-12 col-md-6 flex">
        <q-card class="q-pa-md full-width column justify-between">
          <div class="text-subtitle1 q-mb-md">
            Orders by Month ({{ selectedYear }})
          </div>

          <div v-if="loadingMonthly" class="text-center q-pa-lg">
            <q-spinner-dots size="40px" />
          </div>

          <div v-else-if="monthlyLabels.length === 0" class="text-center q-pa-lg">
            No data available
          </div>

          <div v-else class="col">
            <MonthlyChart 
              :labels="monthlyLabels"
              :values="monthlyValues"
            />
          </div>
        </q-card>
      </div>
    </div>
  </q-page>
</template>
<script>
import api from '../boot/axios'
import CountryChart from '../components/CountryChart.vue'
import MonthlyChart from '../components/MonthlyChart.vue';
export default {
  components: {
    CountryChart,
    MonthlyChart
  },
  
  data() {
    return {
      years: [],
      selectedYear: null,

      countryLabels: [],
      countryValues: [],

      loadingChart: false,

      monthlyLabels: [],
      monthlyValues: [],
      loadingMonthly: false
    }
  },

  mounted(){
    this.loadYears()
  },

  watch: {
    selectedYear(val) {
      if(val){
        this.loadCountryChart()
        this.loadMonthlyChart()
      }
    }
  },
  methods: {
    async loadYears(){
      try {
        const response = await api.get('/Order/years')

        this.years = response.data

        if (this.years.length > 0) {
          this.selectedYear = this.years[this.years.length - 1]
        }
      } catch (error) {
        console.error('Error loading years:', error)
      }
    },

    async loadCountryChart() {
      this.loadingChart = true
      try {
        const response = await api.get('/Order/countries-by-year', {
          params: { year: this.selectedYear }
        })

        const data = response.data
        const top = data.slice(0, 5)
        const rest = data.slice(5)

        const otherTotal = rest.reduce((sum, x) => sum + x.total, 0)

        this.countryLabels = top.map(x => x.country)
        this.countryValues = top.map(x => x.total)

        if (otherTotal > 0) {
          this.countryLabels.push('Others')
          this.countryValues.push(otherTotal)
        }

      } catch (error) {
        console.error('Error loading country chart:', error)
      } finally {
        this.loadingChart = false
      }
    },
    async loadMonthlyChart() {
      this.loadingMonthly = true

      try {
        const response = await api.get('/Order/monthly-orders', {
          params: { year: this.selectedYear }
        })

        const data = response.data

        const monthNames = [
          'Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun',
          'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'
        ]

        this.monthlyLabels = monthNames
        this.monthlyValues = new Array(12).fill(0)

        data.forEach(x => {
          this.monthlyValues[x.month - 1] = x.total
        });
      } catch (error) {
        console.error('Error loading monthly chart: ', error)
      } finally {
        this.loadingMonthly = false
      }
    }
  }
}
</script>