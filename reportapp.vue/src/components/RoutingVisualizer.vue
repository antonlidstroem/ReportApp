<template>
  <div class="routing-viz">
    <div class="rv-header">
      <span class="rv-eyebrow">ARCHITECTURE</span>
      <h3>Hur requests routas</h3>
      <p>Varje format triggar en annan engine. C#-controllern väljer vid runtime.</p>
    </div>
    <div class="rv-diagram">
      <div class="rv-node user-node">
        <div class="rn-icon">👤</div>
        <div class="rn-label">Client Request</div>
        <div class="format-selector">
          <button v-for="f in formats" :key="f.id"
            :class="['fs-btn', { active: activeFormat === f.id }]"
            @click="activeFormat = f.id">{{ f.icon }} {{ f.label }}</button>
        </div>
      </div>
      <div class="rv-arrow"><div class="arrow-label">POST /api/reports/export</div></div>
      <div class="rv-node controller-node">
        <div class="rn-icon">⚙️</div>
        <div class="rn-label">ReportsController</div>
        <div class="rn-code">
          <code v-for="f in formats" :key="f.id" :class="{ highlight: activeFormat === f.id }">
            "{{ f.id }}" → {{ getEngine(f.id) }}
          </code>
        </div>
      </div>
      <div class="rv-engines">
        <div v-for="eng in activeEngines" :key="eng.id" class="rv-engine" :style="`--ec:${eng.color}`">
          <span>{{ eng.icon }}</span>
          <span>{{ eng.name }}</span>
        </div>
      </div>
    </div>
    <div class="rv-code-panel">
      <div class="rcp-header"><span>{{ pdfEngine === 'playwright' ? 'PlaySyncHybridProvider.cs' : 'JsSyncHybridProvider.cs' }}</span><span class="rcp-lang">C#</span></div>
      <pre class="rcp-code"><code>{{ routerCode }}</code></pre>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
const props = defineProps<{ pdfEngine: 'jsreport' | 'playwright' }>()
const activeFormat = ref('pdf')
const formats = [
  { id: 'pdf',   label: 'PDF',   icon: '📕' },
  { id: 'excel', label: 'Excel', icon: '📗' },
  { id: 'ppt',   label: 'PPT',   icon: '📘' },
]
function getEngine(fmt: string) {
  if (fmt === 'pdf') return props.pdfEngine === 'playwright' ? 'PlaywrightProvider' : 'JsReportProvider'
  if (fmt === 'excel') return 'SyncfusionExcelGen'
  return 'SyncfusionPptGen'
}
const activeEngines = computed(() => {
  const pdfEng = props.pdfEngine === 'playwright'
    ? { id: 'pw', icon: '🎭', name: 'Playwright', color: '#10b981' }
    : { id: 'js', icon: '🌐', name: 'jsreport',   color: '#a855f7' }
  if (activeFormat.value === 'pdf') return [pdfEng]
  if (activeFormat.value === 'excel') return [{ id: 'sf-e', icon: '📊', name: 'Syncfusion XlsIO',       color: '#14b8a6' }]
  return [{ id: 'sf-p', icon: '📋', name: 'Syncfusion Presentation', color: '#14b8a6' }]
})
const routerCode = computed(() => props.pdfEngine === 'playwright'
  ? `public string Name => "play-sync";\n\n// PDF → Playwright\npublic Task<byte[]> GeneratePdfAsync(data) =>\n    _playwright.GeneratePdfAsync(BuildHtml(data));\n\n// Excel → Syncfusion XlsIO\npublic Task<byte[]> GenerateExcelAsync(data) =>\n    _excel.GenerateAsync(data);\n\n// PPT → Syncfusion Presentation\npublic Task<byte[]> GeneratePptAsync(data) =>\n    _ppt.GenerateAsync(data);`
  : `public string Name => "js-sync";\n\n// PDF → jsreport (Handlebars + Chart.js)\npublic Task<byte[]> GeneratePdfAsync(data) =>\n    _pdf.GenerateAsync(data, DefaultTemplate);\n\n// Excel → Syncfusion XlsIO\npublic Task<byte[]> GenerateExcelAsync(data) =>\n    _excel.GenerateAsync(data);\n\n// PPT → Syncfusion Presentation\npublic Task<byte[]> GeneratePptAsync(data) =>\n    _ppt.GenerateAsync(data);`)
</script>

<style scoped>
.routing-viz{background:#06101e;border:1px solid rgba(255,255,255,.06);border-radius:14px;overflow:hidden;margin-bottom:24px}
.rv-header{padding:24px 28px 16px;border-bottom:1px solid rgba(255,255,255,.05)}
.rv-eyebrow{font-size:10px;letter-spacing:3px;color:#475569;display:block;margin-bottom:4px}
.rv-header h3{font-size:16px;font-weight:700;color:#fff;margin:0 0 4px}
.rv-header p{font-size:12px;color:#475569;margin:0}
.rv-diagram{padding:24px;display:flex;align-items:center;gap:16px;flex-wrap:wrap}
.rv-node{background:#0d1a2e;border:1px solid rgba(255,255,255,.08);border-radius:10px;padding:16px;flex-shrink:0;min-width:130px}
.rn-icon{font-size:22px;margin-bottom:6px}
.rn-label{font-size:12px;font-weight:700;color:#e2e8f0;margin-bottom:8px}
.format-selector{display:flex;flex-direction:column;gap:4px}
.fs-btn{font-size:11px;padding:4px 8px;border-radius:5px;border:1px solid rgba(255,255,255,.1);background:transparent;color:#64748b;cursor:pointer;transition:all .12s}
.fs-btn.active{border-color:#f59e0b;color:#f59e0b;background:rgba(245,158,11,.08)}
.rn-code{margin-top:8px;background:rgba(0,0,0,.3);border-radius:6px;padding:8px 10px}
.rn-code code{display:block;font-family:'Courier New',monospace;font-size:9px;color:#475569;line-height:1.6;transition:color .2s}
.rn-code code.highlight{color:#f59e0b;font-weight:700}
.rv-arrow{flex:1;min-width:60px;text-align:center;font-size:9px;color:#374151}
.rv-engines{display:flex;flex-direction:column;gap:8px}
.rv-engine{display:flex;align-items:center;gap:8px;padding:10px 14px;background:#0d1a2e;border:1px solid color-mix(in srgb,var(--ec) 30%,transparent);border-radius:8px;font-size:12px;color:#e2e8f0;font-weight:600}
.rv-code-panel{border-top:1px solid rgba(255,255,255,.05);background:#020a14}
.rcp-header{display:flex;justify-content:space-between;padding:10px 20px;font-size:11px;color:#475569;border-bottom:1px solid rgba(255,255,255,.04)}
.rcp-lang{color:#14b8a6;font-weight:700}
.rcp-code{margin:0;padding:16px 20px;overflow-x:auto}
.rcp-code code{font-family:'Courier New',monospace;font-size:11px;line-height:1.7;color:#94a3b8;white-space:pre}
</style>
