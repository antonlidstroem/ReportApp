<template>
  <div class="stress-panel">
    <div class="sp-header">
      <div class="sp-title-group">
        <span class="sp-eyebrow">PERFORMANCE BENCHMARK</span>
        <h3>Stress Test — 500 Questions × 200 Responses</h3>
        <p>Exportera enkät #4 (100 000 rader) och mät genereringstid.</p>
      </div>
      <div class="sp-meta">
        <div class="sm-stat"><span class="sm-val">500</span><span class="sm-lbl">Questions</span></div>
        <div class="sm-stat"><span class="sm-val">100k</span><span class="sm-lbl">Responses</span></div>
        <div class="sm-stat"><span class="sm-val">3</span><span class="sm-lbl">Formats</span></div>
      </div>
    </div>
    <div class="sp-controls">
      <div class="format-buttons">
        <button v-for="fmt in formats" :key="fmt.id" class="fmt-btn"
          :class="{ active: selectedFormats.includes(fmt.id), running: runningFormat === fmt.id }"
          @click="toggleFormat(fmt.id)">
          <span>{{ fmt.icon }}</span><span>{{ fmt.label }}</span>
        </button>
      </div>
      <button class="run-btn" :class="{ running: isRunning }"
        :disabled="isRunning || selectedFormats.length === 0" @click="runStressTest">
        <span v-if="!isRunning">⚡ Run Benchmark</span>
        <span v-else><span class="spinner"></span> {{ runningFormat?.toUpperCase() }}…</span>
      </button>
    </div>
    <div v-if="results.length > 0" class="sp-results">
      <div v-for="r in results" :key="r.format" class="result-row">
        <div class="rr-format">{{ getFormatIcon(r.format) }} {{ r.format.toUpperCase() }}</div>
        <div class="rr-bar-container">
          <div class="rr-bar" :style="`width:${barWidth(r.ms)}%;background:${providerColor}`"></div>
          <span class="rr-time" :class="speedClass(r.ms)">{{ r.ms }}ms</span>
        </div>
        <div class="rr-size">{{ formatSize(r.bytes) }}</div>
      </div>
    </div>
    <div v-if="error" class="sp-error">⚠ {{ error }}</div>
  </div>
</template>
<script setup lang="ts">
import { ref, computed } from 'vue'
import { useBenchmarkStore } from '../stores/benchmarks'
import { exportReport } from '../composables/useApi'
const props = defineProps<{ provider: string; providerColor: string }>()
const benchmarks = useBenchmarkStore()
const STRESS_SURVEY_ID = 4
const formats = [
  { id: 'pdf', label: 'PDF', icon: '📕' },
  { id: 'excel', label: 'Excel', icon: '📗' },
  { id: 'ppt', label: 'PowerPoint', icon: '📘' },
]
const selectedFormats = ref<string[]>(['pdf', 'excel', 'ppt'])
const isRunning = ref(false)
const runningFormat = ref<string | null>(null)
const results = ref<{ format: string; ms: number; bytes: number }[]>([])
const error = ref<string | null>(null)
function toggleFormat(id: string) {
  if (selectedFormats.value.includes(id)) selectedFormats.value = selectedFormats.value.filter(f => f !== id)
  else selectedFormats.value.push(id)
}
function getFormatIcon(fmt: string) { return formats.find(f => f.id === fmt)?.icon ?? '📄' }
async function runStressTest() {
  if (isRunning.value) return
  isRunning.value = true; error.value = null; results.value = []
  for (const fmt of selectedFormats.value) {
    runningFormat.value = fmt
    try {
      const result = await exportReport({ provider: props.provider, format: fmt, surveyId: STRESS_SURVEY_ID })
      const r = { format: fmt, ms: result.generationMs, bytes: result.fileSizeBytes || result.blob.size }
      results.value.push(r)
      benchmarks.add({ provider: props.provider, format: fmt, surveyId: STRESS_SURVEY_ID, surveyTitle: '⚡ Stresstest', generationMs: r.ms, fileSizeBytes: r.bytes, timestamp: new Date().toISOString(), questionCount: 500 })
    } catch (e) { error.value = `${fmt.toUpperCase()} failed: ${e instanceof Error ? e.message : 'Error'}` }
  }
  runningFormat.value = null; isRunning.value = false
}
const maxMs = computed(() => results.value.length ? Math.max(...results.value.map(r => r.ms)) : 1)
function barWidth(ms: number) { return Math.min(100, (ms / maxMs.value) * 100) }
function speedClass(ms: number) { return ms < 500 ? 'fast' : ms < 2000 ? 'ok' : 'slow' }
function formatSize(bytes: number) { if (!bytes) return '—'; if (bytes > 1_000_000) return `${(bytes/1_000_000).toFixed(1)} MB`; return `${(bytes/1024).toFixed(0)} KB` }
</script>
<style scoped>
.stress-panel{background:#060e1a;border:1px solid rgba(255,255,255,.06);border-radius:16px;overflow:hidden;margin-bottom:32px}
.sp-header{display:flex;justify-content:space-between;align-items:flex-start;padding:28px 32px 20px;background:linear-gradient(135deg,#0a1628,#060e1a);border-bottom:1px solid rgba(255,255,255,.05);gap:24px}
.sp-eyebrow{font-size:10px;letter-spacing:3px;text-transform:uppercase;color:#f59e0b;display:block;margin-bottom:6px;font-weight:700}
.sp-title-group h3{font-size:18px;font-weight:700;color:#fff;margin:0 0 6px}
.sp-title-group p{font-size:13px;color:#64748b;margin:0}
.sp-meta{display:flex;gap:16px;flex-shrink:0}
.sm-stat{text-align:center;background:rgba(255,255,255,.04);border:1px solid rgba(255,255,255,.06);border-radius:10px;padding:12px 16px}
.sm-val{display:block;font-size:22px;font-weight:800;color:#f59e0b}
.sm-lbl{font-size:10px;color:#475569;text-transform:uppercase;letter-spacing:1px}
.sp-controls{display:flex;align-items:center;gap:16px;padding:20px 32px;border-bottom:1px solid rgba(255,255,255,.04);flex-wrap:wrap}
.format-buttons{display:flex;gap:8px}
.fmt-btn{display:flex;align-items:center;gap:6px;padding:8px 16px;border-radius:8px;border:1px solid rgba(255,255,255,.1);background:transparent;color:#64748b;cursor:pointer;font-size:13px;font-weight:500;transition:all .15s}
.fmt-btn:hover{border-color:rgba(255,255,255,.2);color:#94a3b8}
.fmt-btn.active{border-color:#f59e0b;color:#f59e0b;background:rgba(245,158,11,.08)}
.run-btn{margin-left:auto;padding:10px 28px;border-radius:8px;border:none;background:#f59e0b;color:#000;font-size:14px;font-weight:700;cursor:pointer;transition:all .15s;min-width:160px;display:flex;align-items:center;justify-content:center;gap:8px}
.run-btn:hover:not(:disabled){background:#fbbf24}
.run-btn:disabled{opacity:.5;cursor:not-allowed}
.run-btn.running{background:#374151;color:#9ca3af;cursor:wait}
.spinner{width:14px;height:14px;border:2px solid rgba(255,255,255,.2);border-top-color:#f59e0b;border-radius:50%;animation:spin .7s linear infinite}
@keyframes spin{to{transform:rotate(360deg)}}
.sp-results{padding:20px 32px;display:flex;flex-direction:column;gap:12px}
.result-row{display:flex;align-items:center;gap:16px}
.rr-format{font-size:12px;font-weight:700;color:#94a3b8;width:100px;flex-shrink:0}
.rr-bar-container{flex:1;position:relative;height:32px;background:rgba(255,255,255,.04);border-radius:6px;overflow:hidden;display:flex;align-items:center}
.rr-bar{position:absolute;left:0;top:0;height:100%;border-radius:6px;opacity:.7}
.rr-time{position:relative;z-index:1;font-size:14px;font-weight:800;padding-left:12px}
.rr-time.fast{color:#22c55e}.rr-time.ok{color:#f59e0b}.rr-time.slow{color:#ef4444}
.rr-size{width:60px;text-align:right;font-size:11px;color:#475569;flex-shrink:0}
.sp-error{margin:0 32px 20px;padding:12px 16px;background:rgba(239,68,68,.08);border:1px solid rgba(239,68,68,.3);border-radius:8px;font-size:13px;color:#ef4444}
</style>
