<template>
  <div class="export-panel">
    <div class="ep-header">
      <h3>📤 Exportera rapport</h3>
    </div>

    <div class="format-grid">
      <button
        v-for="fmt in formats"
        :key="fmt.id"
        class="format-btn"
        :class="{ active: selectedFormat === fmt.id }"
        @click="selectedFormat = fmt.id"
      >
        <span class="fmt-icon">{{ fmt.icon }}</span>
        <span class="fmt-name">{{ fmt.label }}</span>
        <span class="fmt-best" v-if="bestTime(fmt.id)">
          ⚡ {{ bestTime(fmt.id) }}ms
        </span>
      </button>
    </div>

    <div class="export-actions">
      <button
        class="export-btn"
        :class="{ loading: isExporting }"
        :disabled="isExporting || !store.surveyId"
        @click="doExport"
      >
        <span v-if="!isExporting">
          {{ selectedFormat === 'pdf' && useTemplate ? '🎨 Exportera med mall' : '⬇ Exportera' }}
        </span>
        <span v-else class="loading-text">
          <span class="dot-pulse"></span> Genererar...
        </span>
      </button>

      <label v-if="supportsHtmlTemplate && selectedFormat === 'pdf'" class="template-toggle">
        <input type="checkbox" v-model="useTemplate" />
        Använd HTML-mall
      </label>
    </div>

    <!-- Result card -->
    <transition name="fade">
      <div v-if="lastResult" class="result-card" :class="lastResult.status">
        <div class="result-header">
          <span class="result-icon">{{ lastResult.status === 'ok' ? '✅' : '❌' }}</span>
          <span class="result-title">{{ lastResult.status === 'ok' ? 'Export klar' : 'Fel vid export' }}</span>
          <button class="dismiss" @click="lastResult = null">✕</button>
        </div>
        <div v-if="lastResult.status === 'ok'" class="result-stats">
          <div class="stat">
            <span class="stat-label">Tid</span>
            <span class="stat-val" :class="speedClass(lastResult.ms)">{{ lastResult.ms }} ms</span>
          </div>
          <div class="stat">
            <span class="stat-label">Storlek</span>
            <span class="stat-val">{{ formatSize(lastResult.bytes) }}</span>
          </div>
          <div class="stat">
            <span class="stat-label">Format</span>
            <span class="stat-val">{{ lastResult.format.toUpperCase() }}</span>
          </div>
        </div>
        <div v-else class="result-error">{{ lastResult.message }}</div>
      </div>
    </transition>

    <!-- No survey warning -->
    <div v-if="!store.surveyId" class="warning-hint">
      ⚠ Välj en enkät i survey-väljaren ovan för att kunna exportera.
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useReportBuilderStore } from '../stores/reportBuilder'
import { useBenchmarkStore } from '../stores/benchmarks'
import { exportReport, downloadBlob } from '../composables/useApi'

const props = defineProps<{
  provider: string
  supportsHtmlTemplate: boolean
}>()

const store     = useReportBuilderStore()
const benchmarks = useBenchmarkStore()

type Format = 'pdf' | 'excel' | 'ppt'

const formats = [
  { id: 'pdf'   as Format, label: 'PDF',        icon: '📕' },
  { id: 'excel' as Format, label: 'Excel',       icon: '📗' },
  { id: 'ppt'   as Format, label: 'PowerPoint',  icon: '📘' },
]

const selectedFormat = ref<Format>('pdf')
const useTemplate    = ref(false)
const isExporting    = ref(false)
const lastResult     = ref<{
  status: 'ok' | 'error'
  ms: number; bytes: number; format: string; message?: string
} | null>(null)

function bestTime(fmt: string) {
  const b = benchmarks.best(props.provider, fmt)
  return b?.generationMs
}

function speedClass(ms: number) {
  if (ms < 500) return 'fast'
  if (ms < 2000) return 'ok'
  return 'slow'
}

function formatSize(bytes: number) {
  if (bytes > 1_000_000) return `${(bytes / 1_000_000).toFixed(1)} MB`
  return `${(bytes / 1024).toFixed(0)} KB`
}

async function doExport() {
  if (!store.surveyId || isExporting.value) return
  isExporting.value = true
  lastResult.value = null

  try {
    const result = await exportReport({
      provider:    props.provider,
      format:      selectedFormat.value,
      surveyId:    store.surveyId,
      start:       store.dateStart || undefined,
      end:         store.dateEnd   || undefined,
      questionIds: store.selectedQuestionIds.length > 0 ? store.selectedQuestionIds : undefined,
      htmlTemplate: useTemplate.value && props.supportsHtmlTemplate && selectedFormat.value === 'pdf'
        ? store.templateHtml : undefined,
    })

    downloadBlob(result.blob, result.filename)

    lastResult.value = {
      status: 'ok',
      ms:     result.generationMs,
      bytes:  result.fileSizeBytes || result.blob.size,
      format: selectedFormat.value,
    }

    // Log to benchmark store
    const survey = store.surveys.find(s => s.id === store.surveyId)
    benchmarks.add({
      provider:       props.provider,
      format:         selectedFormat.value,
      surveyId:       store.surveyId!,
      surveyTitle:    survey?.title ?? '',
      generationMs:   result.generationMs,
      fileSizeBytes:  result.fileSizeBytes || result.blob.size,
      timestamp:      new Date().toISOString(),
      questionCount:  store.selectedQuestionIds.length || (survey?.questionCount ?? 0),
    })

  } catch (err: unknown) {
    lastResult.value = {
      status: 'error',
      ms: 0, bytes: 0, format: selectedFormat.value,
      message: err instanceof Error ? err.message : 'Okänt fel',
    }
  } finally {
    isExporting.value = false
  }
}
</script>

<style scoped>
.export-panel {
  background: var(--surface);
  border: 1px solid var(--border);
  border-radius: 12px;
  overflow: hidden;
}

.ep-header {
  padding: 14px 18px;
  background: var(--surface-2);
  border-bottom: 1px solid var(--border);
}
.ep-header h3 { font-size: 14px; font-weight: 600; margin: 0; }

.format-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 8px;
  padding: 14px;
}

.format-btn {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 4px;
  padding: 12px 8px;
  border-radius: 8px;
  border: 2px solid var(--border);
  background: transparent;
  cursor: pointer;
  transition: all 0.15s;
  color: var(--text);
}
.format-btn:hover { border-color: var(--accent); background: color-mix(in srgb, var(--accent) 5%, transparent); }
.format-btn.active { border-color: var(--accent); background: color-mix(in srgb, var(--accent) 10%, transparent); }

.fmt-icon { font-size: 22px; }
.fmt-name { font-size: 12px; font-weight: 500; }
.fmt-best { font-size: 10px; color: #22c55e; }

.export-actions {
  padding: 0 14px 14px;
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.export-btn {
  width: 100%;
  padding: 12px;
  border-radius: 8px;
  border: none;
  background: var(--accent);
  color: white;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.15s;
  min-height: 44px;
}
.export-btn:hover:not(:disabled) { filter: brightness(1.1); }
.export-btn:disabled { opacity: 0.6; cursor: not-allowed; }
.export-btn.loading { cursor: wait; }

.loading-text { display: flex; align-items: center; justify-content: center; gap: 8px; }

.dot-pulse {
  display: inline-block;
  width: 8px;
  height: 8px;
  border-radius: 50%;
  background: white;
  animation: pulse 1s ease-in-out infinite;
}
@keyframes pulse { 0%, 100% { opacity: 1; transform: scale(1); } 50% { opacity: 0.4; transform: scale(0.7); } }

.template-toggle {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 12px;
  color: var(--text-muted);
  cursor: pointer;
}
.template-toggle input { accent-color: var(--accent); }

.result-card {
  margin: 0 14px 14px;
  border-radius: 8px;
  border: 1px solid var(--border);
  overflow: hidden;
}
.result-card.ok   { border-color: #22c55e; }
.result-card.error { border-color: #ef4444; }

.result-header {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 8px 12px;
  background: var(--surface-2);
}
.result-icon { font-size: 14px; }
.result-title { flex: 1; font-size: 13px; font-weight: 600; }
.dismiss { background: transparent; border: none; color: var(--text-muted); cursor: pointer; font-size: 12px; }

.result-stats {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 1px;
  background: var(--border);
}
.stat {
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 10px;
  background: var(--surface);
}
.stat-label { font-size: 10px; color: var(--text-muted); text-transform: uppercase; letter-spacing: 0.5px; }
.stat-val { font-size: 16px; font-weight: 700; margin-top: 2px; }
.stat-val.fast { color: #22c55e; }
.stat-val.ok   { color: #f59e0b; }
.stat-val.slow { color: #ef4444; }

.result-error { padding: 10px 12px; font-size: 12px; color: #ef4444; }

.warning-hint {
  margin: 0 14px 14px;
  padding: 10px 12px;
  background: color-mix(in srgb, #f59e0b 8%, transparent);
  border: 1px solid #f59e0b;
  border-radius: 8px;
  font-size: 12px;
  color: var(--text);
}

.fade-enter-active, .fade-leave-active { transition: all 0.2s; }
.fade-enter-from, .fade-leave-to { opacity: 0; transform: translateY(4px); }
</style>
