<template>
    <Line :data="chartData" :options="chartOptions" />
</template>

<script>
import {
    Chart as ChartJS,
    LineElement,
    PointElement,
    CategoryScale,
    LinearScale,
    Tooltip,
    Legend,
    plugins
} from 'chart.js'

import { Line } from 'vue-chartjs'

ChartJS.register(
    LineElement,
    PointElement,
    CategoryScale,
    LinearScale,
    Tooltip,
    Legend
)

export default {
    components: { Line },

    props: {
        labels: Array,
        values: Array
    },
    computed: {
        chartData() {
            return {
                labels: this.labels,
                datasets: [
                    {
                        label: 'Orders',
                        data: this.values,
                        tension: 0.4,
                        fill: true
                    }
                ]
            }
        }
    },
    chartOptions(){
        return {
            responsive: true,
            plugins: {
                legend: { display: false }
            }
        }
    }
}

</script>