<template>
  <div class="chart-cfg">
    <div class="cc-header">
      <span class="cc-label">Diagram i PDF</span>
      <span class="cc-note">Påverkar Chart.js som renderas av Chromium</span>
    </div>

    <!-- Chart type -->
    <div class="cc-section">
      <div class="cc-sub">Typ</div>
      <div class="ct-grid">
        <button
          v-for="ct in chartTypes"
          :key="ct.id"
          :class="['ct-btn', { active: store.chartConfig.type === ct.id }]"
          @click="store.updateChartConfig({ type: ct.id as ChartConfig['type'] })"
        >
          <span class="ct-icon">{{ ct.icon }}</span>
          <span class="ct-label">{{ ct.label }}</span>
        </button>
      </div>
    </div>

    <!-- Question count -->
    <div class="cc-section">
      <div class="cc-sub">
        Antal frågor i diagrammet
        <span class="cc-note-inline">(första {{ store.chartConfig.questionCount }} i exportordningen)</span>
      </div>
      <div class="cc-stepper">
        <button
          class="step-btn"
          @click="decrement"
          :disabled="store.chartConfig.questionCount <= 1"
        >−</button>
        <span class="step-val">{{ store.chartConfig.questionCount }}</span>
        <button
          class="step-btn"
          @click="increment"
          :disabled="store.chartConfig.questionCount >= maxCount"
        >+</button>
        <span class="step-max">/ {{ maxCount }} valda</span>
      </div>
    </div>

    <!-- Live preview -->
    <div class="cc-section">
      <div class="cc-sub">Förhandsgranskning</div>
      <div class="cc-canvas-wrap">
        <canvas ref="canvasRef" height="180"></canvas>
        <div v-if="!hasQuestions" class="cc-empty">
          Välj frågor i trädet till vänster
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted, onUnmounted, nextTick } from 'vue'
import { useReportBuilderStore } from '../stores/reportBuilder'
import type { ChartConfig } from '../stores/reportBuilder'
import Chart from 'chart.js/auto'

const store = useReportBuilderStore()
const canvasRef = ref<HTMLCanvasElement>()
let chartInst: Chart | null = null

const chartTypes = [
  { id: 'bar',      icon: '📊', label: 'Stapel'   },
  { id: 'line',     icon: '📈', label: 'Linje'    },
  { id: 'radar',    icon: '🎯', label: 'Radar'    },
  { id: 'doughnut', icon: '🍩', label: 'Munk'     },
] as const

const hasQuestions = computed(() => store.selectedCount > 0)
const maxCount = computed(() => Math.min(store.selectedCount, 10))

// The questions that will actually feed the chart
const chartQuestions = computed(() =>
  store.orderedSelectedQuestions.slice(0, store.chartConfig.questionCount)
)

function scoreColor(v: number) {
  return v >= 4 ? '#22c55e' : v >= 3 ? '#f59e0b' : '#ef4444'
}

function increment() {
  if (store.chartConfig.questionCount < maxCount.value) {
    store.updateChartConfig({ questionCount: store.chartConfig.questionCount + 1 })
  }
}

function decrement() {
  if (store.chartConfig.questionCount > 1) {
    store.updateChartConfig({ questionCount: store.chartConfig.questionCount - 1 })
  }
}

function buildChart() {
  if (!canvasRef.value || !hasQuestions.value) return
  if (chartInst) { chartInst.destroy(); chartInst = null }

  const qs = chartQuestions.value
  // Use index as label since we may not have real avg values — placeholder 3.5
  const labels = qs.map((_, i) => `Q${i + 1}`)
  const values = qs.map(() => 2.5 + Math.random() * 2.5) // representative preview data
  const colors = values.map(v => scoreColor(v))
  const isRound = store.chartConfig.type === 'doughnut'

  chartInst = new Chart(canvasRef.value, {
    type: store.chartConfig.type,
    data: {
      labels,
      datasets: [{
        data: values,
        backgroundColor: isRound
          ? ['#a855f7','#7c3aed','#6d28d9','#5b21b6','#4c1d95','#6366f1','#818cf8','#a5b4fc','#c7d2fe','#e0e7ff']
          : colors.map(c => c + 'cc'),
        borderColor: isRound ? 'transparent' : colors,
        borderWidth: 2,
        borderRadius: store.chartConfig.type === 'bar' ? 4 : undefined,
        fill: store.chartConfig.type === 'line',
        tension: 0.4,
        pointBackgroundColor: '#a855f7',
      }],
    },
    options: {
      animation: { duration: 250 },
      plugins: {
        legend: { display: isRound, labels: { color: '#94a3b8', font: { size: 9 }, boxWidth: 10 } },
      },
      scales: store.chartConfig.type === 'radar'
        ? {
            r: {
              min: 0, max: 5,
              ticks: { color: '#475569', stepSize: 1, font: { size: 8 } },
              grid: { color: '#1e293b' },
              pointLabels: { color: '#94a3b8', font: { size: 8 } },
            },
          }
        : isRound ? {}
        : {
            y: { min: 0, max: 5, ticks: { color: '#888', font: { size: 9 } }, grid: { color: '#1e2a3a' } },
            x: { ticks: { color: '#888', font: { size: 9 } }, grid: { display: false } },
          },
    },
  })
}

// Rebuild when type or question selection/order changes
watch(
  [() => store.chartConfig.type, () => store.chartConfig.questionCount, () => store.orderedSelectedIds.slice()],
  () => nextTick(buildChart),
  { deep: true }
)

onMounted(() => nextTick(buildChart))
onUnmounted(() => { if (chartInst) chartInst.destroy() })
</script>

<style scoped>
.chart-cfg {
  display: flex;
  flex-direction: column;
  gap: 0;
}

.cc-header {
  display: flex;
  flex-direction: column;
  gap: 2px;
  padding: 12px 14px;
  background: var(--surface-2, #334155);
  border-bottom: 1px solid var(--border, #475569);
}

.cc-label {
  font-size: 11px;
  font-weight: 700;
  color: var(--text, #f1f5f9);
}

.cc-note {
  font-size: 10px;
  color: var(--text-muted, #64748b);
}

.cc-section {
  padding: 12px 14px;
  border-bottom: 1px solid var(--border, #334155);
}
.cc-section:last-child { border-bottom: none; }

.cc-sub {
  font-size: 10px;
  font-weight: 600;
  color: var(--text-muted, #94a3b8);
  text-transform: uppercase;
  letter-spacing: 0.5px;
  margin-bottom: 8px;
}

.cc-note-inline {
  font-weight: 400;
  text-transform: none;
  letter-spacing: 0;
  color: var(--text-muted, #64748b);
  font-size: 10px;
}

.ct-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 5px;
}

.ct-btn {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 3px;
  padding: 8px 4px;
  border-radius: 7px;
  border: 1px solid var(--border, #475569);
  background: transparent;
  cursor: pointer;
  color: var(--text-muted, #94a3b8);
  font-family: inherit;
  transition: all 0.12s;
}
.ct-btn:hover { border-color: var(--accent, #a855f7); color: var(--text, #f1f5f9); }
.ct-btn.active {
  border-color: var(--accent, #a855f7);
  background: color-mix(in srgb, var(--accent, #a855f7) 12%, transparent);
  color: var(--accent, #a855f7);
}

.ct-icon  { font-size: 16px; }
.ct-label { font-size: 9px; font-weight: 500; }

.cc-stepper {
  display: flex;
  align-items: center;
  gap: 8px;
}

.step-btn {
  width: 26px;
  height: 26px;
  border-radius: 6px;
  border: 1px solid var(--border, #475569);
  background: transparent;
  color: var(--text, #f1f5f9);
  cursor: pointer;
  font-size: 16px;
  line-height: 1;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.12s;
  font-family: inherit;
}
.step-btn:hover:not(:disabled) { border-color: var(--accent, #a855f7); color: var(--accent, #a855f7); }
.step-btn:disabled { opacity: 0.35; cursor: not-allowed; }

.step-val {
  font-size: 18px;
  font-weight: 700;
  color: var(--text, #f1f5f9);
  width: 28px;
  text-align: center;
  font-variant-numeric: tabular-nums;
}

.step-max {
  font-size: 10px;
  color: var(--text-muted, #64748b);
}

.cc-canvas-wrap {
  position: relative;
  background: #050d1e;
  border-radius: 8px;
  padding: 8px;
  min-height: 196px;
}

.cc-empty {
  position: absolute;
  inset: 0;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 11px;
  color: var(--text-muted, #64748b);
}
</style>
