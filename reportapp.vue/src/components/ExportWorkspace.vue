<template>
  <div class="export-workspace">
    <!-- Left: survey tree -->
    <div class="ew-left">
      <SurveyTree />
    </div>

    <!-- Center: tabbed options -->
    <div class="ew-center">
      <div class="tab-bar">
        <button
          v-for="tab in visibleTabs"
          :key="tab.id"
          :class="['tab-btn', { active: activeTab === tab.id }]"
          @click="activeTab = tab.id"
        >
          {{ tab.icon }} {{ tab.label }}
        </button>
      </div>

      <div class="tab-body">
        <!-- Format tab -->
        <div v-if="activeTab === 'format'" class="tab-content">
          <div class="tc-label">Exportformat</div>
          <div class="format-grid">
            <button
              v-for="f in formats"
              :key="f.id"
              :class="['fmt-card', { active: selectedFormat === f.id }]"
              @click="selectedFormat = f.id"
            >
              <span class="fmt-icon">{{ f.icon }}</span>
              <span class="fmt-name">{{ f.label }}</span>
              <span v-if="bestMs(f.id)" class="fmt-best">⚡ {{ bestMs(f.id) }}ms</span>
            </button>
          </div>

          <!-- Template picker for PDF with HTML support -->
          <template v-if="selectedFormat === 'pdf' && supportsHtmlTemplate">
            <div class="tc-label" style="margin-top:16px">PDF-mall</div>
            <div class="tmpl-list">
              <label
                v-for="t in templateOptions"
                :key="t.id"
                :class="['tmpl-opt', { active: selectedTemplate === t.id }]"
              >
                <input type="radio" :value="t.id" v-model="selectedTemplate" />
                <span class="to-icon">{{ t.icon }}</span>
                <div>
                  <span class="to-name">{{ t.name }}</span>
                  <span class="to-desc">{{ t.desc }}</span>
                </div>
              </label>
            </div>
          </template>
        </div>

        <!-- Chart tab -->
        <div v-if="activeTab === 'chart'" class="tab-content tab-chart">
          <ChartConfigurator />
        </div>

        <!-- Template tab -->
        <div v-if="activeTab === 'template'" class="tab-content">
          <div class="tc-label">HTML-mall (förhandsgranskning)</div>
          <div class="tmpl-presets">
            <button
              v-for="p in presets"
              :key="p.id"
              :class="['preset-btn', { active: selectedTemplate === p.id }]"
              @click="selectedTemplate = p.id"
            >{{ p.icon }} {{ p.name }}</button>
          </div>
          <div class="tmpl-note">
            Välj en mall ovan. Den skickas till {{ provider }} och renderas av Chromium.
          </div>
        </div>

        <!-- Options slot -->
        <div v-if="activeTab === 'options'" class="tab-content">
          <slot name="options">
            <div class="no-options">Inga extra alternativ för denna provider.</div>
          </slot>
        </div>
      </div>
    </div>

    <!-- Right: export panel -->
    <div class="ew-right">
      <!-- Export button -->
      <div class="export-panel">
        <div class="ep-top">
          <div class="ep-format">
            <span class="ep-fmt-icon">{{ currentFormat?.icon }}</span>
            <span class="ep-fmt-label">{{ currentFormat?.label }}</span>
          </div>
          <div v-if="!store.surveyId" class="ep-warn">Välj en enkät</div>
          <div v-else-if="store.selectedCount === 0" class="ep-warn">Välj minst en fråga</div>
        </div>

        <button
          :class="['export-btn', { loading: isLoading, disabled: !canExport }]"
          :disabled="!canExport || isLoading"
          @click="doExport"
          :style="`--pc: ${providerColor}`"
        >
          <span v-if="!isLoading" class="eb-inner">
            <span>⬇</span>
            <span>Exportera {{ currentFormat?.label }}</span>
          </span>
          <span v-else class="eb-inner">
            <span class="eb-spin"></span>
            <span>Genererar…</span>
          </span>
        </button>

        <!-- Result card -->
        <Transition name="result-fade">
          <div v-if="lastResult" :class="['result-card', lastResult.ok ? 'ok' : lastResult.isNotImplemented ? 'stub' : 'err']">
            <div class="rc-hdr">
              <span>{{ resultIcon }}</span>
              <span>{{ resultTitle }}</span>
              <button class="rc-close" @click="lastResult = null">✕</button>
            </div>

            <template v-if="lastResult.ok">
              <div class="rc-stats">
                <div class="rcs"><span>Tid</span><span :class="speedClass(lastResult.ms!)">{{ lastResult.ms }}ms</span></div>
                <div class="rcs"><span>Storlek</span><span>{{ lastResult.size }}</span></div>
                <div class="rcs"><span>Frågor</span><span>{{ store.selectedCount }}</span></div>
              </div>
            </template>

            <template v-else-if="lastResult.isNotImplemented">
              <div class="rc-stub-msg">
                <strong>{{ provider }}</strong> är inte implementerad ännu.<br>
                Lägg till NuGet-paketet och fyll i provider-klassen för att aktivera.
              </div>
            </template>

            <template v-else>
              <div class="rc-err-detail">
                <div v-if="lastResult.status" class="rc-status">HTTP {{ lastResult.status }}</div>
                <div class="rc-msg">{{ lastResult.msg }}</div>
              </div>
            </template>
          </div>
        </Transition>
      </div>

      <!-- Mini benchmark -->
      <div v-if="recentBenchmarks.length > 0" class="mini-bench">
        <div class="mb-label">Senaste exports</div>
        <div
          v-for="r in recentBenchmarks"
          :key="r.timestamp"
          class="mb-row"
        >
          <span class="mb-fmt">{{ r.format.toUpperCase() }}</span>
          <span :class="['mb-ms', speedClass(r.generationMs)]">{{ r.generationMs }}ms</span>
          <span class="mb-size">{{ formatSize(r.fileSizeBytes) }}</span>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import SurveyTree from './SurveyTree.vue'
import ChartConfigurator from './ChartConfigurator.vue'
import { useReportBuilderStore } from '../stores/reportBuilder'
import { useBenchmarkStore } from '../stores/benchmarks'
import { exportReport, downloadBlob, RICH_TEMPLATES } from '../composables/useApi'
import type { ExportError } from '../composables/useApi'

const props = defineProps<{
  provider: string
  providerColor?: string
  supportsHtmlTemplate?: boolean
  supportsCharts?: boolean
  isStub?: boolean  // DevExpress etc — shows "not implemented" card
}>()

const store = useReportBuilderStore()
const bench = useBenchmarkStore()

// ── Tab setup ─────────────────────────────────────────────────────────────
const allTabs = [
  { id: 'format',   icon: '📄', label: 'Format'   },
  { id: 'chart',    icon: '📊', label: 'Diagram'   },
  { id: 'template', icon: '🎨', label: 'Mall'      },
  { id: 'options',  icon: '⚙️', label: 'Alternativ' },
]

const visibleTabs = computed(() =>
  allTabs.filter(t => {
    if (t.id === 'chart')    return props.supportsCharts
    if (t.id === 'template') return props.supportsHtmlTemplate
    return true
  })
)

const activeTab = ref('format')

// ── Format ────────────────────────────────────────────────────────────────
const formats = [
  { id: 'pdf',   icon: '📕', label: 'PDF'   },
  { id: 'excel', icon: '📗', label: 'Excel' },
  { id: 'ppt',   icon: '📘', label: 'PPT'   },
]
const selectedFormat = ref('pdf')
const currentFormat  = computed(() => formats.find(f => f.id === selectedFormat.value))

// ── Template options ───────────────────────────────────────────────────────
const isPlaywright = computed(() => props.provider === 'play-sync')

const templateOptions = computed(() => {
  if (isPlaywright.value) {
    return [
      { id: 'playwright', icon: '🎭', name: 'Playwright Enkel', desc: 'Token-ersättning, ingen Handlebars' },
    ]
  }
  return [
    { id: 'full',      icon: '📊', name: 'Fullständig rapport', desc: 'Stapeldiagram + tabell via Chart.js' },
    { id: 'executive', icon: '🌙', name: 'Executive',           desc: 'Mörkt tema + kategorimedelvärden'   },
  ]
})

const presets = templateOptions  // alias for template tab

const selectedTemplate = ref(isPlaywright.value ? 'playwright' : 'full')

function getHtmlTemplate(): string | undefined {
  if (selectedFormat.value !== 'pdf' || !props.supportsHtmlTemplate) return undefined
  if (selectedTemplate.value === 'executive') return RICH_TEMPLATES.jsreportExecutive
  if (selectedTemplate.value === 'playwright') return RICH_TEMPLATES.playwrightSimple
  return RICH_TEMPLATES.jsreportFull
}

// ── Export ────────────────────────────────────────────────────────────────
const isLoading = ref(false)
const canExport = computed(
  () => !!store.surveyId && store.selectedCount > 0 && !isLoading.value
)

const lastResult = ref<{
  ok: boolean
  ms?: number
  size?: string
  status?: number | null
  msg?: string
  isNotImplemented?: boolean
} | null>(null)

const resultIcon  = computed(() => {
  if (!lastResult.value) return ''
  if (lastResult.value.ok) return '✅'
  if (lastResult.value.isNotImplemented) return '🚧'
  return '❌'
})
const resultTitle = computed(() => {
  if (!lastResult.value) return ''
  if (lastResult.value.ok) return 'Export klar'
  if (lastResult.value.isNotImplemented) return 'Ej implementerad'
  return 'Export misslyckades'
})

async function doExport() {
  if (!canExport.value) return
  isLoading.value = true
  lastResult.value = null

  try {
    const r = await exportReport({
      provider: props.provider,
      format: selectedFormat.value,
      surveyId: store.surveyId!,
      htmlTemplate: getHtmlTemplate(),
      start: store.dateStart || undefined,
      end:   store.dateEnd   || undefined,
      questionIds: store.selectedQuestionIds.length > 0
        ? store.selectedQuestionIds
        : undefined,
      moduleConfig: {
        chartType:          store.chartConfig.type,
        chartQuestionCount: store.chartConfig.questionCount,
      },
    })

    downloadBlob(r.blob, r.filename)

    const kb = r.fileSizeBytes > 1_000_000
      ? `${(r.fileSizeBytes / 1_000_000).toFixed(1)} MB`
      : `${(r.fileSizeBytes / 1024).toFixed(0)} KB`

    lastResult.value = { ok: true, ms: r.generationMs, size: kb }

    const survey = store.surveys.find(s => s.id === store.surveyId)
    bench.add({
      provider: props.provider,
      format: selectedFormat.value,
      surveyId: store.surveyId!,
      surveyTitle: survey?.title ?? '',
      generationMs: r.generationMs,
      fileSizeBytes: r.fileSizeBytes,
      timestamp: new Date().toISOString(),
      questionCount: store.selectedCount,
    })
  } catch (err) {
    const exportErr = (err as Error & { exportError?: ExportError }).exportError
    lastResult.value = {
      ok: false,
      status: exportErr?.status ?? null,
      msg: err instanceof Error ? err.message.substring(0, 300) : 'Okänt fel',
      isNotImplemented: exportErr?.isNotImplemented ?? false,
    }
  } finally {
    isLoading.value = false
  }
}

// ── Benchmark helpers ─────────────────────────────────────────────────────
const recentBenchmarks = computed(() =>
  bench.results.filter(r => r.provider === props.provider).slice(0, 5)
)

function bestMs(fmt: string) {
  return bench.best(props.provider, fmt)?.generationMs
}

function speedClass(ms: number) {
  return ms < 500 ? 'fast' : ms < 2000 ? 'med' : 'slow'
}

function formatSize(bytes: number) {
  if (!bytes) return '—'
  if (bytes > 1_000_000) return `${(bytes / 1_000_000).toFixed(1)} MB`
  return `${(bytes / 1024).toFixed(0)} KB`
}
</script>

<style scoped>
.export-workspace {
  display: grid;
  grid-template-columns: 280px 1fr 260px;
  gap: 16px;
  align-items: start;
}

/* ── Left ────────────────────────────────────────── */
.ew-left {
  position: sticky;
  top: 80px;
  max-height: calc(100vh - 100px);
  overflow-y: auto;
}
.ew-left::-webkit-scrollbar { width: 3px; }
.ew-left::-webkit-scrollbar-thumb { background: var(--border, #475569); border-radius: 3px; }

/* ── Center ──────────────────────────────────────── */
.ew-center {
  background: var(--surface, #1e293b);
  border: 1px solid var(--border, #334155);
  border-radius: 12px;
  overflow: hidden;
}

.tab-bar {
  display: flex;
  border-bottom: 1px solid var(--border, #334155);
  background: var(--surface-2, #334155);
}

.tab-btn {
  flex: 1;
  padding: 10px 8px;
  border: none;
  background: transparent;
  color: var(--text-muted, #64748b);
  font-size: 12px;
  font-weight: 500;
  cursor: pointer;
  border-bottom: 2px solid transparent;
  transition: all 0.13s;
  font-family: inherit;
}
.tab-btn:hover { color: var(--text, #f1f5f9); }
.tab-btn.active {
  color: var(--accent, #3b82f6);
  border-bottom-color: var(--accent, #3b82f6);
  background: color-mix(in srgb, var(--accent, #3b82f6) 5%, transparent);
}

.tab-body { min-height: 320px; }

.tab-content { padding: 16px; }
.tab-chart { padding: 0; }

.tc-label {
  font-size: 10px;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 1px;
  color: var(--text-muted, #64748b);
  margin-bottom: 10px;
}

.format-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 8px;
  margin-bottom: 4px;
}

.fmt-card {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 4px;
  padding: 14px 8px;
  border-radius: 9px;
  border: 2px solid var(--border, #475569);
  background: transparent;
  cursor: pointer;
  color: var(--text, #f1f5f9);
  font-family: inherit;
  transition: all 0.13s;
}
.fmt-card:hover { border-color: var(--accent, #3b82f6); }
.fmt-card.active {
  border-color: var(--accent, #3b82f6);
  background: color-mix(in srgb, var(--accent, #3b82f6) 10%, transparent);
}
.fmt-icon { font-size: 22px; }
.fmt-name { font-size: 12px; font-weight: 600; }
.fmt-best { font-size: 9px; color: #22c55e; }

.tmpl-list { display: flex; flex-direction: column; gap: 5px; }
.tmpl-opt {
  display: flex;
  align-items: flex-start;
  gap: 10px;
  padding: 9px 12px;
  border-radius: 8px;
  border: 1px solid var(--border, #475569);
  cursor: pointer;
  transition: all 0.12s;
}
.tmpl-opt:hover { background: rgba(255,255,255,0.03); }
.tmpl-opt.active {
  border-color: var(--accent, #3b82f6);
  background: color-mix(in srgb, var(--accent, #3b82f6) 8%, transparent);
}
.tmpl-opt input { display: none; }
.to-icon  { font-size: 16px; flex-shrink: 0; margin-top: 1px; }
.to-name  { display: block; font-size: 12px; font-weight: 600; margin-bottom: 2px; }
.to-desc  { font-size: 10px; color: var(--text-muted, #64748b); }

.tmpl-presets { display: flex; gap: 6px; flex-wrap: wrap; margin-bottom: 10px; }
.preset-btn {
  font-size: 11px;
  padding: 6px 12px;
  border-radius: 6px;
  border: 1px solid var(--border, #475569);
  background: transparent;
  color: var(--text-muted, #94a3b8);
  cursor: pointer;
  font-family: inherit;
  transition: all 0.12s;
}
.preset-btn:hover { background: rgba(255,255,255,0.05); color: var(--text, #f1f5f9); }
.preset-btn.active { border-color: var(--accent, #3b82f6); color: var(--accent, #3b82f6); }

.tmpl-note {
  font-size: 11px;
  color: var(--text-muted, #64748b);
  background: rgba(255,255,255,0.02);
  border-radius: 6px;
  padding: 8px 10px;
  border: 1px solid var(--border, #475569);
  line-height: 1.5;
}

.no-options {
  font-size: 12px;
  color: var(--text-muted, #64748b);
  text-align: center;
  padding: 24px;
}

/* ── Right ───────────────────────────────────────── */
.ew-right {
  display: flex;
  flex-direction: column;
  gap: 12px;
  position: sticky;
  top: 80px;
}

.export-panel {
  background: var(--surface, #1e293b);
  border: 1px solid var(--border, #334155);
  border-radius: 12px;
  padding: 14px;
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.ep-top {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.ep-format {
  display: flex;
  align-items: center;
  gap: 8px;
}
.ep-fmt-icon  { font-size: 20px; }
.ep-fmt-label { font-size: 13px; font-weight: 700; color: var(--text, #f1f5f9); }

.ep-warn {
  font-size: 10px;
  color: #f59e0b;
  background: rgba(245,158,11,0.08);
  border: 1px solid rgba(245,158,11,0.25);
  padding: 2px 8px;
  border-radius: 4px;
}

.export-btn {
  width: 100%;
  padding: 13px;
  border-radius: 9px;
  border: none;
  background: var(--pc, var(--accent, #3b82f6));
  color: white;
  font-size: 14px;
  font-weight: 700;
  cursor: pointer;
  transition: all 0.15s;
  min-height: 46px;
  font-family: inherit;
}
.export-btn:hover:not(.disabled):not(.loading) { filter: brightness(1.12); transform: translateY(-1px); }
.export-btn.disabled { opacity: 0.45; cursor: not-allowed; transform: none; }
.export-btn.loading  { cursor: wait; opacity: 0.8; }

.eb-inner {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
}
.eb-spin {
  width: 15px; height: 15px;
  border: 2px solid rgba(255,255,255,0.2);
  border-top-color: white;
  border-radius: 50%;
  animation: spin 0.7s linear infinite;
}
@keyframes spin { to { transform: rotate(360deg); } }

/* Result card */
.result-card {
  border-radius: 8px;
  overflow: hidden;
  border: 1px solid var(--border, #475569);
}
.result-card.ok   { border-color: #22c55e; }
.result-card.err  { border-color: #ef4444; }
.result-card.stub { border-color: #f59e0b; }

.rc-hdr {
  display: flex;
  align-items: center;
  gap: 7px;
  padding: 7px 10px;
  background: var(--surface-2, #334155);
  font-size: 12px;
  font-weight: 600;
  color: var(--text, #f1f5f9);
}
.rc-close {
  margin-left: auto;
  background: transparent;
  border: none;
  color: var(--text-muted, #64748b);
  cursor: pointer;
  font-size: 11px;
}

.rc-stats {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 1px;
  background: var(--border, #475569);
}
.rcs {
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 8px 4px;
  background: var(--surface, #1e293b);
}
.rcs span:first-child { font-size: 9px; color: var(--text-muted, #64748b); text-transform: uppercase; }
.rcs span:last-child  { font-size: 13px; font-weight: 700; color: var(--text, #f1f5f9); margin-top: 2px; }
.fast { color: #22c55e !important; }
.med  { color: #f59e0b !important; }
.slow { color: #ef4444 !important; }

.rc-stub-msg {
  padding: 10px 12px;
  font-size: 11px;
  color: #f59e0b;
  line-height: 1.5;
}

.rc-err-detail { padding: 10px 12px; }
.rc-status { font-size: 11px; font-weight: 700; color: #ef4444; margin-bottom: 4px; }
.rc-msg    { font-size: 11px; color: #fca5a5; line-height: 1.4; }

/* Mini bench */
.mini-bench {
  background: var(--surface, #1e293b);
  border: 1px solid var(--border, #334155);
  border-radius: 10px;
  overflow: hidden;
}

.mb-label {
  font-size: 10px;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 1px;
  color: var(--text-muted, #64748b);
  padding: 8px 12px;
  background: var(--surface-2, #334155);
  border-bottom: 1px solid var(--border, #475569);
}

.mb-row {
  display: grid;
  grid-template-columns: 48px 1fr 60px;
  padding: 6px 12px;
  border-bottom: 1px solid rgba(255,255,255,0.03);
  font-size: 11px;
  align-items: center;
}
.mb-row:last-child { border-bottom: none; }
.mb-fmt  { font-weight: 700; color: var(--text-muted, #94a3b8); font-size: 10px; }
.mb-ms   { font-weight: 700; }
.mb-size { font-size: 10px; color: var(--text-muted, #64748b); text-align: right; }

/* Transitions */
.result-fade-enter-active, .result-fade-leave-active { transition: all 0.2s ease; }
.result-fade-enter-from, .result-fade-leave-to { opacity: 0; transform: translateY(4px); }
</style>
