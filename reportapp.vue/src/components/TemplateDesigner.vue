<template>
  <div class="template-designer">
    <div class="td-header">
      <h3>🎨 Malldesigner</h3>
      <div class="template-presets">
        <button v-for="t in presets" :key="t.name" class="preset-btn"
          :class="{ active: activePreset === t.name }" @click="loadPreset(t)">{{ t.name }}</button>
        <button class="preset-btn save-btn" @click="saveCustom">💾 Spara</button>
      </div>
    </div>
    <div class="note" v-if="!supportsHtmlTemplate">
      ⚠️ <strong>{{ providerName }}</strong> stödjer inte HTML-mallar. Mallen visas men används ej vid export.
    </div>
    <div class="td-body">
      <div class="editor-pane">
        <div class="pane-label">
          HTML-mall
          <span class="var-hint" @click="showVars = !showVars">{{ showVars ? '▲' : '▼' }} variabler</span>
        </div>
        <transition name="slide">
          <div v-if="showVars" class="var-list">
            <code v-for="v in templateVars" :key="v" @click="insertVar(v)">{{ v }}</code>
          </div>
        </transition>
        <textarea ref="editorEl" v-model="store.templateHtml" class="code-editor"
          spellcheck="false" @keydown.tab.prevent="insertTab"></textarea>
        <div class="editor-footer">
          <span>{{ store.templateHtml.length }} tecken</span>
          <button @click="formatHtml" class="fmt-btn">✨ Formatera</button>
        </div>
      </div>
      <div class="preview-pane">
        <div class="pane-label">Förhandsgranskning
          <button @click="refreshPreview" class="refresh-btn" title="Uppdatera">↺</button>
        </div>
        <iframe ref="previewFrame" class="preview-iframe"
          sandbox="allow-same-origin allow-scripts" title="Template preview"></iframe>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, watch, onMounted, nextTick } from 'vue'
import { useReportBuilderStore } from '../stores/reportBuilder'
import { defaultTemplate, executiveTemplate, detailedTemplate } from '../assets/templates/index'

const props = defineProps<{ providerName: string; supportsHtmlTemplate: boolean; mockData?: Record<string, unknown> }>()
const store = useReportBuilderStore()
const editorEl = ref<HTMLTextAreaElement>()
const previewFrame = ref<HTMLIFrameElement>()
const showVars = ref(false)
const activePreset = ref('Standard')
const presets = [
  { name: 'Standard',   html: defaultTemplate  },
  { name: 'Executive',  html: executiveTemplate },
  { name: 'Detaljerad', html: detailedTemplate  },
]
const templateVars = [
  '{{SurveyTitle}}','{{CompanyName}}','{{GeneratedAt}}','{{StartDate}}','{{EndDate}}',
  '{{QuestionCount}}','{{TotalResponses}}','{{OverallAverage}}','{{CategoryCount}}',
  '{{#each QuestionSummaries}}','{{Text}}','{{Category}}','{{AverageValue}}','{{TotalResponses}}','{{/each}}'
]
const MOCK: Record<string, unknown> = {
  SurveyTitle: 'Förhandsgranskning — Exempelrapport', CompanyName: 'Demo AB',
  GeneratedAt: new Date().toLocaleDateString('sv-SE'),
  StartDate: '2025-01-01', EndDate: '2025-12-31',
  QuestionCount: 3, TotalResponses: 87, OverallAverage: '3.85', CategoryCount: 2,
  QuestionSummaries: [
    { Text: 'Exempelfråga 1 — Hur mår du?',       Category: 'Hälsa',        AverageValue: '4.2', TotalResponses: 31 },
    { Text: 'Exempelfråga 2 — Upplever du stress?',Category: 'Psykosocialt', AverageValue: '3.1', TotalResponses: 28 },
    { Text: 'Exempelfråga 3 — Stöd från chef?',    Category: 'Ledarskap',    AverageValue: '4.0', TotalResponses: 28 },
  ]
}
function renderTemplate(html: string, data: Record<string, unknown>): string {
  let result = html
  const eachReg = /\{\{#each (\w+)\}\}([\s\S]*?)\{\{\/each\}\}/g
  result = result.replace(eachReg, (_, key, body) => {
    const arr = data[key] as Record<string, unknown>[]
    if (!Array.isArray(arr)) return ''
    return arr.map((item, idx) => {
      let row = body.replace(/\{\{@index\}\}/g, String(idx + 1))
      for (const [k, v] of Object.entries(item))
        row = row.replace(new RegExp(`\\{\\{${k}\\}\\}`, 'g'), String(v))
      return row
    }).join('')
  })
  result = result.replace(/\{\{#if (\w+)\}\}([\s\S]*?)\{\{\/if\}\}/g, (_, key, body) => data[key] ? body : '')
  for (const [k, v] of Object.entries(data))
    if (typeof v !== 'object') result = result.replace(new RegExp(`\\{\\{${k}\\}\\}`, 'g'), String(v))
  return result
}
function refreshPreview() {
  if (!previewFrame.value) return
  const data = { ...MOCK, ...(props.mockData ?? {}) }
  const rendered = renderTemplate(store.templateHtml, data)
  const doc = previewFrame.value.contentDocument
  if (doc) { doc.open(); doc.write(rendered); doc.close() }
}
watch(() => store.templateHtml, () => refreshPreview(), { flush: 'post' })
onMounted(() => {
  if (!store.templateHtml) store.templateHtml = defaultTemplate
  nextTick(refreshPreview)
})
function loadPreset(t: { name: string; html: string }) { activePreset.value = t.name; store.templateHtml = t.html }
function saveCustom() {
  localStorage.setItem(`reportapp:template:custom:${props.providerName}`, store.templateHtml)
  alert('Mall sparad!')
}
function insertVar(v: string) {
  const el = editorEl.value
  if (!el) return
  const s = el.selectionStart, e = el.selectionEnd
  store.templateHtml = store.templateHtml.slice(0, s) + v + store.templateHtml.slice(e)
  nextTick(() => { el.selectionStart = el.selectionEnd = s + v.length; el.focus() })
}
function insertTab() {
  const el = editorEl.value
  if (!el) return
  const s = el.selectionStart
  store.templateHtml = store.templateHtml.slice(0, s) + '  ' + store.templateHtml.slice(s)
  nextTick(() => { el.selectionStart = el.selectionEnd = s + 2 })
}
function formatHtml() {
  let level = 0
  const lines = store.templateHtml.replace(/></g, '>\n<').split('\n').map(l => l.trim()).filter(Boolean)
  store.templateHtml = lines.map(line => {
    if (line.startsWith('</')) level = Math.max(0, level - 1)
    const indent = '  '.repeat(level)
    if (!line.startsWith('</') && !line.endsWith('/>') && line.includes('<') && !line.includes('</')) level++
    return indent + line
  }).join('\n')
}
</script>

<style scoped>
.template-designer{background:var(--surface,#1e293b);border:1px solid var(--border,#334155);border-radius:12px;overflow:hidden}
.td-header{display:flex;justify-content:space-between;align-items:center;flex-wrap:wrap;gap:8px;padding:14px 18px;background:var(--surface-2,#0f172a);border-bottom:1px solid var(--border,#334155)}
.td-header h3{font-size:14px;font-weight:600;margin:0}
.template-presets{display:flex;gap:6px;flex-wrap:wrap}
.preset-btn{font-size:12px;padding:5px 12px;border-radius:6px;border:1px solid var(--border,#334155);background:transparent;color:var(--text,#f1f5f9);cursor:pointer;transition:all .12s}
.preset-btn:hover{background:var(--surface-2,#0f172a)}.preset-btn.active{background:var(--accent,#a855f7);color:white;border-color:var(--accent,#a855f7)}
.save-btn{border-color:#22c55e;color:#22c55e}.save-btn:hover{background:#22c55e;color:white}
.note{margin:12px;padding:10px 14px;background:color-mix(in srgb,#f59e0b 10%,transparent);border:1px solid #f59e0b;border-radius:8px;font-size:12px;color:var(--text,#f1f5f9)}
.td-body{display:grid;grid-template-columns:1fr 1fr;height:420px}
.editor-pane,.preview-pane{display:flex;flex-direction:column;overflow:hidden}
.editor-pane{border-right:1px solid var(--border,#334155)}
.pane-label{display:flex;justify-content:space-between;align-items:center;font-size:11px;font-weight:600;text-transform:uppercase;letter-spacing:.5px;color:var(--text-muted,#64748b);padding:8px 14px;background:var(--surface-2,#0f172a);border-bottom:1px solid var(--border,#334155)}
.var-hint{font-size:10px;cursor:pointer;color:var(--accent,#a855f7);text-transform:none;letter-spacing:0}
.var-list{display:flex;flex-wrap:wrap;gap:4px;padding:8px;background:var(--surface-2,#0f172a);border-bottom:1px solid var(--border,#334155)}
.var-list code{font-size:10px;padding:2px 7px;background:color-mix(in srgb,var(--accent,#a855f7) 12%,transparent);color:var(--accent,#a855f7);border-radius:4px;cursor:pointer;font-family:'Courier New',monospace}
.var-list code:hover{background:var(--accent,#a855f7);color:white}
.code-editor{flex:1;resize:none;border:none;outline:none;padding:12px;font-family:'Courier New',Courier,monospace;font-size:11px;line-height:1.6;background:#1e1e1e;color:#d4d4d4;tab-size:2}
.editor-footer{display:flex;justify-content:space-between;align-items:center;padding:6px 12px;background:#1e1e1e;border-top:1px solid #333;font-size:10px;color:#666}
.fmt-btn{font-size:10px;background:transparent;border:1px solid #444;color:#aaa;padding:2px 8px;border-radius:4px;cursor:pointer}
.fmt-btn:hover{border-color:var(--accent,#a855f7);color:var(--accent,#a855f7)}
.refresh-btn{background:transparent;border:none;color:var(--text-muted,#64748b);cursor:pointer;font-size:14px}
.refresh-btn:hover{color:var(--accent,#a855f7)}
.preview-iframe{flex:1;border:none;background:white}
.slide-enter-active,.slide-leave-active{transition:all .15s}.slide-enter-from,.slide-leave-to{opacity:0;transform:translateY(-4px)}
</style>
