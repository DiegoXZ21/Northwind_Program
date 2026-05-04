<template>
    <Pie :data="chartData" :options="chartOptions" />
</template>

<script>
import {
    Chart as ChartJS,
    ArcElement,
    Tooltip,
    Legend
} from 'chart.js'

import { Pie } from 'vue-chartjs'

ChartJS.register(ArcElement, Tooltip, Legend)

export default {
    components: { Pie },

    props: {
        labels: Array,
        values: Array
    },

    computed: {
        chartData() {
            return {
                labels: this.labels,
                datasets: [{
                    label: 'Orders',
                    data: this.values,
                    backgroundColor: [
                        '#4caf50',
                        '#2196f3',
                        '#ff9800',
                        '#e91e63',
                        '#9c27b0',
                        '#00bcd4',
                        '#ffc107',
                        '#8bc34a',
                        '#ff5722',
                        '#607d8b'
                    ]
                }]
            }
        }
    },

    chartOptions() {
        return {
            responsive: true,
            plugins: {
                legend: { 
                    position: 'right'
                }
            }
        }
    }
}
</script>