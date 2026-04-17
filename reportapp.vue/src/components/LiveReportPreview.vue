<template>
  <div class="live-preview">
    <div class="lp-header">
      <div class="lp-title-group">
        <span class="lp-eyebrow">LIVE REPORT PREVIEW</span>
        <h3>Förhandsgranskning</h3>
        <span class="lp-sub">Visar HTML som skickas till {{ engineLabel }}.</span>
      </div>
      <div class="lp-controls">
        <button v-for="t in templates" :key="t.id"
          :class="['tmpl-btn', { active: activeTemplate === t.id }]"
          @click="switchTemplate(t.id)">{{ t.icon }} {{ t.name }}</button>
        <button class="refresh-btn" @click="renderPreview" title="Refresh">↺</button>
      </div>
    </div>
    <div class="lp-frame-wrap">
      <div v-if="isLoading" class="lp-loading"><div class="lp-spinner"></div><span>Rendering…</span></div>
      <iframe ref="frameRef" class="lp-frame" sandbox="allow-same-origin allow-scripts" title="Report preview"></iframe>
    </div>
    <div class="lp-footer">
      <span class="lf-engine">Engine: {{ engineLabel }}</span>
      <span class="lf-note">{{ footerNote }}</span>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch, nextTick } from 'vue'
import { buildPreviewHtml, CHART_JS_URL } from '../composables/useApi'

const props = defineProps<{ provider: string; showSliders?: boolean }>()
const frameRef = ref<HTMLIFrameElement>()
const activeTemplate = ref('full')
const isLoading = ref(false)

const engineLabel = computed(() => {
  if (props.provider === 'play-sync') return 'Playwright (token replacement)'
  if (props.provider === 'Syncfusion') return 'Syncfusion (code-first, no HTML)'
  return 'jsreport (Handlebars + Chromium)'
})
const footerNote = computed(() => {
  if (props.provider === 'play-sync') return '⚠ Playwright only substitutes {{Token}} — no Handlebars'
  if (props.provider === 'Syncfusion') return 'ℹ Syncfusion builds documents programmatically — no HTML template'
  return '✓ Chromium renders this HTML exactly. Chart.js runs before the PDF is captured.'
})
const templates = computed(() => {
  if (props.provider === 'play-sync') return [{ id: 'playwright', name: 'Simple', icon: '📄' }]
  if (props.provider === 'Syncfusion') return [{ id: 'syncfusion', name: 'Native .NET', icon: '💎' }]
  return [
    { id: 'full',      name: 'Full Report', icon: '📊' },
    { id: 'executive', name: 'Executive',   icon: '🌙' },
  ]
})

const previewData = ref([
  { label: 'Leadership',   value: 4.1 },
  { label: 'Psychosocial', value: 3.2 },
  { label: 'Health',       value: 3.8 },
  { label: 'Safety',       value: 4.5 },
  { label: 'Resources',    value: 3.5 },
])

function switchTemplate(id: string) { activeTemplate.value = id; renderPreview() }

function getHtml(): string {
  if (props.provider === 'Syncfusion') return buildSyncfusionPlaceholder()
  if (props.provider === 'play-sync') return buildPlaywrightPreview()
  const tmpl = activeTemplate.value === 'executive' ? 'executive' : 'full'
  return buildPreviewHtml(tmpl, {
    title: 'Survey Analysis Report',
    company: 'Storkommunen AB',
    questions: previewData.value.map(d => ({ text: d.label + ' example question', category: d.label, avg: d.value, responses: 25 })),
  })
}

function buildSyncfusionPlaceholder(): string {
  return `<!DOCTYPE html><html><head><meta charset="UTF-8"><style>*{margin:0;padding:0;box-sizing:border-box}body{font-family:'Segoe UI',sans-serif;padding:40px;color:#1e293b;background:#f8fafc}.card{background:white;border-radius:12px;padding:32px;border:1px solid #e2e8f0;max-width:600px;margin:0 auto}h2{font-size:20px;font-weight:700;color:#0f172a;margin-bottom:8px}p{font-size:13px;color:#64748b;line-height:1.6;margin-bottom:16px}.code{background:#f1f5f9;border-radius:8px;padding:16px;font-family:monospace;font-size:11px;color:#334155;line-height:1.6}</style></head><body><div class="card"><div style="font-size:48px;margin-bottom:16px">💎</div><h2>Syncfusion: Code-First</h2><p>Syncfusion builds documents via .NET object models. The PDF/Excel/PPT output is generated programmatically — there is no HTML template to preview here.</p><div class="code">sheet.Range["A1"].Text = "Question";<br>sheet.Range["B1"].Text = "Category";<br>sheet.Range["C1"].Formula = "=AVERAGE(D2:D100)";</div></div></body></html>`
}

function buildPlaywrightPreview(): string {
  const rows = previewData.value.map((d, i) => `<tr><td style="padding:7px 12px">Q${i+1}: ${d.label}</td><td style="padding:7px 12px;font-weight:700">${d.value.toFixed(2)}</td></tr>`).join('')
  return `<!DOCTYPE html><html><head><meta charset="UTF-8"><style>*{margin:0;padding:0;box-sizing:border-box}body{font-family:'Segoe UI',sans-serif;padding:32px;color:#1e293b}h1{font-size:22px;font-weight:800;color:#0f2d4a;margin-bottom:4px}.sub{color:#64748b;font-size:13px;margin-bottom:24px}table{width:100%;border-collapse:collapse;font-size:12px}thead{background:#0f2d4a;color:white}th{padding:9px 12px;text-align:left}td{border-bottom:1px solid #e2e8f0}tr:nth-child(even) td{background:#f8fafc}.notice{padding:12px;background:#fffbeb;border:1px solid #fcd34d;border-radius:6px;font-size:11px;color:#92400e;margin-top:20px}</style></head><body><h1>Survey Analysis Report</h1><div class="sub">Storkommunen AB · ${new Date().toLocaleDateString('sv-SE')}</div><table><thead><tr><th>Question</th><th>Average</th></tr></thead><tbody>${rows}</tbody></table><div class="notice">⚠ Playwright: token replacement only. No Chart.js, no #each loops.</div></body></html>`
}

function renderPreview() {
  const frame = frameRef.value
  if (!frame) return
  isLoading.value = true
  const html = getHtml()
  nextTick(() => {
    try {
      const doc = frame.contentDocument
      if (doc) { doc.open(); doc.write(html); doc.close() }
    } finally { setTimeout(() => { isLoading.value = false }, 300) }
  })
}

watch(activeTemplate, renderPreview)
onMounted(() => {
  if (props.provider === 'play-sync') activeTemplate.value = 'playwright'
  else if (props.provider === 'Syncfusion') activeTemplate.value = 'syncfusion'
  setTimeout(renderPreview, 100)
})
</script>

<style scoped>
.live-preview{background:var(--surface,#1e293b);border:1px solid var(--border,#334155);border-radius:14px;overflow:hidden}
.lp-header{display:flex;justify-content:space-between;align-items:flex-start;padding:16px 20px;background:var(--surface-2,#0f172a);border-bottom:1px solid var(--border,#334155);gap:16px;flex-wrap:wrap}
.lp-eyebrow{display:block;font-size:9px;letter-spacing:2.5px;text-transform:uppercase;color:var(--accent,#a855f7);margin-bottom:4px;opacity:.7}
.lp-header h3{font-size:14px;font-weight:700;margin:0 0 2px;color:var(--text,#f1f5f9)}
.lp-sub{font-size:11px;color:var(--text-muted,#64748b)}
.lp-controls{display:flex;gap:6px;align-items:center;flex-wrap:wrap;flex-shrink:0}
.tmpl-btn{font-size:11px;padding:5px 12px;border-radius:6px;border:1px solid var(--border,#334155);background:transparent;color:var(--text-muted,#64748b);cursor:pointer;transition:all .12s}
.tmpl-btn.active{border-color:var(--accent,#a855f7);color:var(--accent,#a855f7);background:color-mix(in srgb,var(--accent,#a855f7) 8%,transparent)}
.refresh-btn{width:28px;height:28px;border-radius:6px;border:1px solid var(--border,#334155);background:transparent;color:var(--text-muted,#64748b);cursor:pointer;font-size:14px;display:flex;align-items:center;justify-content:center}
.refresh-btn:hover{border-color:var(--accent,#a855f7);color:var(--accent,#a855f7)}
.lp-frame-wrap{position:relative;height:520px;background:#f8fafc}
.lp-loading{position:absolute;inset:0;display:flex;align-items:center;justify-content:center;gap:10px;background:rgba(0,0,0,.4);z-index:10;font-size:13px;color:#94a3b8}
.lp-spinner{width:18px;height:18px;border:2px solid rgba(255,255,255,.1);border-top-color:var(--accent,#a855f7);border-radius:50%;animation:spin .7s linear infinite}
@keyframes spin{to{transform:rotate(360deg)}}
.lp-frame{width:100%;height:100%;border:none}
.lp-footer{display:flex;justify-content:space-between;padding:8px 20px;background:var(--surface-2,#0f172a);border-top:1px solid var(--border,#334155)}
.lf-engine{font-size:10px;color:var(--text-muted,#64748b)}.lf-note{font-size:10px;color:var(--text-muted,#64748b);text-align:right;max-width:60%}
</style>
