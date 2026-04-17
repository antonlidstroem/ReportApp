<template>
  <div class="export-panel">
    <div class="ep-header"><h3>📤 Exportera rapport</h3></div>

    <!-- Format picker -->
    <div class="format-grid">
      <button v-for="f in formats" :key="f.id" :class="['fmt-btn', { active: selectedFmt === f.id }]" @click="selectedFmt = f.id">
        <span class="fmt-icon">{{ f.icon }}</span>
        <span class="fmt-name">{{ f.label }}</span>
        <span v-if="bestMs(f.id)" class="fmt-best">⚡ {{ bestMs(f.id) }}ms</span>
      </button>
    </div>

    <!-- Template picker for PDF with HTML support -->
    <div v-if="selectedFmt === 'pdf' && supportsHtmlTemplate" class="tmpl-picker">
      <div class="tp-label">PDF-mall</div>
      <div class="tp-opts">
        <label v-for="t in templateOpts" :key="t.id" :class="['tp-opt', { active: selectedTmpl === t.id }]">
          <input type="radio" :value="t.id" v-model="selectedTmpl" />
          <span class="tp-icon">{{ t.icon }}</span>
          <div>
            <span class="tp-name">{{ t.name }}</span>
            <span class="tp-desc">{{ t.desc }}</span>
          </div>
        </label>
      </div>
    </div>

    <!-- Export button -->
    <div class="ep-action">
      <button :class="['exp-btn', { loading: isLoading }]" :disabled="isLoading || !store.surveyId" @click="doExport">
        <span v-if="!isLoading" class="exp-inner">⬇ {{ btnLabel }}</span>
        <span v-else class="exp-inner"><span class="spin"></span> Genererar {{ selectedFmt.toUpperCase() }}…</span>
      </button>
    </div>

    <!-- Result -->
    <transition name="fade">
      <div v-if="lastResult" :class="['result', lastResult.ok ? 'ok' : 'err']">
        <div class="result-hdr">
          <span>{{ lastResult.ok ? '✅ Export klar' : '❌ Export misslyckades' }}</span>
          <button class="result-close" @click="lastResult = null">✕</button>
        </div>
        <div v-if="lastResult.ok" class="result-stats">
          <div class="rs"><span>Tid</span><span :class="msClass(lastResult.ms!)">{{ lastResult.ms }}ms</span></div>
          <div class="rs"><span>Storlek</span><span>{{ lastResult.size }}</span></div>
          <div class="rs"><span>Format</span><span>{{ lastResult.fmt.toUpperCase() }}</span></div>
          <div class="rs"><span>Mall</span><span>{{ lastResult.tmpl }}</span></div>
        </div>
        <div v-else class="result-err-msg">{{ lastResult.msg }}</div>
      </div>
    </transition>

    <div v-if="!store.surveyId" class="ep-hint">⚠ Välj en enkät ovan för att aktivera export.</div>
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
const bench = useBenchmarkStore()

const selectedFmt = ref('pdf')
const isLoading = ref(false)

// Default template: playwright for play-sync, full otherwise
const defaultTmpl = props.provider === 'play-sync' ? 'playwright' : 'full'
const selectedTmpl = ref(defaultTmpl)

const formats = [
  { id: 'pdf',   icon: '📕', label: 'PDF'        },
  { id: 'excel', icon: '📗', label: 'Excel'      },
  { id: 'ppt',   icon: '📘', label: 'PowerPoint' },
]

const templateOpts = computed(() => {
  if (props.provider === 'play-sync') {
    return [
      { id: 'playwright', icon: '🎭', name: 'Playwright Enkel', desc: 'Token-ersättning, ingen Handlebars' },
      { id: 'custom',     icon: '✏️', name: 'Anpassad HTML',    desc: 'Använd Template Designer' },
    ]
  }
  return [
    { id: 'full',      icon: '📊', name: 'Fullständig rapport', desc: 'Stapeldiagram + datatabell via Chart.js'  },
    { id: 'executive', icon: '🌙', name: 'Executive',           desc: 'Mörkt tema + radarkarta via Chart.js'     },
    { id: 'custom',    icon: '✏️', name: 'Anpassad HTML',       desc: 'Använd Template Designer'                },
  ]
})

const btnLabel = computed(() => {
  if (selectedFmt.value !== 'pdf' || !props.supportsHtmlTemplate) return `Exportera ${selectedFmt.value.toUpperCase()}`
  const t = templateOpts.value.find(t => t.id === selectedTmpl.value)
  return `PDF — "${t?.name}"`
})

function getTemplate(): string | undefined {
  if (selectedFmt.value !== 'pdf' || !props.supportsHtmlTemplate) return undefined
  if (selectedTmpl.value === 'custom')     return store.templateHtml || undefined
  if (selectedTmpl.value === 'executive')  return RICH_TEMPLATES.jsreportExecutive
  if (selectedTmpl.value === 'playwright') return RICH_TEMPLATES.playwrightSimple
  return RICH_TEMPLATES.jsreportFull
}

function tmplLabel(): string {
  if (selectedFmt.value !== 'pdf' || !props.supportsHtmlTemplate) return selectedFmt.value.toUpperCase()
  return templateOpts.value.find(t => t.id === selectedTmpl.value)?.name ?? '?'
}

function bestMs(fmt: string) { return bench.best(props.provider, fmt)?.generationMs }
function msClass(ms: number) { return ms < 500 ? 'fast' : ms < 2000 ? 'med' : 'slow' }

const lastResult = ref<{
  ok: boolean; ms?: number; size?: string; fmt: string; tmpl: string; msg?: string
} | null>(null)

async function doExport() {
  if (!store.surveyId || isLoading.value) return
  isLoading.value = true
  lastResult.value = null

  const tmpl = tmplLabel()
  try {
    // FIX: convert number[] to comma-separated string
    const qIds = store.selectedQuestionIds.length > 0
      ? store.selectedQuestionIds.join(',')
      : undefined

    const r = await exportReport({
      provider: props.provider,
      format: selectedFmt.value,
      surveyId: store.surveyId,
      htmlTemplate: getTemplate(),
      start: store.dateStart || undefined,
      end:   store.dateEnd   || undefined,
      questionIds: qIds,
    })

    downloadBlob(r.blob, r.filename)

    const kb = r.fileSizeBytes > 1_000_000
      ? `${(r.fileSizeBytes/1_000_000).toFixed(1)} MB`
      : `${(r.fileSizeBytes/1024).toFixed(0)} KB`

    lastResult.value = { ok: true, ms: r.generationMs, size: kb, fmt: selectedFmt.value, tmpl }

    const survey = store.surveys.find(s => s.id === store.surveyId)
    bench.add({
      provider: props.provider,
      format: selectedFmt.value,
      surveyId: store.surveyId,
      surveyTitle: survey?.title ?? '',
      generationMs: r.generationMs,
      fileSizeBytes: r.fileSizeBytes,
      timestamp: new Date().toISOString(),
      questionCount: store.selectedQuestionIds.length || (survey?.questionCount ?? 0),
    })
  } catch (err) {
    lastResult.value = {
      ok: false,
      fmt: selectedFmt.value,
      tmpl,
      msg: err instanceof Error ? err.message.substring(0, 200) : 'Okänt fel — kontrollera backend-loggar.',
    }
  } finally {
    isLoading.value = false
  }
}
</script>

<style scoped>
.export-panel { background: var(--surface, #1e293b); border: 1px solid var(--border, #334155); border-radius: 12px; overflow: hidden; }
.ep-header { padding: 13px 17px; background: var(--surface-2, #0f172a); border-bottom: 1px solid var(--border, #334155); }
.ep-header h3 { font-size: 13px; font-weight: 600; margin: 0; color: var(--text, #f1f5f9); }
.format-grid { display: grid; grid-template-columns: repeat(3,1fr); gap: 8px; padding: 13px; }
.fmt-btn { display:flex; flex-direction:column; align-items:center; gap:3px; padding:11px 6px; border-radius:8px; border:2px solid var(--border,#334155); background:transparent; cursor:pointer; transition:all .14s; color:var(--text,#f1f5f9); font-family:inherit; }
.fmt-btn:hover { border-color: var(--accent,#a855f7); }
.fmt-btn.active { border-color:var(--accent,#a855f7); background:color-mix(in srgb,var(--accent,#a855f7) 10%,transparent); }
.fmt-icon { font-size:21px; } .fmt-name { font-size:12px; font-weight:500; } .fmt-best { font-size:10px; color:#22c55e; }
.tmpl-picker { margin: 0 13px 13px; background:var(--surface-2,#0f172a); border-radius:8px; overflow:hidden; border:1px solid var(--border,#334155); }
.tp-label { font-size:10px; font-weight:700; text-transform:uppercase; letter-spacing:1px; color:var(--text-muted,#64748b); padding:7px 12px; border-bottom:1px solid var(--border,#334155); }
.tp-opts { display:flex; flex-direction:column; }
.tp-opt { display:flex; align-items:flex-start; gap:9px; padding:9px 12px; cursor:pointer; border-bottom:1px solid var(--border,#334155); transition:background .1s; }
.tp-opt:last-child { border-bottom:none; }
.tp-opt:hover { background:rgba(255,255,255,.03); }
.tp-opt.active { background:color-mix(in srgb,var(--accent,#a855f7) 6%,transparent); }
.tp-opt input { display:none; }
.tp-icon { font-size:15px; flex-shrink:0; margin-top:1px; }
.tp-name { display:block; font-size:12px; font-weight:600; color:var(--text,#f1f5f9); margin-bottom:1px; }
.tp-desc { font-size:10px; color:var(--text-muted,#64748b); }
.ep-action { padding: 0 13px 13px; }
.exp-btn { width:100%; padding:11px; border-radius:8px; border:none; background:var(--accent,#a855f7); color:white; font-size:13px; font-weight:600; cursor:pointer; transition:all .14s; min-height:42px; font-family:inherit; }
.exp-btn:hover:not(:disabled) { filter:brightness(1.1); }
.exp-btn:disabled { opacity:.5; cursor:not-allowed; }
.exp-btn.loading { cursor:wait; opacity:.8; }
.exp-inner { display:flex; align-items:center; justify-content:center; gap:7px; }
.spin { width:14px; height:14px; border:2px solid rgba(255,255,255,.2); border-top-color:white; border-radius:50%; animation:spin .7s linear infinite; }
@keyframes spin { to { transform:rotate(360deg); } }
.result { margin: 0 13px 13px; border-radius:8px; border:1px solid var(--border,#334155); overflow:hidden; }
.result.ok { border-color:#22c55e; } .result.err { border-color:#ef4444; }
.result-hdr { display:flex; justify-content:space-between; align-items:center; padding:7px 11px; background:var(--surface-2,#0f172a); font-size:12px; font-weight:600; color:var(--text,#f1f5f9); }
.result-close { background:transparent; border:none; color:var(--text-muted,#64748b); cursor:pointer; font-size:11px; }
.result-stats { display:grid; grid-template-columns:repeat(4,1fr); gap:1px; background:var(--border,#334155); }
.rs { display:flex; flex-direction:column; align-items:center; padding:9px 5px; background:var(--surface,#1e293b); }
.rs span:first-child { font-size:9px; color:var(--text-muted,#64748b); text-transform:uppercase; letter-spacing:.5px; }
.rs span:last-child { font-size:12px; font-weight:700; margin-top:2px; color:var(--text,#f1f5f9); white-space:nowrap; overflow:hidden; text-overflow:ellipsis; max-width:68px; }
.fast { color:#22c55e !important; } .med { color:#f59e0b !important; } .slow { color:#ef4444 !important; }
.result-err-msg { padding:9px 11px; font-size:11px; color:#ef4444; line-height:1.5; }
.ep-hint { margin:0 13px 13px; padding:9px 11px; background:color-mix(in srgb,#f59e0b 8%,transparent); border:1px solid rgba(245,158,11,.3); border-radius:7px; font-size:12px; color:var(--text,#f1f5f9); }
.fade-enter-active, .fade-leave-active { transition:all .2s; }
.fade-enter-from, .fade-leave-to { opacity:0; transform:translateY(4px); }
</style>
