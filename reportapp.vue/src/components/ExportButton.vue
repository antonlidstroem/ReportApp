<template>
  <div class="export-panel">
    <div class="ep-header">
      <h3>📤 Export Report</h3>
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
        <span v-if="bestTime(fmt.id)" class="fmt-best">⚡ {{ bestTime(fmt.id) }}ms</span>
      </button>
    </div>

    <!-- Template picker — only for PDF + HTML-capable providers -->
    <div v-if="selectedFormat === 'pdf' && supportsHtmlTemplate" class="template-picker">
      <div class="tp-label">PDF Template</div>
      <div class="tp-options">
        <label
          v-for="t in templateOptions"
          :key="t.id"
          class="tp-opt"
          :class="{ active: selectedTemplate === t.id }"
        >
          <input type="radio" :value="t.id" v-model="selectedTemplate" />
          <span class="tpo-icon">{{ t.icon }}</span>
          <div class="tpo-info">
            <span class="tpo-name">{{ t.name }}</span>
            <span class="tpo-desc">{{ t.desc }}</span>
          </div>
        </label>
      </div>
      <div v-if="selectedTemplate === 'custom'" class="tp-custom">
        <label class="custom-toggle">
          <input type="checkbox" v-model="useCustomHtml" />
          <span class="ct-track"><span class="ct-thumb"></span></span>
          <span>Use Template Designer HTML</span>
        </label>
      </div>
    </div>

    <div class="export-actions">
      <button
        class="export-btn"
        :class="{ loading: isExporting }"
        :disabled="isExporting || !store.surveyId"
        @click="doExport"
      >
        <span v-if="!isExporting" class="btn-inner">
          <span class="btn-icon">⬇</span>
          <span>{{ exportLabel }}</span>
        </span>
        <span v-else class="btn-inner">
          <span class="dot-pulse"></span>
          <span>Generating {{ selectedFormat.toUpperCase() }}...</span>
        </span>
      </button>
    </div>

    <!-- Result card -->
    <transition name="fade">
      <div v-if="lastResult" class="result-card" :class="lastResult.status">
        <div class="rc-header">
          <span>{{ lastResult.status === 'ok' ? '✅ Export complete' : '❌ Export failed' }}</span>
          <button class="rc-dismiss" @click="lastResult = null">✕</button>
        </div>
        <div v-if="lastResult.status === 'ok'" class="rc-stats">
          <div class="rcs-item">
            <span class="rcs-lbl">Time</span>
            <span class="rcs-val" :class="speedClass(lastResult.ms)">{{ lastResult.ms }}ms</span>
          </div>
          <div class="rcs-item">
            <span class="rcs-lbl">Size</span>
            <span class="rcs-val">{{ formatSize(lastResult.bytes) }}</span>
          </div>
          <div class="rcs-item">
            <span class="rcs-lbl">Format</span>
            <span class="rcs-val">{{ lastResult.format.toUpperCase() }}</span>
          </div>
          <div class="rcs-item">
            <span class="rcs-lbl">Template</span>
            <span class="rcs-val">{{ lastResult.template }}</span>
          </div>
        </div>
        <div v-else class="rc-error">{{ lastResult.message }}</div>
      </div>
    </transition>

    <div v-if="!store.surveyId" class="no-survey-hint">
      ⚠ Select a survey above to enable export.
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { useReportBuilderStore } from '../stores/reportBuilder'
import { useBenchmarkStore } from '../stores/benchmarks'
import { exportReport, downloadBlob, RICH_TEMPLATES } from '../composables/useApi'

const props = defineProps<{
  provider: string
  supportsHtmlTemplate: boolean
}>()

const store = useReportBuilderStore()
const benchmarks = useBenchmarkStore()

type Format = 'pdf' | 'excel' | 'ppt'

const formats = [
  { id: 'pdf' as Format, label: 'PDF', icon: '📕' },
  { id: 'excel' as Format, label: 'Excel', icon: '📗' },
  { id: 'ppt' as Format, label: 'PowerPoint', icon: '📘' },
]

const selectedFormat = ref<Format>('pdf')
const isExporting = ref(false)

// Template selection — default depends on provider
const defaultTmpl = (props.provider === 'play-sync') ? 'playwright' : 'full'
const selectedTemplate = ref<string>(defaultTmpl)
const useCustomHtml = ref(false)

const templateOptions = computed(() => {
  if (props.provider === 'play-sync') {
    return [
      { id: 'playwright', icon: '📄', name: 'Simple (token)', desc: '{{Token}} replacement only — Playwright limitation' },
      { id: 'custom', icon: '✏️', name: 'Custom HTML', desc: 'Use Template Designer editor' },
    ]
  }
  return [
    { id: 'full', icon: '📊', name: 'Full Report', desc: 'Bar chart + data table via Chart.js' },
    { id: 'executive', icon: '🌙', name: 'Executive', desc: 'Dark theme + radar chart via Chart.js' },
    { id: 'custom', icon: '✏️', name: 'Custom HTML', desc: 'Use Template Designer editor' },
  ]
})

const exportLabel = computed(() => {
  if (selectedFormat.value === 'pdf' && props.supportsHtmlTemplate) {
    const t = templateOptions.value.find(t => t.id === selectedTemplate.value)
    return `Export PDF — "${t?.name}"`
  }
  return `Export ${selectedFormat.value.toUpperCase()}`
})

const lastResult = ref<{
  status: 'ok' | 'error'
  ms: number; bytes: number; format: string; template: string; message?: string
} | null>(null)

function bestTime(fmt: string) {
  return benchmarks.best(props.provider, fmt)?.generationMs
}

function speedClass(ms: number) {
  return ms < 500 ? 'fast' : ms < 2000 ? 'ok' : 'slow'
}

function formatSize(bytes: number) {
  if (!bytes) return '—'
  if (bytes > 1_000_000) return `${(bytes / 1_000_000).toFixed(1)} MB`
  return `${(bytes / 1024).toFixed(0)} KB`
}

function getTemplateHtml(): string | undefined {
  if (selectedFormat.value !== 'pdf' || !props.supportsHtmlTemplate) return undefined
  if (selectedTemplate.value === 'custom' && useCustomHtml.value) {
    return store.templateHtml || undefined
  }
  if (selectedTemplate.value === 'executive') return RICH_TEMPLATES.jsreportExecutive
  if (selectedTemplate.value === 'playwright') return RICH_TEMPLATES.playwrightSimple
  // default: 'full'
  return RICH_TEMPLATES.jsreportFull
}

function templateLabel(): string {
  if (selectedFormat.value !== 'pdf' || !props.supportsHtmlTemplate) return selectedFormat.value.toUpperCase()
  return templateOptions.value.find(t => t.id === selectedTemplate.value)?.name ?? 'Unknown'
}

async function doExport() {
  if (!store.surveyId || isExporting.value) return
  isExporting.value = true
  lastResult.value = null

  try {
    // FIX: Convert number[] to comma-separated string for backend query param
    const qIds = store.selectedQuestionIds.length > 0
      ? store.selectedQuestionIds.join(',')
      : undefined

    const result = await exportReport({
      provider: props.provider,
      format: selectedFormat.value,
      surveyId: store.surveyId,
      start: store.dateStart || undefined,
      end: store.dateEnd || undefined,
      questionIds: qIds,
      htmlTemplate: getTemplateHtml(),
    })

    downloadBlob(result.blob, result.filename)

    lastResult.value = {
      status: 'ok',
      ms: result.generationMs,
      bytes: result.fileSizeBytes,
      format: selectedFormat.value,
      template: templateLabel(),
    }

    const survey = store.surveys.find(s => s.id === store.surveyId)
    benchmarks.add({
      provider: props.provider,
      format: selectedFormat.value,
      surveyId: store.surveyId!,
      surveyTitle: survey?.title ?? '',
      generationMs: result.generationMs,
      fileSizeBytes: result.fileSizeBytes,
      timestamp: new Date().toISOString(),
      questionCount: store.selectedQuestionIds.length || (survey?.questionCount ?? 0),
    })
  } catch (err: unknown) {
    lastResult.value = {
      status: 'error',
      ms: 0, bytes: 0, format: selectedFormat.value,
      template: templateLabel(),
      message: err instanceof Error ? err.message : 'Unknown error — check browser console and backend logs.',
    }
  } finally {
    isExporting.value = false
  }
}
</script>

<style scoped>
.export-panel {
  background: var(--surface, #1e293b);
  border: 1px solid var(--border, #334155);
  border-radius: 12px;
  overflow: hidden;
}

.ep-header {
  padding: 14px 18px;
  background: var(--surface-2, #0f172a);
  border-bottom: 1px solid var(--border, #334155);
}
.ep-header h3 { font-size: 14px; font-weight: 600; margin: 0; color: var(--text, #f1f5f9); }

/* Format grid */
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
  padding: 12px 6px;
  border-radius: 8px;
  border: 2px solid var(--border, #334155);
  background: transparent;
  cursor: pointer;
  transition: all 0.15s;
  color: var(--text, #f1f5f9);
}
.format-btn:hover { border-color: var(--accent, #3b82f6); background: color-mix(in srgb, var(--accent, #3b82f6) 5%, transparent); }
.format-btn.active { border-color: var(--accent, #3b82f6); background: color-mix(in srgb, var(--accent, #3b82f6) 12%, transparent); }

.fmt-icon { font-size: 22px; }
.fmt-name { font-size: 12px; font-weight: 500; }
.fmt-best { font-size: 10px; color: #22c55e; }

/* Template picker */
.template-picker {
  margin: 0 14px 14px;
  background: var(--surface-2, #0f172a);
  border-radius: 8px;
  overflow: hidden;
  border: 1px solid var(--border, #334155);
}

.tp-label {
  font-size: 10px;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 1px;
  color: var(--text-muted, #64748b);
  padding: 8px 12px;
  border-bottom: 1px solid var(--border, #334155);
}

.tp-options { display: flex; flex-direction: column; }

.tp-opt {
  display: flex;
  align-items: flex-start;
  gap: 10px;
  padding: 10px 12px;
  cursor: pointer;
  border-bottom: 1px solid var(--border, #334155);
  transition: background 0.1s;
}
.tp-opt:last-child { border-bottom: none; }
.tp-opt:hover { background: rgba(255, 255, 255, 0.03); }
.tp-opt.active { background: color-mix(in srgb, var(--accent, #3b82f6) 6%, transparent); }
.tp-opt input { display: none; }

.tpo-icon { font-size: 16px; flex-shrink: 0; margin-top: 1px; }
.tpo-name { display: block; font-size: 12px; font-weight: 600; color: var(--text, #f1f5f9); margin-bottom: 1px; }
.tpo-desc { font-size: 10px; color: var(--text-muted, #64748b); line-height: 1.3; }

.tp-custom {
  padding: 8px 12px;
  border-top: 1px solid var(--border, #334155);
}

.custom-toggle {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 12px;
  color: var(--text-muted, #64748b);
  cursor: pointer;
}
.custom-toggle input { display: none; }

.ct-track {
  width: 32px; height: 18px;
  background: var(--border, #334155);
  border-radius: 100px;
  position: relative;
  transition: background 0.2s;
  flex-shrink: 0;
}
.custom-toggle input:checked ~ .ct-track { background: var(--accent, #3b82f6); }

.ct-thumb {
  position: absolute;
  left: 2px; top: 2px;
  width: 14px; height: 14px;
  border-radius: 50%;
  background: white;
  transition: transform 0.2s;
  box-shadow: 0 1px 3px rgba(0,0,0,0.3);
}
.custom-toggle input:checked ~ .ct-track .ct-thumb { transform: translateX(14px); }

/* Export button */
.export-actions {
  padding: 0 14px 14px;
}

.export-btn {
  width: 100%;
  padding: 11px;
  border-radius: 8px;
  border: none;
  background: var(--accent, #3b82f6);
  color: white;
  font-size: 13px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.15s;
  min-height: 42px;
  font-family: inherit;
}
.export-btn:hover:not(:disabled) { filter: brightness(1.1); }
.export-btn:disabled { opacity: 0.5; cursor: not-allowed; }
.export-btn.loading { cursor: wait; opacity: 0.8; }

.btn-inner { display: flex; align-items: center; justify-content: center; gap: 7px; }
.btn-icon { font-size: 14px; }

.dot-pulse {
  width: 8px; height: 8px;
  border-radius: 50%;
  background: white;
  animation: pulse 1s ease-in-out infinite;
}
@keyframes pulse {
  0%, 100% { opacity: 1; transform: scale(1); }
  50% { opacity: 0.4; transform: scale(0.75); }
}

/* Result card */
.result-card {
  margin: 0 14px 14px;
  border-radius: 8px;
  border: 1px solid var(--border, #334155);
  overflow: hidden;
}
.result-card.ok { border-color: #22c55e; }
.result-card.error { border-color: #ef4444; }

.rc-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 8px 12px;
  background: var(--surface-2, #0f172a);
  font-size: 12px;
  font-weight: 600;
  color: var(--text, #f1f5f9);
}
.rc-dismiss {
  background: transparent;
  border: none;
  color: var(--text-muted, #64748b);
  cursor: pointer;
  font-size: 11px;
}

.rc-stats {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 1px;
  background: var(--border, #334155);
}

.rcs-item {
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 10px 6px;
  background: var(--surface, #1e293b);
}
.rcs-lbl { font-size: 9px; color: var(--text-muted, #64748b); text-transform: uppercase; letter-spacing: 0.5px; }
.rcs-val { font-size: 13px; font-weight: 700; margin-top: 2px; color: var(--text, #f1f5f9); white-space: nowrap; overflow: hidden; text-overflow: ellipsis; max-width: 72px; }
.rcs-val.fast { color: #22c55e; }
.rcs-val.ok   { color: #f59e0b; }
.rcs-val.slow { color: #ef4444; }

.rc-error {
  padding: 10px 12px;
  font-size: 12px;
  color: #ef4444;
  line-height: 1.5;
}

.no-survey-hint {
  margin: 0 14px 14px;
  padding: 10px 12px;
  background: color-mix(in srgb, #f59e0b 8%, transparent);
  border: 1px solid rgba(245, 158, 11, 0.3);
  border-radius: 8px;
  font-size: 12px;
  color: var(--text, #f1f5f9);
}

.fade-enter-active, .fade-leave-active { transition: all 0.2s; }
.fade-enter-from, .fade-leave-to { opacity: 0; transform: translateY(4px); }
</style>
